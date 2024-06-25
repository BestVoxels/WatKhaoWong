using System;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.UI;
using Firebase.Auth;

namespace WatKhaoWong.SharePopup
{
    public class LoginPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Login Popup Status Text")]
        [SerializeField] private string _statusSucceeded = "Logged in successfully";
        [SerializeField] private Color32 _statusSucceededColor;
        [Space]
        [SerializeField] private string _statusErrored = "Logged in failed.";
        [SerializeField] private Color32 _statusErroredColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Login Popup General Settings")]
        [field: SerializeField] public byte MinimumPhoneNumberLength { get; private set; } = 9;
        [field: SerializeField] public byte MaximumPhoneNumberLength { get; private set; } = 15;
        [field: Space]
        [field: Header("Login Popup Status Text")]
        [field: SerializeField] public string StatusInvalidPhoneNumber { get; private set; } = "Invalid Phone Number.";
        [field: SerializeField] public Color32 StatusInvalidPhoneNumberColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusPhoneNumberTooShort { get; private set; } = "Invalid Phone Number (too Short).";
        [field: SerializeField] public Color32 StatusPhoneNumberTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusPhoneNumberTooLong { get; private set; } = "Invalid Phone Number (too Long).";
        [field: SerializeField] public Color32 StatusPhoneNumberTooLongColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusInvalidEmail { get; private set; } = "Invalid Email.";
        [field: SerializeField] public Color32 StatusInvalidEmailColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusInvalidPassword { get; private set; } = "Invalid Account or Password.";
        [field: SerializeField] public Color32 StatusInvalidPasswordColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusForgotPassword { get; private set; } = "Please contact for support at developer website.";
        [field: SerializeField] public Color32 StatusForgotPasswordColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusWrongFormat { get; private set; } = "Wrong Format!";
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
            Debug.LogWarning("Click \"Signup Text\" UI!");

            _onSignupTextClick?.Invoke();
        }

        public void OnForgotTextClick()
        {
            Debug.LogWarning("Click \"Forgot Text\" UI!");

            _onForgotTextClick?.Invoke();
        }

        public void OnValidateSucceeded(EAuthType authType, string phoneNumber, string email, string password)
        {
            Debug.LogWarning("Validate Texts Succeeded");


            if (_isRunningOnBackground) return;

            if (authType == EAuthType.PhoneNumber)
            {
                _verifyPopup.SendNewCode(phoneNumber, _onLoginSucceeded, _onLoginFailed, _statusErrored, _statusErroredColor, _statusSucceeded, _statusSucceededColor);
            }
            else if (authType == EAuthType.EmailPassword)
            {
                LoginAsyncWithEmailAndPassword(email, password);
            }
        }

        public void OnValidateFailed()
        {
            Debug.LogWarning("Validate Texts Failed");

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
                _statusText.Show($"{_statusErrored} Error Code ({e.ErrorCode})\n{e.Message}", _statusErroredColor);

                _onLoginFailed?.Invoke(e);
                _isRunningOnBackground = false;
            }

            Debug.Log($"Successfully Logged in user {result.User.Email}");
            _statusText.Show(_statusSucceeded, _statusSucceededColor);

            _onLoginSucceeded?.Invoke(result.User);
            _isRunningOnBackground = false;
        }
        #endregion
    }
}