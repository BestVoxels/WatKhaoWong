using System;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.UI;
using Firebase.Auth;
using UnityEngine.Localization;

namespace WatKhaoWong.Authentication
{
    public class LoginPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Login Popup Status Text")]
        [SerializeField] private LocalizedString _statusSucceeded;
        [SerializeField] private Color32 _statusSucceededColor;
        [Space]
        [SerializeField] private LocalizedString _statusErrored;
        [SerializeField] private Color32 _statusErroredColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Login Popup General Settings")]
        [field: SerializeField] public byte MinimumPhoneNumberLength { get; private set; } = 9;
        [field: SerializeField] public byte MaximumPhoneNumberLength { get; private set; } = 15;
        [field: Space]
        [field: Header("Login Popup Status Text")]
        [field: SerializeField] public LocalizedString StatusInvalidPhoneNumber { get; private set; }
        [field: SerializeField] public Color32 StatusInvalidPhoneNumberColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusPhoneNumberTooShort { get; private set; }
        [field: SerializeField] public Color32 StatusPhoneNumberTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusPhoneNumberTooLong { get; private set; }
        [field: SerializeField] public Color32 StatusPhoneNumberTooLongColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusInvalidEmail { get; private set; }
        [field: SerializeField] public Color32 StatusInvalidEmailColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusInvalidPassword { get; private set; }
        [field: SerializeField] public Color32 StatusInvalidPasswordColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusForgotPassword { get; private set; }
        [field: SerializeField] public Color32 StatusForgotPasswordColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusWrongFormat { get; private set; }
        [field: SerializeField] public Color32 StatusWrongFormatColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Login Popup UI Event")]
        [SerializeField] private UnityEvent _onSignupTextClick;
        [SerializeField] private UnityEvent _onForgotTextClick;
        [Space]
        [SerializeField] private UnityEvent<FirebaseUser> _onLoginSucceeded;
        [SerializeField] private UnityEvent<Exception> _onLoginFailed;
        [SerializeField] private UnityEvent _onValidateTextFailed;
        #endregion



        #region --Fields-- (In Class)
        private bool _isRunningOnBackground = false;

        private StatusText _statusText;
        private VerifyPopup _verifyPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _statusText = FindAnyObjectByType<StatusText>();
            _verifyPopup = GameObject.FindWithTag("Player").GetComponentInChildren<VerifyPopup>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnSignupTextClick()
        {
            _onSignupTextClick?.Invoke();
        }

        public void OnForgotTextClick()
        {
            _onForgotTextClick?.Invoke();
        }

        public void OnValidateSucceeded(EAuthType authType, string phoneNumber, string email, string password)
        {
            if (_isRunningOnBackground) return;

            if (authType == EAuthType.PhoneNumber)
            {
                _verifyPopup.SendNewCode(phoneNumber, _onLoginSucceeded, _onLoginFailed, _statusErrored.GetLocalizedString(), _statusErroredColor, _statusSucceeded.GetLocalizedString(), _statusSucceededColor);
            }
            else if (authType == EAuthType.EmailPassword)
            {
                LoginAsyncWithEmailAndPassword(email, password);
            }
        }

        public void OnValidateFailed()
        {
            _onValidateTextFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async void LoginAsyncWithEmailAndPassword(string email, string password)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            AuthResult result = null;
            try
            {
                _isRunningOnBackground = true;
                result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (Firebase.FirebaseException e)
            {
                Debug.LogError($"Login encountered an error: ({e.ErrorCode})\n{e.Message}");
                _statusText.Show($"{_statusErrored.GetLocalizedString()} Error Code ({e.ErrorCode})\n{e.Message}", _statusErroredColor);

                _onLoginFailed?.Invoke(e);
                _isRunningOnBackground = false;
            }

            _statusText.Show(_statusSucceeded.GetLocalizedString(), _statusSucceededColor);

            _onLoginSucceeded?.Invoke(result.User);
            // No need to assign Role to user because 'HandleStateChanged' will be triggered and Load Role back

            _isRunningOnBackground = false;
        }
        #endregion
    }
}