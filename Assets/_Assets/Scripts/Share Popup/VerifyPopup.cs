using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.SharePopup
{
    public class VerifyPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Verify Popup General Settings")]
        [field: SerializeField] public byte MinimumCodeLength { get; private set; } = 6;
        [field: Space]
        [field: Header("Verify Popup Status Text")]
        [field: SerializeField] public string StatusCodeTooShort { get; private set; } = "Your code must be at least 6 characters long. Please check the code that we've just sent.";
        [field: SerializeField] public Color32 StatusCodeTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusCodeNotMatch { get; private set; } = "Please enter a valid code. Please check the code that we've just sent.";
        [field: SerializeField] public Color32 StatusCodeNotMatchColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusResendCode { get; private set; } = "Code Sent\nPlease check the code that we've just sent.";
        [field: SerializeField] public Color32 StatusResendCodeColor { get; private set; }
        #endregion



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