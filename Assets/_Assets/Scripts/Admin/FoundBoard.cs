using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using WatKhaoWong.SceneManagement;
using UnityEngine.Localization;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Identities;
using UnityEngine.Events;

namespace WatKhaoWong.Admin
{
    public class FoundBoard : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Board Settings")]
        [SerializeField] private EFoundCategory _defaultCategory;
        #endregion



        #region --Properties-- (Inspector)
        [field: Range(1, 200)]
        [field: SerializeField] public ushort MaxRowNumber { get; private set; } = 50;

        [field: Header("Board Filter Settings")]
        [field: SerializeField] public Color32 SelectedColor { get; private set; }
        [field: SerializeField] public Color32 UnselectedColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString LoadCategoryCompleted { get; private set; }
        [field: SerializeField] public Color32 LoadCategoryCompletedColor { get; private set; }
        [field: SerializeField] public LocalizedString CantChangeCategory { get; private set; }
        [field: SerializeField] public Color32 CantChangeCategoryColor { get; private set; }
        [field: Space]
        [field: SerializeField] public CategoryNameEntry[] CategoryName { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("ManageMembers UI Event")]
        [SerializeField] private UnityEvent _onRegisterMemberButtonClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnCategoryChanged;
        public event Action OnCallRefreshFoundBoardUI;
        public event Action OnIsBoardExistsUpdated;
        #endregion



        #region --Fields-- (In Class)
        private RecordCollection _records;

        private SavingWrapper _savingWrapper;
        private StatusText _statusText;
        private SearchPanel _searchPanel;
        private List<UIStateC> _uiStates = new List<UIStateC>();
        #endregion



        #region --Properties-- (With Backing Fields)
        public EFoundCategory Category
        {
            get => _defaultCategory;

            set
            {
                _defaultCategory = value;

                OnCategoryChanged?.Invoke();
                OnIsBoardExistsUpdated?.Invoke();
            }
        }

        // Doing this way to PREVENT Null Error from accessing Records. This way it will gets value when it needs, no need to initialize on Start().
        private RecordCollection Records
        {
            get
            {
                if (_records == null)
                    _records = new(this);

                return _records;
            }

            set => _records = value;
        }
        #endregion



        #region --Properties-- (Auto)
        public static bool IsAsyncRunning { get; private set; } = false;
        public static bool ShowStatusRowLoaded { get; set; } = false;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _searchPanel = player.GetComponentInChildren<SearchPanel>();

            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _statusText = FindAnyObjectByType<StatusText>();

