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
        [SerializeField] private string StatusSucceeded = "Logged in successfully";
        [SerializeField] private Color32 StatusSucceededColor;
        [Space]
        [SerializeField] private string StatusErrored = "Logged in failed.";
        [SerializeField] private Color32 StatusErroredColor;
        #endregion



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
        [SerializeField] private UnityEvent<FirebaseUser> _onLoginSucceeded;
        [SerializeField] private UnityEvent<Exception> _onLoginFailed;
        [SerializeField] private UnityEvent _onValidateFailed;
        #endregion



        #region --Fields-- (In Class)
        private bool _isRunningOnBackground = false;

        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _statusText = FindAnyObjectByType<StatusText>();
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

        public void OnValidateSucceeded(string userName, string password)
        {
            Debug.LogWarning("Validate Texts Succeeded");

            if (!_isRunningOnBackground)
                LoginAsync(userName, password);
        }

        public void OnValidateFailed()
        {
            Debug.LogWarning("Validate Texts Failed");

            _onValidateFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async void LoginAsync(string userName, string password)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            AuthResult result = null;
            try
            {
                _isRunningOnBackground = true;
                result = await auth.SignInWithEmailAndPasswordAsync(userName, password);
            }
            catch (Firebase.FirebaseException e)
            {
                Debug.LogError($"Login encountered an error: ({e.ErrorCode})\n{e.Message}");
                _statusText.Show($"{StatusErrored} Error Code ({e.ErrorCode})\n{e.Message}", StatusErroredColor);

                _onLoginFailed?.Invoke(e);
                _isRunningOnBackground = false;
            }

            Debug.Log($"Successfully Logged in user {result.User.Email}");
            _statusText.Show(StatusSucceeded, StatusSucceededColor);

            _onLoginSucceeded?.Invoke(result.User);
            _isRunningOnBackground = false;
        }
        #endregion
    }
}