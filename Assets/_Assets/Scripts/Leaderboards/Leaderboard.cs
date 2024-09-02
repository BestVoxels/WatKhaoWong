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
        [SerializeField] private ushort _maxRowNumber = 100;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Leaderboard Filter Settings")]
        [field: SerializeField] public Color32 SelectedColor { get; private set; }
        [field: SerializeField] public Color32 UnselectedColor { get; private set; }

        [field: Space]

        [field: Header("Leaderboard Status Text")]
        [field: SerializeField] public string NoAllTimeLeaderboardText { get; private set; } = "No data for All Time leaderboard";
        [field: SerializeField] public string NoTodayLeaderboardText { get; private set; } = "No data for Today leaderboard";
        [field: SerializeField] public string NoChallengeLeaderboardText { get; private set; } = "No active Challenge at the moment";
        [field: Space]
        [field: Tooltip("Not likely to be shown.")]
        [field: SerializeField] public string HasAllTimeLeaderboardText { get; private set; } = "Displaying data for All Time leaderboard";
        [field: Tooltip("Not likely to be shown.")]
        [field: SerializeField] public string HasTodayLeaderboardText { get; private set; } = "Displaying data for Today leaderboard";
        [field: Tooltip("Not likely to be shown.")]
        [field: SerializeField] public string HasChallengeLeaderboardText { get; private set; } = "Displaying data for the Active Challenge";
        [field: Space]
        [field: SerializeField] public string ChallengeBannerTextBegin { get; private set; } = $"Challenge ends in ";
        [field: SerializeField] public string ChallengeBannerTextEnd { get; private set; } = $" days!";
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnLeaderboardCategoryChanged;
        public event Action OnLeaderboardScoreUpdated;
        #endregion



        #region --Fields-- (In Class)
        private RecordCollection _records;

        private bool _isAsyncRunning = false;
        private DateTime _leaderboardFirstUploadTimeOfDayTM;
        private bool _isLeaderboardTMTodayExists = false;

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

        // Doing this way to PREVENT Null Error from accessing Records. This way it will gets value when it needs, no need to initialize on Start().
        private RecordCollection Records
        {
            get
            {
                if (_records == null)
                    _records = new(_maxRowNumber);

                return _records;
            }

            set => _records = value;
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
        public ushort GetMyRank() => Records[Category].MyRank;

        public ELeaderboardPresence GetMyPresence() => Records[Category].MyPresence;

        public bool IsLeaderboardExists() => Records[Category].IsLeaderboardExists;

        public int GetChallengeDayLeft() => 10; // TODO return properly

        public async IAsyncEnumerable<OtherUserData> GetRows()
        {
            if (_isAsyncRunning) yield break; // Prevent duplicates Rows Bug. Since we are dealing with 'await' so we only allow ONE instance of this method to run at a time.

            _isAsyncRunning = true;

            IAsyncEnumerable<DataSnapshot> rows = Category switch
            {
                ELeaderboardCategory.AllTime => GetRowsFromServer(ECategoryNode.Users, EValueNode.TotalTMPoint),
                ELeaderboardCategory.Today => GetRowsFromServer(ECategoryNode.LeaderboardTMToday, EValueNode.TodayTMPoint),
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
        private async IAsyncEnumerable<DataSnapshot> GetRowsFromServer(ECategoryNode categoryNode, EValueNode valueNode)
        {
            // *** First Initialize List & also Return Asynchronous one by one when loaded from server. ***
            if (Records[Category].CachedRows.Count == 0)
            {
                ushort index = 0;
                await foreach (DataSnapshot eachData in _savingWrapper.LoadAndSortByChildValue(categoryNode, valueNode, _maxRowNumber))
                {
                    ++index;
                    Records[Category].IsLeaderboardExists = true;

                    if (eachData.Key.Equals(FirebaseUtils.CurrentUserID))
                    {
                        Records[Category].MyRank = index;
                        Records[Category].MyPresence = ELeaderboardPresence.Present;
                    }

                    DataSnapshot data = eachData;
                    // IF data is from 'LeaderboardTMToday' or 'LeaderboardTMChallenge', 'eachData' ONLY has Key, it has no data inside
                    if (categoryNode == ECategoryNode.LeaderboardTMToday || categoryNode == ECategoryNode.LeaderboardTMChallenge)
                        data = await _savingWrapper.LoadOtherUser(eachData.Key);

                    Records[Category].CachedRows.Add(data);

                    yield return data;
                }

                yield break; // Important to stop here because 'await' will resume call and if we don't end here it will run code below too.
            }

            // *** Return List as Synchronous ***
            foreach (DataSnapshot each in Records[Category].CachedRows)
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
            Records[ELeaderboardCategory.Today].CachedRows.Clear();
            OnLeaderboardScoreUpdated?.Invoke();
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        private class Record
        {
            private readonly ushort _maxRowNumber;
            private ushort _myRank = 9999;

            public ushort MyRank
            {
                get
                {
                    if (MyPresence == ELeaderboardPresence.Absent)
                        _myRank = _maxRowNumber;

                    return _myRank;
                }
                set => _myRank = value;
            }

            public ELeaderboardPresence MyPresence { get; set; } = ELeaderboardPresence.Absent;
            public bool IsLeaderboardExists { get; set; } = false;
            public List<DataSnapshot> CachedRows { get; private set; } = new();

            public Record(ushort maxRowNumber)
            {
                _maxRowNumber = maxRowNumber;
            }
        }

        private class RecordCollection
        {
            // Collection
            private readonly Record[] _records = new Record[3];

            // Indexer
            public Record this[ELeaderboardCategory category]
            {
                get => _records[GetInt(category)];
            }

            // Constructor
            public RecordCollection(ushort maxRowNumber)
            {
                for (byte i = 0; i < _records.Length; i++)
                    _records[i] = new Record(maxRowNumber);
            }

            // Methods
            private int GetInt(ELeaderboardCategory category)
            {
                return category switch
                {
                    ELeaderboardCategory.AllTime => 0,
                    ELeaderboardCategory.Today => 1,
                    ELeaderboardCategory.Challenge => 2,
                    _ => -1
                };
            }
        }
        #endregion
    }
}