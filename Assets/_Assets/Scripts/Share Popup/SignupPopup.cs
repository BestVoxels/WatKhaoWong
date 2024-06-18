using System;
using System.Collections;
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
        [SerializeField] private string StatusSucceeded = "Signed up successfully";
        [SerializeField] private Color32 StatusSucceededColor;
        [Space]
        [SerializeField] private string StatusCanceled = "Canceled";
        [SerializeField] private Color32 StatusCanceledColor;
        [Space]
        [SerializeField] private string StatusErrored = "Signed up failed. Error.";
        [SerializeField] private Color32 StatusErroredColor;
        #endregion



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
        [SerializeField] private UnityEvent<FirebaseUser> _onSignupSucceeded;
        [SerializeField] private UnityEvent<Exception> _onSignupFailed;
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

        public void OnValidateSucceeded(string firstName, string lastName, string userName, string password)
        {
            Debug.LogWarning("Validate Texts Succeeded");

            // TODO : Upload to Account.cs - 'firstName', 'lastName'
            // ...

            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(SignupCoroutine(userName, password));
        }

        public void OnValidateFailed()
        {
            Debug.LogWarning("Validate Texts Failed");

            _onValidateFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator SignupCoroutine(string userName, string password)
        {
            var auth = FirebaseAuth.DefaultInstance;
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(userName, password);
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

                _onSignupFailed?.Invoke(registerTask.Exception);
            }
            else if (registerTask.IsCompletedSuccessfully)
            {
                Debug.Log($"Successfully registered user {registerTask.Result.User.Email}");
                _statusText.Show(StatusSucceeded, StatusSucceededColor);

                _onSignupSucceeded?.Invoke(auth.CurrentUser);
            }

            _previousCoroutine = null;
            yield break;
        }
        #endregion
    }
}