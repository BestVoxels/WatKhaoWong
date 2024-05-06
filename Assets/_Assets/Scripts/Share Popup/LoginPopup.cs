using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.SharePopup
{
    public class LoginPopup : Popup
    {
        #region --Events-- (UnityEvent)
        [Header("Login Popup UI Event")]
        [SerializeField] private UnityEvent _onSignupTextClick;
        [SerializeField] private UnityEvent _onForgotTextClick;
        [Space]
        [SerializeField] private UnityEvent _onConfirmButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonCantClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnSignupTextClick()
        {
            Debug.LogWarning("Click \"Signup Text\" UI!");

            _onSignupTextClick?.Invoke();
        }

        public void OnForgotTextClick()
        {
            Debug.LogWarning("Click \"Forgot Text\" UI!");

            _onForgotTextClick?.Invoke();
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