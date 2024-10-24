using System;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Authentication
{
    public class SignupPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Signup Popup Status Text")]
        [SerializeField] private LocalizedString _statusSucceeded;
        [SerializeField] private Color32 _statusSucceededColor;
        [Space]
        [SerializeField] private LocalizedString _statusErrored;
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
        [field: SerializeField] public LocalizedString StatusFirstNameTooShort { get; private set; }
        [field: SerializeField] public Color32 StatusFirstNameTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusLastNameTooShort { get; private set; }
        [field: SerializeField] public Color32 StatusLastNameTooShortColor { get; private set; }
        [field: Space]
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
        [field: SerializeField] public LocalizedString StatusPasswordTooShort { get; private set; }
        [field: SerializeField] public Color32 StatusPasswordTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusConfirmPasswordNotMatch { get; private set; }
        [field: SerializeField] public Color32 StatusConfirmPasswordNotMatchColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusWrongFormat { get; private set; }
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

        private VerifyPopup _verifyPopup;
        private MyUserData _myUserData;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _verifyPopup = player.GetComponentInChildren<VerifyPopup>();
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _statusText = FindAnyObjectByType<StatusText>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnInformTextClick()
        {
            _onInformTextClick?.Invoke();
        }

        public void OnLoginTextClick()
        {
            _onLoginTextClick?.Invoke();
        }

        public void OnValidateSucceeded(EAuthType authType, string firstName, string lastName, string phoneNumber, string email, string password)
        {
            // Can't just call _savingWrapper.SaveWithoutAuth() without Subscribe to _onSignupSucceeded BECAUSE have to wait for 'CurrentUser.UserId' otherwise can't get Path to save.
            _onSignupSucceeded.AddListener((FirebaseUser user) =>
            {
                _myUserData.SetFirstName(firstName);
                _myUserData.SetLastName(lastName);
                _myUserData.SetMemberSinceText(DateTime.Now);
            });

            if (_isRunningOnBackground) return;

            if (authType == EAuthType.PhoneNumber)
            {
                _verifyPopup.SendNewCode(phoneNumber, _onSignupSucceeded, _onSignupFailed, _statusErrored.GetLocalizedString(), _statusErroredColor, _statusSucceeded.GetLocalizedString(), _statusSucceededColor);
            }
            else if (authType == EAuthType.EmailPassword)
            {
                SignupAsyncWithEmailAndPassword(email, password);
            }
        }

        public void OnValidateFailed()
        {
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
                _statusText.Show($"{_statusErrored.GetLocalizedString()} Error Code ({e.ErrorCode})\n{e.Message}", _statusErroredColor);

                _onSignupFailed?.Invoke(e);
                _isRunningOnBackground = false;
            }

            _statusText.Show(_statusSucceeded.GetLocalizedString(), _statusSucceededColor);

            _onSignupSucceeded?.Invoke(result.User);
            _myUserData.SetRole(EUserRole.Member);

            _isRunningOnBackground = false;
        }
        #endregion
    }
}