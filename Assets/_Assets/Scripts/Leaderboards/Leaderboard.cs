using System.Collections.Generic;
using System;
using UnityEngine;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Identity;
using Firebase.Database;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.Conditions;
using Firebase.Auth;

namespace WatKhaoWong.Leaderboards
{
    public class Leaderboard : MonoBehaviour, IConditionEvaluator
    {
        #region --Fields-- (Inspector)
        [Header("Leaderboard Settings")]
        [SerializeField] private ELeaderboardCategory _defaultCategory;
        [Range(1, 200)]
        [SerializeField] private int _maxRowNumber = 100;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Leaderboard Filter Settings")]
        [field: SerializeField] public Color32 SelectedColor { get; private set; }
        [field: SerializeField] public Color32 UnselectedColor { get; private set; }

        [field: Space]

        [field: Header("Leaderboard Status Text")]
        [field: SerializeField] public string NoChallengeText { get; private set; } = "No active Challenge at the moment";
        [field: SerializeField] public string CountDownChallengeTextBegin { get; private set; } = $"Challenge ends in ";
        [field: SerializeField] public string CountDownChallengeTextEnd { get; private set; } = $" days!";
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnLeaderboardCategoryChanged;
        public event Action OnLeaderboardScoreUpdated;
        #endregion



        #region --Fields-- (In Class)
        private bool _isAsyncRunning = false;
        private bool _isLeaderboardTMTodayExists = false;

        private ushort _myUserRank = 9999;
        private bool _isMeInLeaderboard = false;
        private List<DataSnapshot> _allTimeRows = new();

        private List<DataSnapshot> _todayRows = new();
        private DateTime _leaderboardFirstUploadTimeOfDayTM;

        private SavingWrapper _savingWrapper;
        private MyUserData _myUserData;
        #endregion



        #region --Properties-- (With Backing Fields)
        public ELeaderboardCategory Category
        {
            get => _defaultCategory;

            set
            {
                _defaultCategory = value;

                OnLeaderboardCategoryChanged?.Invoke();
            }
        }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged; // This will trigger on Start() too so don't have to call LoadSave() on Start()
            _myUserData.OnTodayTMPointsAdded += AddTodayTMPointsToLeaderboard;
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
            _myUserData.OnTodayTMPointsAdded -= AddTodayTMPointsToLeaderboard;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Leaderboard~
        public ushort GetMyUserRank()
        {
            if (_isMeInLeaderboard == false)
                _myUserRank = (ushort)_maxRowNumber;

            return _myUserRank;
        }

        public bool IsMeInLeaderboard() => _isMeInLeaderboard;

