using UnityEngine;
using WatKhaoWong.Leaderboards;
using WatKhaoWong.Identity;

namespace WatKhaoWong.UI.Leaderboards
{
    public class LeaderboardUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Leaderboard Stuffs")]
        [SerializeField] private GameObject _countDownTimerGameObject;
        [SerializeField] private GameObject _noChallengePanel;
        [SerializeField] private Transform _tabsTransform;
        [SerializeField] private RowUI _myRowUI;

        [Space]

        [Header("Spawn Stuffs")]
        [SerializeField] private RowUI _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        #endregion



        #region --Fields-- (In Class)
        private bool _isAsyncRunning = false;

        private Leaderboard _leaderboard;
        private MyUserData _myUserData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _leaderboard = GameObject.FindWithTag("Player").GetComponentInChildren<Leaderboard>();
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();

            _leaderboard.OnLeaderboardCategoryChanged += RefreshUI;

            UIRefresher.OnLeaderboardRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
        }

        private void Start()
        {
            InitialSetup();

            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void InitialSetup()
        {
            SetupFilterButtonsUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Row~
        private async void BuildRows()
        {
            if (_isAsyncRunning) return; // Prevent duplicates Rows Bug. Since we are dealing with 'await' so we only allow ONE instance of this method to run at a time.

            ushort rowCounter = 1;

            _isAsyncRunning = true;
            await foreach (OtherUserData otherUserData in _leaderboard.GetRows())
            {
                RowUI createdPrefab = Instantiate(_rowPrefab, _spawnParent);

                createdPrefab.Setup(otherUserData, rowCounter, _leaderboard.Category, RowUI.IsInLeaderboard.Yes);

                ++rowCounter;
            }
            
            _isAsyncRunning = false;
        }

        private void SetupMyRow()
        {
            RowUI.IsInLeaderboard isMeInLeaderboard = _leaderboard.IsMeInLeaderboard() ? RowUI.IsInLeaderboard.Yes : RowUI.IsInLeaderboard.No;

            _myRowUI.Setup(_myUserData, _leaderboard.GetMyUserRank(), _leaderboard.Category, isMeInLeaderboard);
        }

        private void ClearRows()
        {
            foreach (Transform eachChild in _spawnParent)
                Destroy(eachChild.gameObject);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~FilterButtons~
        private void SetupFilterButtonsUI()
        {
            foreach (FilterButtonUI button in _tabsTransform.GetComponentsInChildren<FilterButtonUI>())
            {
                button.Setup(_leaderboard);
            }
        }

        private void UpdateFilterButtonsUI()
        {
            foreach (FilterButtonUI button in _tabsTransform.GetComponentsInChildren<FilterButtonUI>())
            {
                button.RefreshUI();
            }
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RefreshUI()
        {
            UpdateFilterButtonsUI();

            // Row
            ClearRows();

            BuildRows();

            SetupMyRow();
        }
        #endregion
    }
}