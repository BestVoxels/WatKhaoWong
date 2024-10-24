using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Settings;
using WatKhaoWong.Utils.UI;
using Michsky.MUIP;

namespace WatKhaoWong.UI.Settings
{
    public class NotificationPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Notification Popup UI Stuffs")]
        [SerializeField] private SwitchManager _notificationSwitch;
        [Space]
        [SerializeField] private Toggle[] _timeToggles;
        [Space]
        [SerializeField] private GameObject[] _toggleGameObjects;
        #endregion



        #region --Fields-- (In Class)
        private int _togglesOnCount = 0;
        private bool _isUIRefreshedAtStart = false;

        private NotificationPopup _playerNotificationPopup;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerNotificationPopup = GameObject.FindWithTag("Player").GetComponentInChildren<NotificationPopup>();
            _statusText = FindAnyObjectByType<StatusText>();

            _closeButton.onClick.AddListener(Close);

            _notificationSwitch.onValueChanged.AddListener(SwitchChanged);

            foreach (Toggle each in _timeToggles)
                each.onValueChanged.AddListener(ToggleChanged);
        }

        private void Start()
        {
            RefreshNotificationSwitchUI();

            RefreshTogglesUIAtStart();

            RefreshToggleGameObjects(_notificationSwitch.isOn);

            _isUIRefreshedAtStart = true; // Need this because we don't want to trigger 'SwitchChanged()' 'ToggleChanged()' subscriber methods when updating UI.
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshNotificationSwitchUI()
        {
            _notificationSwitch.isOn = _playerNotificationPopup.LoadNotificationSwitchValue();

            _notificationSwitch.UpdateUI();
        }

        private void RefreshToggleGameObjects(bool isOnStatus)
        {
            foreach (GameObject each in _toggleGameObjects)
                each.SetActive(isOnStatus);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Only Run At Start~
        private void RefreshTogglesUIAtStart()
        {
            if (_timeToggles.Length != _playerNotificationPopup.DefaultTimeTogglesLength)
            {
                Debug.LogError("Can't Refresh Toggles UI. Length should matches between assigned UI Toggles and specified in Inspector of NotificationPopup.");
                return;
            }

            bool[] loadValues = _playerNotificationPopup.LoadToggleIsOnValues();
            for (byte i = 0; i < loadValues.Length; i++)
            {
                _timeToggles[i].isOn = loadValues[i];

                if (loadValues[i])
                    _togglesOnCount++;
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerNotificationPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void SwitchChanged(bool isOn)
        {
            if (!_isUIRefreshedAtStart) return; // DON'T run code below if UI is not yet refreshed at start.

            // Show Status Text
            if (isOn)
                _statusText.Show(_playerNotificationPopup.StatusSwitchOn.GetLocalizedString(), _playerNotificationPopup.StatusSwitchOnColor);
            else
                _statusText.Show(_playerNotificationPopup.StatusSwitchOff.GetLocalizedString(), _playerNotificationPopup.StatusSwitchOffColor);

            // When Switch is ON & all Toggles are OFF -> Turn any Toggle ON randomly
            if (isOn && _togglesOnCount <= 0)
            {
                _timeToggles[Random.Range(0, _timeToggles.Length)].isOn = true;
            }

            RefreshToggleGameObjects(isOn);

            _playerNotificationPopup.SaveNotificationSwitchValue(isOn);

            _playerNotificationPopup.OnNotificationSwitchClick(); // Need to be below 'Save' since it will update Settings' UI according to what it's saved.
        }

        private void ToggleChanged(bool isOn)
        {
            if (!_isUIRefreshedAtStart) return; // DON'T run code below if UI is not yet refreshed at start.

            // Update ToggleOnCount
            if (isOn)
                _togglesOnCount++;
            else
                _togglesOnCount--;

            // When all Toggles are OFF -> Turn OFF Switch
            if (_togglesOnCount <= 0)
            {
                SwitchChanged(false);
                RefreshNotificationSwitchUI();
            }

            _playerNotificationPopup.SaveToggleValues(_timeToggles);
        }
        #endregion
    }
}