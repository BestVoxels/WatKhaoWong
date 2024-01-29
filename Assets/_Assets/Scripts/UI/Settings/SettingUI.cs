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
        [SerializeField] private TMP_Text _languageStatusText;
        [SerializeField] private Image _languageStatusIcon;
        #endregion



        #region --Fields-- (In Class)
        private Setting _playerSetting;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerSetting = GameObject.FindWithTag("Player").GetComponentInChildren<Setting>();

            _backButton.onClick.AddListener(Back);

            _accountButton.onClick.AddListener(Account);
            _notificationButton.onClick.AddListener(Notification);
            _languageButton.onClick.AddListener(Language);
            _supportButton.onClick.AddListener(Support);
            _creditsButton.onClick.AddListener(Credits);

            _sfxSlider.onValueChanged.AddListener(SfxSliderValueChanged);
            _musicSlider.onValueChanged.AddListener(MusicSliderValueChanged);
        }

        private void OnEnable()
        {
            UIRefresher.OnSettingRefreshed += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            UIRefresher.OnSettingRefreshed -= RefreshUI;
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

            // TODO update these UI, wait for 'LanguagePopup' and 'NotificationPopup' classes
            //_notificationStatusText.text = "";
            //_languageStatusText.text = "";
            //_languageStatusIcon.overrideSprite = ;
        }
        #endregion
    }
}