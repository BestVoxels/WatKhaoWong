using System.Collections;
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
        [SerializeField] private string StatusCanceled = "Canceled";
        [SerializeField] private Color32 StatusCanceledColor;
        [Space]
        [SerializeField] private string StatusErrored = "Logged in failed. Error.";
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
        private Coroutine _previousCoroutine;

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

            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(LoginCoroutine(userName, password));
        }

        public void OnValidateFailed()
        {
            Debug.LogWarning("Validate Texts Failed");

            _onValidateFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator LoginCoroutine(string userName, string password)
        {
            var auth = FirebaseAuth.DefaultInstance;
            var registerTask = auth.SignInWithEmailAndPasswordAsync(userName, password);
            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.IsCanceled)
            {
                Debug.Log("Registration was canceled.");
                _statusText.Show(StatusCanceled, StatusCanceledColor);
            }
            else if (registerTask.IsFaulted)
            {
                Debug.LogError($"Registration encountered an error: {registerTask.Exception}");
                _statusText.Show(StatusErrored, StatusErroredColor);

                _onLoginFailed?.Invoke(registerTask.Exception);
            }
            else if (registerTask.IsCompletedSuccessfully)
            {
                Debug.Log($"Successfully registered user {registerTask.Result.User.Email}");
                _statusText.Show(StatusSucceeded, StatusSucceededColor);

                _onLoginSucceeded?.Invoke(auth.CurrentUser);
            }

            _previousCoroutine = null;
            yield break;
        }
        #endregion
    }
}