            foreach (var each in GameObject.FindGameObjectsWithTag("FoundBoard"))
            {
                _uiStates.Add(each.GetComponentInChildren<UIStateC>(true));
            }
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged; // This will trigger on Start() too so don't have to call LoadSave() on Start()
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }

        private async void OnApplicationFocus(bool focusStatus)
        {
            if (!await MyUserData.IsAdmin()) return;

            if (focusStatus)
            {
                ShowStatusRowLoaded = false;

                // IMPORTANT : MUST 'LoadSave()' when open from background, To Get Latest Data from Server. ONLY for 'LoadSave()' that use 'Share Categories', eg LeaderboardStats, ServerStats, RemoteConfig.

                // Example Case :
                // User A open App on Day 1 -> Uploads score into Today Leaderboard & Close App in Background.
                // User B open App on Day 1 -> Uploads score into Today Leaderboard.
                // User C open App on Day 2 -> Deletes Today Leaderboard & Uploads score into Today Leaderboard.
                // User D open App on Day 2 -> Uploads score into Today Leaderboard.
                // User A open App from Background on Day 2 -> Deletes Today Leaderboard (AGAIN! because it stills has old Data from Server!).
                // This is why some users Data NOT showing on Today Leaderboard.
                LoadSave(); // DON'T  [Clear All Record Category CachedRows, because Rows will be missing, it mess up refresh order, just refresh in 'LoadSave()']
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Leaderboard~
        public bool IsBoardExists() => Records[Category].IsBoardExists;

        public async IAsyncEnumerable<IUserData> GetRows(ESearchPanelLocation location)
        {
            //+Prevent duplicates Rows Bug. Since we are dealing with 'await' so we only allow ONE instance of this method to run at a time.
            //+Prevent some LeaderboardUI GameObject show Empty Data (No Rows), solve by make LeaderboardUI GameObject that comes after wait first then loads when Async is done.
            if (IsAsyncRunning) yield break;

            IsAsyncRunning = true;

            IAsyncEnumerable<IUserData> rows = Category switch
            {
                EFoundCategory.Total => GetRowsFromServer(ECategoryNode.Users),
                EFoundCategory.AtTemple => GetRowsFromServer(ECategoryNode.ActiveStay),
                _ => null
            };

            if (rows == null)
            {
                Debug.LogError("Error : Can't fetch data to display rows on learderboard. Because 'rows' is null.");
                IsAsyncRunning = false;
                yield break;
            }

            _searchPanel.SetLocation(location);

            ushort index = 0;
            await foreach (IUserData each in rows)
            {
                if (each == null) continue;

                if (_searchPanel.HasFilter())
                {
                    var filteredData = await _searchPanel.FilterRowData(each);
                    if (filteredData == null) continue;

                    index++;
                    yield return filteredData;
                }
                else
                {
                    index++;
                    yield return each;
                }

                if (!IsBoardExists())
                    Records[Category].IsBoardExists = true;
            }

            if (index == 0)
                Records[Category].IsBoardExists = false;

            IsAsyncRunning = false;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnRegisterMemberButtonClick()
        {
            _onRegisterMemberButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async IAsyncEnumerable<IUserData> GetRowsFromServer(ECategoryNode categoryNode)
        {            
            // *** First Initialize List & also Return Asynchronous one by one when loaded from server. ***
            if (Records[Category].CachedRows.Count == 0)
            {
                ushort index = 0;
                if (categoryNode == ECategoryNode.Users)
                {
                    await foreach (DataSnapshot dataSnapShot in _savingWrapper.LoadAllUsers())
                    {
                        ++index;
                        IUserData data = new OtherUserData(dataSnapShot);
                        await data.GetActiveStayEntry(); // Call this so "SearchPanel" can filter by "ActiveStayEntry" without having to call "await GetActiveStayEntry()".
                        
                        Records[Category].CachedRows.Add(data);

                        yield return data;
                    }
                }
                else if (categoryNode == ECategoryNode.ActiveStay)
                {
                    await foreach ((StayEntry stayEntry, string keyId) in _savingWrapper.LoadEntryFromCategory(ECategoryNode.ActiveStay))
                    {
                        DataSnapshot dataSnapShot = await _savingWrapper.LoadOtherUser(stayEntry.UserId);
                        if (dataSnapShot == null) continue;

                        ++index;
                        IUserData userData = new OtherUserData(dataSnapShot);

                        Records[Category].CachedRows.Add(userData);

                        yield return userData;
                    }
                }

                // Indicates when Leaderboard is loaded.
                if (index > 0 && _uiStates.Any(e => e.gameObject.activeInHierarchy) && ShowStatusRowLoaded)
                {
                    string categoryName = CategoryName.First(e => e.category == Category).localizedString.GetLocalizedString();

                    _statusText.Show(LoadCategoryCompleted.GetLocalizedString(categoryName), LoadCategoryCompletedColor);
                }

                yield break; // Important to stop here because 'await' will resume call and if we don't end here it will run code below too.
            }

            // *** Return List as Synchronous ***
            foreach (IUserData each in Records[Category].CachedRows)
                yield return each;
        }

        private void LoadSave()
        {
            // Clear All Record Category CachedRows so that it has to fetch from database again.
            Records.ClearAllCachedRows();
            OnCallRefreshFoundBoardUI?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake(). And at time of assiging to 'FirebaseAuth.DefaultInstance.StateChanged'
        /// </summary>
        private void HandleStateChanged(object obj, EventArgs args)
        {
            LoadSave(); // So Don't have to call on Awake()
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        private class Record
        {
            private FoundBoard _board;
            private bool _defaultIsBoardExists = false;
            public bool IsBoardExists
            {
                get => _defaultIsBoardExists;
                set
                {
                    _defaultIsBoardExists = value;
                    _board.OnIsBoardExistsUpdated?.Invoke();
                }
            }
            public List<IUserData> CachedRows { get; private set; } = new();

            public Record(FoundBoard foundBoard)
            {
                _board = foundBoard;
            }
        }

        private class RecordCollection
        {
            // Collection
            private readonly Record[] _records = new Record[3];

            // Indexer
            public Record this[EFoundCategory category]
            {
                get => _records[GetInt(category)];
            }

            // Constructor
            public RecordCollection(FoundBoard foundBoard)
            {
                for (byte i = 0; i < _records.Length; i++)
                    _records[i] = new Record(foundBoard);
            }

            // PUBLIC Methods
            public void ClearAllCachedRows()
            {
                foreach (Record each in _records)
                    each.CachedRows.Clear();
            }

            // Methods
            private int GetInt(EFoundCategory category)
            {
                return category switch
                {
                    EFoundCategory.Total => 0,
                    EFoundCategory.AtTemple => 1,
                    _ => -1
                };
            }
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        [System.Serializable]
        public class CategoryNameEntry
        {
            public EFoundCategory category;
            public LocalizedString localizedString;
        }
        #endregion
    }
}