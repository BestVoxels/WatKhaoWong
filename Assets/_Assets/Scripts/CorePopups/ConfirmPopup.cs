using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.CorePopups
{
    public class ConfirmPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Confirm Popup UI Settings")]
        [field: SerializeField] public string TitleText { get; private set; } = "Title Text";
        [field: Space]
        [field: SerializeField] public string InfoText { get; private set; } = "Info Text";
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