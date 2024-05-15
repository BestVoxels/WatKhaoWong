using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.SharePopup
{
    public class LoginPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Login Popup Status Text")]
        [field: SerializeField] public string StatusInvalidUserName { get; private set; } = "Invalid Email or Phone Number.";
        [field: SerializeField] public Color32 StatusInvalidUserNameColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusInvalidPassword { get; private set; } = "Invalid Account or Password.";
        [field: SerializeField] public Color32 StatusInvalidPasswordColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusForgotPassword { get; private set; } = "Please contact for support at developer website.";
        [field: SerializeField] public Color32 StatusForgotPasswordColor { get; private set; }
        #endregion



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