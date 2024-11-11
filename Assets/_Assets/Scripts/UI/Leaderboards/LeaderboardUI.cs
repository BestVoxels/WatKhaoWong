using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using WatKhaoWong.Leaderboards;
using WatKhaoWong.Identities;
using WatKhaoWong.Challenges;

namespace WatKhaoWong.UI.Leaderboards
{
    public class LeaderboardUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Leaderboard Stuffs")]
        [SerializeField] private TMP_Text _dataIndicatorText;
        [SerializeField] private TMP_Text _countDownBannerText;
        [Space]
        [SerializeField] private Transform _tabsTransform;
        [SerializeField] private RowUI _myRowUI;

        [Space]

        [Header("Spawn Stuffs")]
        [SerializeField] private RowUI _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        #endregion



        #region --Fields-- (In Class)
        private float _waitAsyncTimeOut = 3f;

        private Leaderboard _leaderboard;
        private Challenge _challenge;
        private MyUserData _myUserData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _leaderboard = player.GetComponentInChildren<Leaderboard>();
            _challenge = player.GetComponentInChildren<Challenge>();
            _myUserData = player.GetComponentInChildren<MyUserData>();

            UIRefresher.OnLeaderboardRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

            UIRefresher.OnLocalizeDynamicString += UpdateTexts;
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
            //+Prevent duplicates Rows Bug. Since we are dealing with 'await' so we only allow ONE instance of this method to run at a time.
            //+Prevent some LeaderboardUI GameObject show Empty Data (No Rows), solve by make LeaderboardUI GameObject that comes after wait first then loads when Async is done.
            float timer = 0f;
            while (Leaderboard.IsAsyncRunning == true)
            {
                timer += Time.deltaTime;

                if (timer >= _waitAsyncTimeOut) return;

                await Task.Delay(100);
            }

            ushort rowCounter = 1;

            await foreach (OtherUserData otherUserData in _leaderboard.GetRows())
            {
                RowUI createdPrefab = Instantiate(_rowPrefab, _spawnParent);

                createdPrefab.Setup(otherUserData, rowCounter, _leaderboard.Category, ELeaderboardPresence.Present, _leaderboard.IsLeaderboardExists());

                ++rowCounter;
            }
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
        private async void UpdateTexts()
        {
            if (!_leaderboard.IsLeaderboardExists())
                _dataIndicatorText.text = _leaderboard.Category switch
                {
                    ELeaderboardCategory.AllTime => _leaderboard.NoAllTimeLeaderboardText.GetLocalizedString(),
                    ELeaderboardCategory.Today => _leaderboard.NoTodayLeaderboardText.GetLocalizedString(),
                    ELeaderboardCategory.Challenge => _challenge.GetStatus() switch
                    {
                        EChallengeStatus.None => _leaderboard.NoChallengeLeaderboardText.GetLocalizedString(),
                        EChallengeStatus.Pending => _leaderboard.PendingChallengeLeaderboardText.GetLocalizedString(),
                        EChallengeStatus.Live => _leaderboard.LiveChallengeLeaderboardText.GetLocalizedString(),
                        _ => ""
                    },
                    _ => ""
                };
            else
                _dataIndicatorText.text = _leaderboard.Category switch
                {
                    ELeaderboardCategory.AllTime => _leaderboard.HasAllTimeLeaderboardText.GetLocalizedString(),
                    ELeaderboardCategory.Today => _leaderboard.HasTodayLeaderboardText.GetLocalizedString(),
                    ELeaderboardCategory.Challenge => _leaderboard.HasChallengeLeaderboardText.GetLocalizedString(),
                    _ => ""
                };

            _countDownBannerText.text = _challenge.GetStatus() switch
            {
                EChallengeStatus.None => _leaderboard.NoChallengeBannerText.GetLocalizedString(),
                EChallengeStatus.Pending => _leaderboard.PendingChallengeBannerText.GetLocalizedString(_challenge.DaysString(await _challenge.GetChallengeStartDaysLeft())),
                EChallengeStatus.Live => _leaderboard.LiveChallengeBannerText.GetLocalizedString(_challenge.DaysString(await _challenge.GetChallengeEndDaysLeft())),
                _ => ""
            };
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
        #endregion
    }
}