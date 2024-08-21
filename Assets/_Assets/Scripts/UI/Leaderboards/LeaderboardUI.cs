using UnityEngine;
using WatKhaoWong.Leaderboards;

namespace WatKhaoWong.UI.Leaderboards
{
    public class LeaderboardUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Leaderboard Stuffs")]
        [SerializeField] private GameObject _countDownTimerGameObject;
        [SerializeField] private GameObject _noChallengePanel;
        [SerializeField] private Transform _tabsTransform;

        [Space]

        [Header("Spawn Stuffs")]
        [SerializeField] private RowUI _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        #endregion



        #region --Fields-- (In Class)
        private Leaderboard _leaderboard;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _leaderboard = GameObject.FindWithTag("Player").GetComponentInChildren<Leaderboard>();

            _leaderboard.OnCategoryChanged += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

            // TODO have to deal with UIRefresher.cs
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
        }
        #endregion
    }
}