        public async IAsyncEnumerable<OtherUserData> GetRows()
        {
            if (_isAsyncRunning) yield break; // Prevent duplicates Rows Bug. Since we are dealing with 'await' so we only allow ONE instance of this method to run at a time.

            _isAsyncRunning = true;

            IAsyncEnumerable<DataSnapshot> rows = Category switch
            {
                ELeaderboardCategory.AllTime => GetAllTimeRows(),
                ELeaderboardCategory.Today => GetTodayRows(),
                ELeaderboardCategory.Challenge => null,
                _ => null
            };

            if (rows == null)
            {
                Debug.LogError("Error : Can't fetch data to display rows on learderboard. Because 'rows' is null.");
                _isAsyncRunning = false;
                yield break;
            }

            await foreach (DataSnapshot eachData in rows)
                yield return new OtherUserData(eachData);

            _isAsyncRunning = false;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async IAsyncEnumerable<DataSnapshot> GetAllTimeRows()
        {
            // *** First Initialize List & also Return Asynchronous one by one when loaded from server. ***
            if (_allTimeRows.Count == 0)
            {
                ushort index = 0;
                await foreach (DataSnapshot each in _savingWrapper.LoadAndSortByChildValue(ECategoryNode.Users, EValueNode.TotalTMPoint, _maxRowNumber))
                {
                    ++index;
                    if (each.Key.Equals(FirebaseUtils.CurrentUserID))
                    {
                        _myUserRank = index;
                        _isMeInLeaderboard = true;
                    }
                    
                    _allTimeRows.Add(each);

                    yield return each;
                }

                yield break; // Important to stop here because 'await' will resume call and if we don't end here it will run code below too.
            }
            
            // *** Return List as Synchronous ***
            foreach (DataSnapshot each in _allTimeRows)
                yield return each;
        }

        private async IAsyncEnumerable<DataSnapshot> GetTodayRows()
        {
            // *** First Initialize List & also Return Asynchronous one by one when loaded from server. ***
            if (_todayRows.Count == 0)
            {
                ushort index = 0;
                await foreach (DataSnapshot eachDataOnlyHasKey in _savingWrapper.LoadAndSortByChildValue(ECategoryNode.LeaderboardTMToday, EValueNode.TodayTMPoint, _maxRowNumber))
                {
                    ++index;
                    if (eachDataOnlyHasKey.Key.Equals(FirebaseUtils.CurrentUserID))
                    {
                        _myUserRank = index;
                        _isMeInLeaderboard = true;
                    }

                    // On Server Side: 'each' ONLY has Key, it has no data inside
                    DataSnapshot fullDataSnapshot = await _savingWrapper.LoadOtherUser(eachDataOnlyHasKey.Key);
                    _todayRows.Add(fullDataSnapshot);

                    yield return fullDataSnapshot;
                }

                yield break; // Important to stop here because 'await' will resume call and if we don't end here it will run code below too.
            }

            // *** Return List as Synchronous ***
            foreach (DataSnapshot each in _todayRows)
                yield return each;
        }

        private async void LoadSave()
        {
            _isLeaderboardTMTodayExists = await _savingWrapper.IsLeaderboardTMTodayExists();

            var data = await _savingWrapper.Load(ECategoryNode.LeaderboardStats, EValueNode.FirstUploadTimeOfDayTM);
            if (data != null)
            {
                if (DateTime.TryParse(data.Value.ToString(), out DateTime result))
                    _leaderboardFirstUploadTimeOfDayTM = result;

                DeleteTodayTMLeaderboardDaily();
            }
        }

        private void DeleteTodayTMLeaderboardDaily()
        {
            if (_leaderboardFirstUploadTimeOfDayTM == default) return;

            if (_leaderboardFirstUploadTimeOfDayTM.Date != DateTime.Today && _isLeaderboardTMTodayExists)
            {
                _savingWrapper.ForceDeleteLeaderboardTMToday();
                _isLeaderboardTMTodayExists = false;
            }
        }

        private void AssignUploadTime()
        {
            if (!_isLeaderboardTMTodayExists)
            {
                _leaderboardFirstUploadTimeOfDayTM = DateTime.Now;
                _savingWrapper.Save(ECategoryNode.LeaderboardStats, EValueNode.FirstUploadTimeOfDayTM, DateTime.Now.ToString());
            }
        }
        #endregion



        #region --Methods-- (Interface)
        bool? IConditionEvaluator.Evaluate(EConditionType conditionType, EConditionValue[] conditionValues)
        {
            switch (conditionType)
            {
                case EConditionType.IsLeaderboardCategoryEquals:
                    byte stringStartIndex = (byte)EConditionType.IsLeaderboardCategoryEquals;
                    string enumString = conditionValues[0].ToString()[stringStartIndex..];

                    if (!Enum.TryParse(enumString, true, out ELeaderboardCategory result))
                        return false;

                    return Category == result;
            }

            return null;
        }
        #endregion



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        /// </summary>
        private void HandleStateChanged(object obj, EventArgs args)
        {
            LoadSave(); // So Don't have to call on Awake()
        }

        private void AddTodayTMPointsToLeaderboard(int score)
        {
            if (score <= 0) return;

            DeleteTodayTMLeaderboardDaily();

            AssignUploadTime();

            // Add score to leaderboard
            _savingWrapper.Save(ECategoryNode.LeaderboardTMToday, EValueNode.TodayTMPoint, score);

            // Clear Lists so that it has to fetch from database again.
            _todayRows = new();
            OnLeaderboardScoreUpdated?.Invoke();
        }
        #endregion
    }
}