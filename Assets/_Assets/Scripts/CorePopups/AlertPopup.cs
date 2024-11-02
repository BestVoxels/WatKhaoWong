using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.CorePopups
{
    public class AlertPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Alert Popup UI Settings")]
        [field: SerializeField] public LocalizedString TitleText { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString InfoText { get; private set; }
        [field: SerializeField] public Color32 InfoTextColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Alert Popup UI Event")]
        [SerializeField] private UnityEvent _onAlertButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnAlertButtonClick()
        {
            _onAlertButtonClick?.Invoke();
        }
        #endregion
    }
}