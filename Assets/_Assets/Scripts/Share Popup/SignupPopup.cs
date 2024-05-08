using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.SharePopup
{
    public class SignupPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Signup Popup General Settings")]
        [field: SerializeField] public byte MinimumFirstNameLength { get; private set; } = 5;
        [field: SerializeField] public byte MinimumLastNameLength { get; private set; } = 5;
        [field: SerializeField] public byte MinimumPasswordLength { get; private set; } = 6;
        [field: Space]
        [field: Header("Signup Popup Status Text")]
        [field: SerializeField] public string StatusFirstNameTooShort { get; private set; } = "Your first name must be at least 5 characters long. Please try another.";
        [field: SerializeField] public Color32 StatusFirstNameTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusLastNameTooShort { get; private set; } = "Your last name must be at least 5 characters long. Please try another.";
        [field: SerializeField] public Color32 StatusLastNameTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusPasswordTooShort { get; private set; } = "Your password must be at least 6 characters long. Please try another.";
        [field: SerializeField] public Color32 StatusPasswordTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusConfirmPasswordNotMatch { get; private set; } = "Confirm Password & Password must match!";
        [field: SerializeField] public Color32 StatusConfirmPasswordNotMatchColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Signup Popup UI Event")]
        [SerializeField] private UnityEvent _onInformTextClick;
        [SerializeField] private UnityEvent _onLoginTextClick;
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

        public void OnLoginTextClick()
        {
            Debug.LogWarning("Click \"Login Text\" UI!");

            _onLoginTextClick?.Invoke();
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