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
        [SerializeField] private Button _doneButton;
        [SerializeField] private Button _playSoundButton;
        #endregion



        #region --Fields-- (In Class)
        private Pray _playerPray;
        private MyUserData _myUserData;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 160f / 135f;  // Formula : [CHANGE THIS] PrayUI Profile's Size  %  [FIX] Inventory Profile's Size (original looks)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerPray = GameObject.FindWithTag("Player").GetComponentInChildren<Pray>();
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

            _doneButton.onClick.AddListener(Done);
            _playSoundButton.onClick.AddListener(PlaySound);

            UIRefresher.OnPrayRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
        }

        private void Start()
        {
            RefreshUI();
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

        private void RefreshUI()
        {
            _myUserData.UpdateProfileIcon(_icon, _myUserData.GetProfileIcon(), MultiplierRatioForDecorator);

            _userNameText.text = _myUserData.GetUserNameText();
            _allTimeTMPointsText.text = _myUserData.GetTotalTMPointsText();
            _todayTMPointsText.text = _myUserData.GetTodayTMPointsText();
            _challengeTMPointsText.text = _myUserData.GetChallengeTMPointsText();
        }
        #endregion
    }
}