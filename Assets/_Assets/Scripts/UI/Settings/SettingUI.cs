using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Settings;

namespace WatKhaoWong.UI.Settings
{
    public class SettingUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;

        [Header("Settings UI Stuffs")]
        [SerializeField] private Button _accountButton;
        [SerializeField] private Button _notificationButton;
        [SerializeField] private Button _languageButton;
        [SerializeField] private Button _supportButton;
        [SerializeField] private Button _creditsButton;
        [Space]
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _musicSlider;
        [Space]
        [SerializeField] private TMP_Text _notificationStatusText;
        #endregion



        #region --Fields-- (In Class)
        private Setting _playerSetting;
        private NotificationPopup _notificationPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _playerSetting = player.GetComponentInChildren<Setting>();
            _notificationPopup = player.GetComponentInChildren<NotificationPopup>();

            _backButton.onClick.AddListener(Back);

            _accountButton.onClick.AddListener(Account);
            _notificationButton.onClick.AddListener(Notification);
            _languageButton.onClick.AddListener(Language);
            _supportButton.onClick.AddListener(Support);
            _creditsButton.onClick.AddListener(Credits);

            _sfxSlider.onValueChanged.AddListener(SfxSliderValueChanged);
            _musicSlider.onValueChanged.AddListener(MusicSliderValueChanged);

            UIRefresher.OnSettingRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

            UIRefresher.OnLocalizeDynamicString += () => _notificationStatusText.text = _notificationPopup.GetNotificationSwitchStatus();
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerSetting.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Account() => _playerSetting.OnAccountButtonClick();
        private void Notification() => _playerSetting.OnNotificationButtonClick();
        private void Language() => _playerSetting.OnLanguageButtonClick();
        private void Support() => _playerSetting.OnSupportButtonClick();
        private void Credits() => _playerSetting.OnCreditsButtonClick();

        private void SfxSliderValueChanged(float value) => _playerSetting.SaveSfxValue(value);
        private void MusicSliderValueChanged(float value) => _playerSetting.SaveMusicValue(value);

        private void RefreshUI()
        {
            _sfxSlider.value = _playerSetting.LoadSfxValue();
            _musicSlider.value = _playerSetting.LoadMusicValue();

            _notificationStatusText.text = _notificationPopup.GetNotificationSwitchStatus();
        }
        #endregion
    }
}