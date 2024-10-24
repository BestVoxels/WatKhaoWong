using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using WatKhaoWong.Settings;

namespace WatKhaoWong.UI.Settings
{
    public class LanguagePopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Language Popup UI Stuffs")]
        [SerializeField] private Toggle _ThaiToggle;
        [SerializeField] private Toggle _EngToggle;
        [Space]
        [SerializeField] private Toggle[] _languageToggles;
        #endregion



        #region --Fields-- (In Class)
        private LanguagePopup _playerLanguagePopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerLanguagePopup = GameObject.FindWithTag("Player").GetComponentInChildren<LanguagePopup>();

            _closeButton.onClick.AddListener(Close);

            _ThaiToggle.onValueChanged.AddListener(ThaiToggle);
            _EngToggle.onValueChanged.AddListener(EngToggle);

            foreach (Toggle each in _languageToggles)
                each.onValueChanged.AddListener(ToggleChanged);
        }

        private void Start()
        {
            RefreshTogglesUIBasedOnUnityDefault();

            RefreshTogglesUIBasedOnUserSave();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshTogglesUIBasedOnUnityDefault()
        {
            int selectedIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);

            _languageToggles[selectedIndex].isOn = true;
            _playerLanguagePopup.InvokeOnLanguageToggleIsOn();
        }

        private void RefreshTogglesUIBasedOnUserSave()
        {
            bool[] loadValues = _playerLanguagePopup.LoadToggleIsOnValues();
            if (loadValues == null) return;

            for (byte i = 0; i < loadValues.Length; i++)
            {
                _languageToggles[i].isOn = loadValues[i];

                if (_languageToggles[i].isOn)
                {
                    _playerLanguagePopup.InvokeOnLanguageToggleIsOn();
                }
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerLanguagePopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void ThaiToggle(bool toggleValue)
        {
            if (toggleValue == true)
            {
                _playerLanguagePopup.OnThaiToggleIsOn();

                _playerLanguagePopup.InvokeOnLanguageToggleIsOn();
            }
        }

        private void EngToggle(bool toggleValue)
        {
            if (toggleValue == true)
            {
                _playerLanguagePopup.OnEngToggleIsOn();

                _playerLanguagePopup.InvokeOnLanguageToggleIsOn();
            }
        }

        private void ToggleChanged(bool isOn)
        {
            if (isOn == true)
            {
                _playerLanguagePopup.SaveToggleValues(_languageToggles);

                _playerLanguagePopup.InvokeOnLanguageToggleIsOn();
            }
        }
        #endregion
    }
}