using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Globalization;
using TMPro;
using WatKhaoWong.Prays;
using WatKhaoWong.Identity;

namespace WatKhaoWong.UI.Prays
{
    public class PrayUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;

        [Header("Pray UI Stuffs")]
        [SerializeField] private EventTrigger _userProfileEventTrigger;
        [SerializeField] private EventTrigger _userStatsEventTrigger;
        [Space]
        [SerializeField] private AccountData.IconUI _icon;
        [Space]
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _allTimeTMPointsText;
        [SerializeField] private TMP_Text _todayTMPointsText;
        [Space]
        [SerializeField] private Button _doneButton;
        [SerializeField] private Button _playSoundButton;
        [Space]
        [SerializeField] private TMP_Text _challengeText;
        [SerializeField] private Button _challengeButton;
        #endregion



        #region --Fields-- (In Class)
        private Pray _playerPray;
        private AccountData _account;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 165f / 135f;  // Formula : Main Profile's Size (BIG) % Inventory Profile's Size (SMALL)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerPray = GameObject.FindWithTag("Player").GetComponentInChildren<Pray>();
            _account = GameObject.FindWithTag("Player").GetComponentInChildren<AccountData>();

            _backButton.onClick.AddListener(Back);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => UserProfile((PointerEventData)data));
            _userProfileEventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => UserStats((PointerEventData)data));
            _userStatsEventTrigger.triggers.Add(entry);

            _doneButton.onClick.AddListener(Done);
            _playSoundButton.onClick.AddListener(PlaySound);
            _challengeButton.onClick.AddListener(StartChallenge);
        }

        private void OnEnable()
        {
            RefreshUI();

            UIRefresher.OnPrayRefreshed += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            UIRefresher.OnPrayRefreshed -= RefreshUI;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerPray.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void UserProfile(PointerEventData data) => _playerPray.OnUserProfileClick();

        private void UserStats(PointerEventData data) => _playerPray.OnUserStatsClick();

        private void Done() => _playerPray.OnDoneButtonClick();

        private void PlaySound() => _playerPray.OnPlaySoundButtonClick();

        private void StartChallenge() => _playerPray.OnChallengeButtonClick();

        private void RefreshUI()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            _account.UpdateProfileIcon(_icon, _account.GetProfileIcon(), MultiplierRatioForDecorator);

            _userNameText.text = _account.GetUserNameText();
            _allTimeTMPointsText.text = _account.GetAllTimeTMPoints().ToString("#,0", nfi);
            _todayTMPointsText.text = _account.GetTodayTMPoints().ToString("#,0", nfi);

            _challengeText.text = _playerPray.GetChallengeText();
        }
        #endregion
    }
}