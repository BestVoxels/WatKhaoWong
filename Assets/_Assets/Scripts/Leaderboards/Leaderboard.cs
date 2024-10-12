using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Identities;
using Firebase.Database;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.Conditions;
using WatKhaoWong.Challenges;
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
        [Range(1, 3)]
        [SerializeField] private ushort _rewardWinnerMaxRowNumber = 1;
        [Range(3, 5)]
        [SerializeField] private ushort _recordWinnerMaxRowNumber = 3;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Leaderboard Filter Settings")]
        [field: SerializeField] public Color32 SelectedColor { get; private set; }
        [field: SerializeField] public Color32 UnselectedColor { get; private set; }

        [field: Space]

        [field: Header("Leaderboard Status Text")]
        [field: SerializeField] public string NoAllTimeLeaderboardText { get; private set; } = "Upload Now! Be the first on All Time Leaderboard";
        [field: SerializeField] public string NoTodayLeaderboardText { get; private set; } = "Upload Now! Be the first on Today Leaderboard";
        [field: SerializeField] public string NoChallengeLeaderboardText { get; private set; } = "No Active Challenge at the moment.";
        [field: SerializeField] public string PendingChallengeLeaderboardText { get; private set; } = "Challenge is pending. Stay tuned!";
        [field: SerializeField] public string LiveChallengeLeaderboardText { get; private set; } = "Challenge is live! Upload Now!";
        [field: Space]
        [field: Tooltip("Not likely to be shown.")]
        [field: SerializeField] public string HasAllTimeLeaderboardText { get; private set; } = "Displaying data for All Time leaderboard";
        [field: Tooltip("Not likely to be shown.")]
        [field: SerializeField] public string HasTodayLeaderboardText { get; private set; } = "Displaying data for Today leaderboard";
        [field: Tooltip("Not likely to be shown.")]
        [field: SerializeField] public string HasChallengeLeaderboardText { get; private set; } = "Displaying data for the Active Challenge";
        [field: Space]
        [field: SerializeField] public string NoChallengeBannerText { get; private set; } = $"Please wait for Admin to create a Challenge";
        [field: Space]
        [field: SerializeField] public string PendingChallengeBannerTextBegin { get; private set; } = $"Challenge Starts in ";
        [field: SerializeField] public string PendingChallengeBannerTextEnd { get; private set; } = $"!";
        [field: Space]
        [field: SerializeField] public string LiveChallengeBannerTextBegin { get; private set; } = $"Ends in ";
        [field: SerializeField] public string LiveChallengeBannerTextEnd { get; private set; } = $"! Aim for first place to win the Challenge!";
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnLeaderboardCategoryChanged;
        public event Action OnLeaderboardScoreUpdated;
        public event Action OnConditionIsLeaderboardExistsUpdated; // Important: 'IsLeaderboardExists' takes sometimes to get value. Once the value is updated, this will make all classes that use Condition.Check() to Evaluate using this 'IsLeaderboardExists' Condition again.
        #endregion



        #region --Fields-- (In Class)
        private RecordCollection _records;
        private bool _isAsyncRunning = false;

        private DateTime _leaderboardFirstUploadTimeOfDayTM;
        private bool _isLeaderboardTMTodayExists = false;
        private DateTime _leaderboardFirstUploadTimeOfChallengeTM;
        private bool _isLeaderboardTMChallengeExists = false;

        private Challenge _challenge;
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
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
            _challenge = GameObject.FindWithTag("Player").GetComponentInChildren<Challenge>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged; // This will trigger on Start() too so don't have to call LoadSave() on Start()
            _myUserData.OnTodayTMPointsAdded += AddTodayTMPointsToLeaderboard;
            _myUserData.OnChallengeTMPointsAdded += AddChallengeTMPointsToLeaderboard;
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
            _myUserData.OnTodayTMPointsAdded -= AddTodayTMPointsToLeaderboard;
            _myUserData.OnChallengeTMPointsAdded -= AddChallengeTMPointsToLeaderboard;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Leaderboard~
        public ushort GetMyRank() => Records[Category].MyRank;

        public ELeaderboardPresence GetMyPresence() => Records[Category].MyPresence;

        public bool IsLeaderboardExists() => Records[Category].IsLeaderboardExists;

        public async IAsyncEnumerable<OtherUserData> GetRows()
        {
            if (_isAsyncRunning) yield break; // Prevent duplicates Rows Bug. Since we are dealing with 'await' so we only allow ONE instance of this method to run at a time.

            _isAsyncRunning = true;

            IAsyncEnumerable<DataSnapshot> rows = Category switch
            {
                ELeaderboardCategory.AllTime => GetRowsFromServer(ECategoryNode.Users, EValueNode.TotalTMPoint),
                ELeaderboardCategory.Today => GetRowsFromServer(ECategoryNode.LeaderboardTMToday, EValueNode.TodayTMPoint),
                ELeaderboardCategory.Challenge => GetRowsFromServer(ECategoryNode.LeaderboardTMChallenge, EValueNode.ChallengeTMPoint),
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
                    if (!IsLeaderboardExists())
                    {
                        Records[Category].IsLeaderboardExists = true;
                        OnConditionIsLeaderboardExistsUpdated?.Invoke();
                    }

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
            bool isChallengeSaveLoaded = await _challenge.LoadCompletionSource.Task;

            if (isChallengeSaveLoaded == false)
            {
                Debug.LogError("Could not continue LoadSave() on Leaderboard.cs because Challenge.cs LoadSave() is not completed.");
                return;
            }

            _isLeaderboardTMTodayExists = await _savingWrapper.IsLeaderboardTMTodayExists();

            var data = await _savingWrapper.Load(ECategoryNode.LeaderboardStats, EValueNode.FirstUploadTimeOfDayTM);
            if (data != null)
            {
                if (DateTime.TryParse(data.Value.ToString(), out DateTime result))
                    _leaderboardFirstUploadTimeOfDayTM = result;

                DeleteTodayTMLeaderboardDaily();
            }

            _isLeaderboardTMChallengeExists = await _savingWrapper.IsLeaderboardTMChallengeExists();

            data = await _savingWrapper.Load(ECategoryNode.LeaderboardStats, EValueNode.FirstUploadTimeOfChallengeTM);
            if (data != null)
            {
                if (DateTime.TryParse(data.Value.ToString(), out DateTime result))
                    _leaderboardFirstUploadTimeOfChallengeTM = result;

                await DeleteChallengeTMLeaderboardAfterEnd();
            }

            // Clear All Record Category CachedRows so that it has to fetch from database again.
            Records.ClearAllCachedRows();
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

        private void AssignTodayUploadTime()
        {
            if (!_isLeaderboardTMTodayExists)
            {
                _leaderboardFirstUploadTimeOfDayTM = DateTime.Now;
                _savingWrapper.Save(ECategoryNode.LeaderboardStats, EValueNode.FirstUploadTimeOfDayTM, DateTime.Now.ToString());
            }
        }

        private async Task DeleteChallengeTMLeaderboardAfterEnd()
        {
            if (_leaderboardFirstUploadTimeOfChallengeTM == default) return;

            if ((!_challenge.CanLive(_leaderboardFirstUploadTimeOfChallengeTM) || !_challenge.CanLiveNow()) && _isLeaderboardTMChallengeExists)
            {
                await RewardChallengeWinnerAfterEnd();
                _savingWrapper.ForceDeleteLeaderboardTMChallenge();

                _isLeaderboardTMChallengeExists = false;
            }
        }

        private void AssignChallengeUploadTime()
        {
            if (!_isLeaderboardTMChallengeExists)
            {
                _leaderboardFirstUploadTimeOfChallengeTM = DateTime.Now;
                _savingWrapper.Save(ECategoryNode.LeaderboardStats, EValueNode.FirstUploadTimeOfChallengeTM, DateTime.Now.ToString());
            }
        }

        private async Task RewardChallengeWinnerAfterEnd()
        {
            if ((_challenge.CanLive(_leaderboardFirstUploadTimeOfChallengeTM) && _challenge.CanLiveNow()) || !_isLeaderboardTMChallengeExists) return;

            ushort i = _rewardWinnerMaxRowNumber;
            await foreach (DataSnapshot eachData in _savingWrapper.LoadAndSortByChildValue(ECategoryNode.LeaderboardTMChallenge, EValueNode.ChallengeTMPoint, _recordWinnerMaxRowNumber))
            {
                DataSnapshot data = await _savingWrapper.LoadOtherUser(eachData.Key);
                OtherUserData anyUserData = new OtherUserData(data);

                // Record Winners
                _savingWrapper.ForceSaveChallengeTMWinner(_challenge.GetID(), eachData.Key, anyUserData.GetChallengeTMPoints());

                // Reward Winner
                for (; i > 0; i--)
                {
                    int totalChallengeTMWonPoint = anyUserData.GetTotalChallengeTMWon() + 1;

                    _savingWrapper.ForceSaveAnyUser(ECategoryNode.Users, eachData.Key, EValueNode.ChallengeTMWon, totalChallengeTMWonPoint);
                }
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

                case EConditionType.IsLeaderboardExists:
                    return IsLeaderboardExists();
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
            DeleteTodayTMLeaderboardDaily();

            if (score <= 0) return;

            AssignTodayUploadTime();

            // Add score to leaderboard
            _savingWrapper.Save(ECategoryNode.LeaderboardTMToday, EValueNode.TodayTMPoint, score);

            // Clear All Record Category CachedRows so that it has to fetch from database again. Why all? IF 'today score' updated, that means 'alltime score' has to be updated as well.
            Records.ClearAllCachedRows();
            OnLeaderboardScoreUpdated?.Invoke();
        }

        private void AddChallengeTMPointsToLeaderboard(int score)
        {
            _ = DeleteChallengeTMLeaderboardAfterEnd();

            if (score <= 0 || !_challenge.CanLiveNow()) return;

            AssignChallengeUploadTime();

            // Add score to leaderboard
            _savingWrapper.Save(ECategoryNode.LeaderboardTMChallenge, EValueNode.ChallengeTMPoint, score);

            // Clear All Record Category CachedRows so that it has to fetch from database again. Why all? IF 'today score' updated, that means 'alltime score' has to be updated as well.
            Records.ClearAllCachedRows();
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

            // PUBLIC Methods
            public void ClearAllCachedRows()
            {
                foreach (Record each in _records)
                    each.CachedRows.Clear();
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