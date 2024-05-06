using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.SharePopup
{
    public class VerifyPopup : Popup
    {
        #region --Events-- (UnityEvent)
        [Header("Verify Popup UI Event")]
        [SerializeField] private UnityEvent _onInformTextClick;
        [SerializeField] private UnityEvent _onResendTextClick;
        [Space]
        [SerializeField] private UnityEvent _onConfirmButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonCantClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnInformTextClick()
        {
            Debug.LogWarning("Click \"Inform Text\" UI!");

            _onInformTextClick?.Invoke();
        }

        public void OnResendTextClick()
        {
            Debug.LogWarning("Click \"Resend Text\" UI!");

            _onResendTextClick?.Invoke();
        }

        public void OnConfirmButtonClick()
        {
            Debug.LogWarning("Click \"Confirm\" Button! on Popup");

            _onConfirmButtonClick?.Invoke();
        }

        public void OnConfirmButtonCantClick()
        {
            Debug.LogWarning("CANT Click \"Confirm\" Button! on Popup");

            _onConfirmButtonCantClick?.Invoke();
        }
        #endregion
    }
}