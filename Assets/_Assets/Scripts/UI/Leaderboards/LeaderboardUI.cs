using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Leaderboards;
using WatKhaoWong.Identities;
using WatKhaoWong.Challenges; // TODO Challenge - remove this namespace

namespace WatKhaoWong.UI.Leaderboards
{
    public class LeaderboardUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Leaderboard Stuffs")]
        [SerializeField] private TMP_Text _dataIndicatorText;
        [SerializeField] private TMP_Text _countDownBannerText;
        [Space]
        [SerializeField] private Button _challengeButton;
        [Space]
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
        private ChallengeCreationPopup _challenge; // TODO Challenge - change from "ChallengePopup.cs" to "Challenge.cs"
        private MyUserData _myUserData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _leaderboard = player.GetComponentInChildren<Leaderboard>();
            _challenge = player.GetComponentInChildren<ChallengeCreationPopup>(); // TODO Challenge - change from "ChallengePopup.cs" to "Challenge.cs"
            _myUserData = player.GetComponentInChildren<MyUserData>();

            _challengeButton.onClick.AddListener(StartChallenge);

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
        private async Task BuildRows()
        {
            if (_isAsyncRunning) return; // Prevent duplicates Rows Bug. Since we are dealing with 'await' so we only allow ONE instance of this method to run at a time.

            ushort rowCounter = 1;

            _isAsyncRunning = true;
            await foreach (OtherUserData otherUserData in _leaderboard.GetRows())
            {
                RowUI createdPrefab = Instantiate(_rowPrefab, _spawnParent);

                createdPrefab.Setup(otherUserData, rowCounter, _leaderboard.Category, ELeaderboardPresence.Present, _leaderboard.IsLeaderboardExists());

                ++rowCounter;
            }
            _isAsyncRunning = false;
        }

        private void SetupMyRow()
        {
            _myRowUI.Setup(_myUserData, _leaderboard.GetMyRank(), _leaderboard.Category, _leaderboard.GetMyPresence(), _leaderboard.IsLeaderboardExists());
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



        #region --Methods-- (Custom PRIVATE) ~Texts~
        private void UpdateTexts()
        {
            if (!_leaderboard.IsLeaderboardExists())
                _dataIndicatorText.text = _leaderboard.Category switch
                {
                    ELeaderboardCategory.AllTime => _leaderboard.NoAllTimeLeaderboardText,
                    ELeaderboardCategory.Today => _leaderboard.NoTodayLeaderboardText,
                    ELeaderboardCategory.Challenge => _leaderboard.NoChallengeLeaderboardText,
                    _ => ""
                };
            else
                _dataIndicatorText.text = _leaderboard.Category switch
                {
                    ELeaderboardCategory.AllTime => _leaderboard.HasAllTimeLeaderboardText,
                    ELeaderboardCategory.Today => _leaderboard.HasTodayLeaderboardText,
                    ELeaderboardCategory.Challenge => _leaderboard.HasChallengeLeaderboardText,
                    _ => ""
                };

            if (_challenge.HasChallengeStarted)
                _countDownBannerText.text = $"{_leaderboard.HasChallengeBannerTextBegin}{_challenge.GetChallengeEndDaysLeft()}{_leaderboard.HasChallengeBannerTextEnd}";
            else
                _countDownBannerText.text = $"{_leaderboard.NoChallengeBannerText}";
        }
        #endregion



        #region --Methods-- (Subscriber)
        private async void RefreshUI()
        {
            UpdateFilterButtonsUI();
            UpdateTexts();

            // Row
            ClearRows();

            await BuildRows(); // Have to call before 'SetupMyRow()' and have wait until it finished. So that it setup 'MyRank' properly.

            SetupMyRow();
        }

        private void StartChallenge() => _leaderboard.OnChallengeButtonClick();
        #endregion
    }
}