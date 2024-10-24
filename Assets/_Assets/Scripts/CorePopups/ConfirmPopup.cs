using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.CorePopups
{
    public class ConfirmPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Confirm Popup UI Settings")]
        [field: SerializeField] public LocalizedString TitleText { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString InfoText { get; private set; }
        [field: SerializeField] public Color32 InfoTextColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Confirm Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            _onCancelButtonClick?.Invoke();
        }

        public void OnConfirmButtonClick()
        {
            _onConfirmButtonClick?.Invoke();
        }
        #endregion
    }
}