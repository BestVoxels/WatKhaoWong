using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using WatKhaoWong.Prays;
using WatKhaoWong.Identities;

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
        [SerializeField] private ProfileIconInspector _icon;
        [Space]
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _allTimeTMPointsText;
        [SerializeField] private TMP_Text _todayTMPointsText;
        [SerializeField] private TMP_Text _challengeTMPointsText;
        [Space]
        [SerializeField] private Button _recordManuallyButton;
        [SerializeField] private Button _meditateButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _endButton;
        [Space]
        [SerializeField] private TMP_Text _meditateText;
        #endregion



        #region --Fields-- (In Class)
        private Pray _pray;
        private MyUserData _myUserData;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 160f / 135f;  // Formula : [CHANGE THIS] PrayUI Profile's Size  %  [FIX] Inventory Profile's Size (original looks)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _pray = GameObject.FindWithTag("Player").GetComponentInChildren<Pray>();
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();

            _backButton.onClick.AddListener(Back);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => UserProfile((PointerEventData)data));
            _userProfileEventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => UserStats((PointerEventData)data));
            _userStatsEventTrigger.triggers.Add(entry);

            _recordManuallyButton.onClick.AddListener(RecordManually);
            _meditateButton.onClick.AddListener(Meditate);
            _pauseButton.onClick.AddListener(Pause);
            _endButton.onClick.AddListener(End);

            UIRefresher.OnPrayRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

            UIRefresher.OnLocalizeDynamicString += RefreshStatUI;
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UpdateMeditateText()
        {
            if (_pray.ToStartMeditateText)
                _meditateText.text = _pray.MeditateText.GetLocalizedString();
            else
                _meditateText.text = _pray.ContinueText.GetLocalizedString();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _pray.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void UserProfile(PointerEventData data) => _pray.OnUserProfileClick();

        private void UserStats(PointerEventData data) => _pray.OnUserStatsClick();

        private void RecordManually() => _pray.OnRecordManuallyButtonClick();

        private void Meditate()
        {
            _pray.OnPlaySoundButtonClick();
        }

        private void Pause()
        {
            _pray.OnPauseSoundButtonClick();

            UpdateMeditateText();
        }

        private void End()
        {
            _pray.OnEndSoundButtonClick();

            UpdateMeditateText();
        }

        private void RefreshUI()
        {
            _myUserData.UpdateProfileIcon(_icon, _myUserData.GetProfileIcon(), MultiplierRatioForDecorator);

            RefreshStatUI();
        }

        private void RefreshStatUI()
        {
            _userNameText.text = _myUserData.GetUserNameText();

            _allTimeTMPointsText.text = _pray.AllTimeText.GetLocalizedString($"{_pray.ValueTextFormatBegin}{_myUserData.GetTotalTMPointsText()}{_pray.ValueTextFormatEnd}");
            _todayTMPointsText.text = _pray.TodayText.GetLocalizedString($"{_pray.ValueTextFormatBegin}{_myUserData.GetTodayTMPointsText()}{_pray.ValueTextFormatEnd}");
            _challengeTMPointsText.text = _pray.ChallengeText.GetLocalizedString($"{_pray.ValueTextFormatBegin}{_myUserData.GetChallengeTMPointsText()}{_pray.ValueTextFormatEnd}");

            UpdateMeditateText();
        }
        #endregion
    }
}