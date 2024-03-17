using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using WatKhaoWong.Prays;
using UnityEngine.EventSystems;

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
        [SerializeField] private TMP_Text _usernameText;
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
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerPray = GameObject.FindWithTag("Player").GetComponentInChildren<Pray>();

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

            _usernameText.text = _playerPray.GetUsernameText();
            _allTimeTMPointsText.text = _playerPray.GetAllTimePoints().ToString("#,0", nfi);
            _todayTMPointsText.text = _playerPray.GetTodayPoints().ToString("#,0", nfi);

            _challengeText.text = _playerPray.GetChallengeText();
        }
        #endregion
    }
}