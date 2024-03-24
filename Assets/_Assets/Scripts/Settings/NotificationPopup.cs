using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Settings
{
    public class NotificationPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Notification Stuffs")]
        [SerializeField] private bool _defaultNotificationSwitchValue = true;
        [SerializeField] private bool[] _defaultTimeTogglesValue;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Notification Status Text")]
        [field: SerializeField] public string StatusSwitchOff { get; private set; } = "Notification is now disabled.";
        [field: SerializeField] public Color32 StatusSwitchOffColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusSwitchOn { get; private set; } = "Notification is now enabled.";
        [field: SerializeField] public Color32 StatusSwitchOnColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Notification Popup UI Event")]
        [SerializeField] private UnityEvent _onNotificationSwitchClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnNotificationSwitchChanged;
        #endregion



        #region --Fields-- (Constant)
        private const string KeyNotificationSwitch = "KeyNotificationSwitchValue";
        private const string KeyTimeToggles = "KeyTimeTogglesValue";
        #endregion



        #region --Properties-- (Computed)
        public int DefaultTimeTogglesLength => _defaultTimeTogglesValue.Length;
        #endregion



        #region --Methods-- (Built In)
        private void OnApplicationPause(bool isAppPaused)
        {
            if (isAppPaused == true)
            {
                // TODO SEND Notifications data, check how to do on ColorPuzzle project, GameNotification.cs
            }
            else if (isAppPaused == false)
            {
                // TODO CLEAR Old Notifications data, check how to do on ColorPuzzle project, GameNotification.cs
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Switch~
        public bool LoadNotificationSwitchValue() => PlayerPrefsX.GetBool(KeyNotificationSwitch, _defaultNotificationSwitchValue); // return _defaultValue IF 'Key' doesn't exist.

        public void SaveNotificationSwitchValue(bool value) => PlayerPrefsX.SetBool(KeyNotificationSwitch, value);

        public string GetNotificationSwitchStatus()
        {
            return LoadNotificationSwitchValue() ? "ON" : "OFF";
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Toggles~
        public bool[] LoadToggleIsOnValues()
        {
            return PlayerPrefsX.GetBools(KeyTimeToggles, _defaultTimeTogglesValue);
        }

        public void SaveToggleValues(Toggle[] values)
        {
            bool[] isOnValues = values.Select(t => t.isOn).ToArray();

            PlayerPrefsX.SetBools(KeyTimeToggles, isOnValues);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnNotificationSwitchClick()
        {
            Debug.Log("Click \"Notification\" Switch!");

            OnNotificationSwitchChanged?.Invoke();
            _onNotificationSwitchClick?.Invoke();
        }
        #endregion
    }
}