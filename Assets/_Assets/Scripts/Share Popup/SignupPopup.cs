using System;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.SharePopup
{
    public class SignupPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Signup Popup Status Text")]
        [SerializeField] private string _statusSucceeded = "Signed up successfully";
        [SerializeField] private Color32 _statusSucceededColor;
        [Space]
        [SerializeField] private string _statusErrored = "Signed up failed.";
        [SerializeField] private Color32 _statusErroredColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Signup Popup General Settings")]
        [field: SerializeField] public byte MinimumFirstNameLength { get; private set; } = 5;
        [field: SerializeField] public byte MinimumLastNameLength { get; private set; } = 5;
        [field: SerializeField] public byte MinimumPasswordLength { get; private set; } = 6;
        [field: SerializeField] public byte MinimumPhoneNumberLength { get; private set; } = 9;
        [field: SerializeField] public byte MaximumPhoneNumberLength { get; private set; } = 15;
        [field: Space]
        [field: Header("Signup Popup Status Text")]
        [field: SerializeField] public string StatusFirstNameTooShort { get; private set; } = "Your first name must be at least 5 characters long. Please try another.";
        [field: SerializeField] public Color32 StatusFirstNameTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusLastNameTooShort { get; private set; } = "Your last name must be at least 5 characters long. Please try another.";
        [field: SerializeField] public Color32 StatusLastNameTooShortColor { get; private set; }
        [field: Space]
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
        [field: SerializeField] public string StatusPasswordTooShort { get; private set; } = "Your password must be at least 6 characters long. Please try another.";
        [field: SerializeField] public Color32 StatusPasswordTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusConfirmPasswordNotMatch { get; private set; } = "Confirm Password & Password must match!";
        [field: SerializeField] public Color32 StatusConfirmPasswordNotMatchColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusWrongFormat { get; private set; } = "Wrong Format!";
        [field: SerializeField] public Color32 StatusWrongFormatColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Signup Popup UI Event")]
        [SerializeField] private UnityEvent _onInformTextClick;
        [SerializeField] private UnityEvent _onLoginTextClick;
        [Space]
        [SerializeField] private UnityEvent<FirebaseUser> _onSignupSucceeded;
        [SerializeField] private UnityEvent<Exception> _onSignupFailed;
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

        public void OnValidateSucceeded(EAuthType authType, string firstName, string lastName, string phoneNumber, string email, string password)
        {
            Debug.LogWarning("Validate Texts Succeeded");

            // TODO : Upload to Account.cs - 'firstName', 'lastName'
            // ...

            if (_isRunningOnBackground) return;

            if (authType == EAuthType.PhoneNumber)
            {
                _verifyPopup.SendNewCode(phoneNumber, _onSignupSucceeded, _onSignupFailed, _statusErrored, _statusErroredColor, _statusSucceeded, _statusSucceededColor);
            }
            else if (authType == EAuthType.EmailPassword)
            {
                SignupAsyncWithEmailAndPassword(email, password);
            }
        }

        public void OnValidateFailed()
        {
            Debug.LogWarning("Validate Texts Failed");

            _onValidateTextFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async void SignupAsyncWithEmailAndPassword(string email, string password)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            AuthResult result = null;
            try
            {
                _isRunningOnBackground = true;
                result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            }
            catch (Firebase.FirebaseException e)
            {
                Debug.LogError($"Registration encountered an error: ({e.ErrorCode})\n{e.Message}");
                _statusText.Show($"{_statusErrored} Error Code ({e.ErrorCode})\n{e.Message}", _statusErroredColor);

                _onSignupFailed?.Invoke(e);
                _isRunningOnBackground = false;
            }

            Debug.Log($"Successfully registered user {result.User.Email}");
            _statusText.Show(_statusSucceeded, _statusSucceededColor);

            _onSignupSucceeded?.Invoke(result.User);
            _isRunningOnBackground = false;
        }
        #endregion
    }
}