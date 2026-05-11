using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using WatKhaoWong.Leaderboards;
using WatKhaoWong.Identities;
using WatKhaoWong.Challenges;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.UI.Leaderboards
{
    [RequireComponent(typeof(RowUIPool))]
    public class LeaderboardUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Leaderboard Stuffs")]
        [SerializeField] private TMP_Text _dataIndicatorText;
        [SerializeField] private TMP_Text _countDownBannerText;
        [Space]
        [SerializeField] private Transform _tabsTransform;
        [SerializeField] private RowUI _myRowUI;
        #endregion



        #region --Fields-- (In Class)
        private List<RowUI> _activeRowUIs = new List<RowUI>();

        private Leaderboard _leaderboard;
        private Challenge _challenge;
        private MyUserData _myUserData;
        private RowUIPool _rowUIPool;
        #endregion



        #region --Fields-- (Constant)
        private const float WaitAsyncTimeOut = 10f;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _leaderboard = player.GetComponentInChildren<Leaderboard>();
            _challenge = player.GetComponentInChildren<Challenge>();
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _rowUIPool = GetComponent<RowUIPool>();

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
            ClearRows();

            //+Prevent some LeaderboardUI GameObject show Empty Data (No Rows), solve by make LeaderboardUI GameObject that comes after wait first then loads when Async is done.
            float timer = 0f;
            while (Leaderboard.IsAsyncRunning == true)
            {
                timer += Time.deltaTime;

                if (timer >= WaitAsyncTimeOut) return;

                await Task.Delay(100);
            }

            ClearRows(); //+Prevent duplicates Rows Bug.

            ushort rowCounter = 1;
            await foreach (OtherUserData otherUserData in _leaderboard.GetRows())
            {
                RowUI createdPrefab = _rowUIPool.Pool.Get();

                createdPrefab.transform.SetSiblingIndex(rowCounter - 1); // -1 bcuz Index starts at 0.
                createdPrefab.Setup(otherUserData, rowCounter, _leaderboard.Category, ELeaderboardPresence.Present, _leaderboard.IsLeaderboardExists());

                _activeRowUIs.Add(createdPrefab);

                ++rowCounter;
            }
        }

        private void SetupMyRow()
        {
            _myRowUI.Setup(_myUserData, _leaderboard.GetMyRank(), _leaderboard.Category, _leaderboard.GetMyPresence(), _leaderboard.IsLeaderboardExists());
        }

        private void ClearRows()
        {
            foreach (RowUI eachRow in _activeRowUIs)
                eachRow.Release();

            _activeRowUIs.Clear();
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
            if (!FirebaseUtils.IsAuthenticated())
                _dataIndicatorText.text = _leaderboard.Category switch
                {
                    ELeaderboardCategory.AllTime => _leaderboard.GuestAllTimeLeaderboardText.GetLocalizedString(),
                    ELeaderboardCategory.Today => _leaderboard.GuestTodayLeaderboardText.GetLocalizedString(),
                    ELeaderboardCategory.Challenge => _leaderboard.GuestChallengeLeaderboardText.GetLocalizedString(),
                    _ => ""
                };
            else if (!_leaderboard.IsLeaderboardExists())
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
            else if (_leaderboard.IsLeaderboardExists())
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

            await BuildRows(); // Have to call before 'SetupMyRow()' and have wait until it finished. So that it setup 'MyRank' properly.

            SetupMyRow();
        }
        #endregion
    }
}