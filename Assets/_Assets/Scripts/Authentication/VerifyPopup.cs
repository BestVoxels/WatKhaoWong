using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Identities;
using Firebase.Auth;
using System;

namespace WatKhaoWong.Authentication
{
    public class VerifyPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Verify Popup Status Text")]
        [SerializeField] private string _statusCodeConnecting = "Connecting...";
        [SerializeField] private Color32 _statusCodeConnectingColor;
        [Space]
        [SerializeField] private string _statusCodeSentSucceeded = "Code sent successfully.";
        [SerializeField] private Color32 _statusCodeSentSucceededColor;
        [Space]
        [SerializeField] private string _statusCodeSentFailed = "Failed to send the code. Please try again.";
        [SerializeField] private Color32 _statusCodeSentFailedColor;
        [Space]
        [SerializeField] private string _statusCodeAutoVerifyCompleted = "Auto verification completed successfully.";
        [SerializeField] private Color32 _statusCodeAutoVerifyCompletedColor;
        [Space]
        [SerializeField] private string _statusCodeAutoRetrievalTimeOut = "Auto-SMS retrieval timed out. Please enter the verification code manually.";
        [SerializeField] private Color32 _statusCodeAutoRetrievalTimeOutColor;
        [Space]
        [SerializeField] private string _statusResendCode = "Code Sent. Please check the code we've just sent.";
        [SerializeField] private Color32 _statusResendCodeColor;
        [Space]
        [SerializeField] private string _statusCantResendCode = "Cannot resend code. Please restart the app and try again.";
        [SerializeField] private Color32 _statusCantResendCodeColor;
        [Space]
        [SerializeField] private string _statusErrorNoSendCodeRequest = "Error: Please send a code request before verifying the OTP. Restart the app and try again.";
        [SerializeField] private Color32 _statusErrorNoSendCodeRequestColor;
        [Space]
        [Header("Phone Number Authentication Settings")]
        [Tooltip("This specifies the maximum amount of time that the system will wait for the SMS verification process to complete. Ex - 60000 = 60sec")]
        [SerializeField] private uint _timeoutInMilliseconds = 60000; // 60sec
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Verify Popup General Settings")]
        [field: SerializeField] public byte MinimumCodeLength { get; private set; } = 6;
        [field: Space]
        [field: Header("Verify Popup Status Text")]
        [field: SerializeField] public string StatusCodeTooShort { get; private set; } = "Your code must be at least 6 characters long. Please check the code that we've just sent.";
        [field: SerializeField] public Color32 StatusCodeTooShortColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Verify Popup UI Event")]
        [SerializeField] private UnityEvent _onInformTextClick;
        [SerializeField] private UnityEvent _onResendTextClick;
        [Space]
        [SerializeField] private UnityEvent _onOTPCodeSentSucceeded;
        [SerializeField] private UnityEvent<FirebaseUser> _onVerifySucceeded;
        [SerializeField] private UnityEvent<Exception> _onVerifyFailed;
        [SerializeField] private UnityEvent _onValidateTextFailed;
        #endregion



        #region --Fields-- (In Class)
        private string _phoneNumber;
        private ForceResendingToken _resendToken = null;

        private bool _isRunningOnBackground = false;
        private string _typedCode;

        private StatusText _statusText;
        private MyUserData _myUserData;
        #endregion



        #region --Fields-- (In Class) ~From Caller~
        private string _firebaseCode;
        private UnityEvent<FirebaseUser> _onCallerSucceeded;
        private UnityEvent<Exception> _onCallerFailed;
        private string _callerStatusErrored;
        private Color32 _callerStatusErroredColor;
        private string _callerStatusSucceeded;
        private Color32 _callerStatusSucceededColor;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _statusText = FindAnyObjectByType<StatusText>();
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnInformTextClick()
        {
            ResendCode();
            _onInformTextClick?.Invoke();
        }

        public void OnResendTextClick()
        {
            ResendCode();
            _onResendTextClick?.Invoke();
        }

        public void OnValidateSucceeded(string typedCode)
        {
            _typedCode = typedCode;

            // Guard check for _caller if they are null, before LETTING SignupOrLoginAsyncWithPhoneNumber() use
            if (string.IsNullOrWhiteSpace(_firebaseCode) || string.IsNullOrWhiteSpace(_phoneNumber) || _onCallerSucceeded == null)
            {
                Debug.LogError("Error: Please send a code request before verifying the OTP. Verify Popup is showed first without Firebase Code sent.");
                _statusText.Show(_statusErrorNoSendCodeRequest, _statusErrorNoSendCodeRequestColor);
                return;
            }
            if (_isRunningOnBackground) return;

            SignupOrLoginAsyncWithPhoneNumber(_firebaseCode, _typedCode);
        }

        public void OnValidateFailed()
        {
            _onValidateTextFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SendNewCode(string phoneNumber, UnityEvent<FirebaseUser> onCallerSucceeded, UnityEvent<Exception> onCallerFailed, string callerStatusErrored, Color32 callerStatusErroredColor, string callerStatusSucceeded, Color32 callerStatusSucceededColor)
        {
            _phoneNumber = phoneNumber;

            _onCallerSucceeded = onCallerSucceeded;
            _onCallerFailed = onCallerFailed;
            _callerStatusErrored = callerStatusErrored;
            _callerStatusErroredColor = callerStatusErroredColor;
            _callerStatusSucceeded = callerStatusSucceeded;
            _callerStatusSucceededColor = callerStatusSucceededColor;

            SendCode(_phoneNumber, null);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ResendCode()
        {
            if (_phoneNumber == string.Empty || _resendToken == null)
            {
                Debug.LogError($"Can't Resend Code : Phone Number value = {_phoneNumber} | Resend Token value = {_resendToken}");
                _statusText.Show(_statusCantResendCode, _statusCantResendCodeColor);
                return;
            }

            _statusText.Show(_statusResendCode, _statusResendCodeColor);
            SendCode(_phoneNumber, _resendToken);
        }

        /// <summary>
        /// Typical Flow:
        /// 1. The "VerifyPhoneNumber" method is called.
        /// 2. Firebase sends the verification code via SMS to the provided phone number.
        /// 3. The "codeSent" callback is invoked, indicating that the code has been sent.
        ///   4. If the automatic SMS retrieval is successful (on Android), the "verificationCompleted" callback will be triggered.
        ///   4. If the automatic SMS retrieval is unsuccessful and the timeout period elapses, the "codeAutoRetrievalTimeOut" callback is triggered.
        /// </summary>
        private void SendCode(string phoneNumber, ForceResendingToken resendToken)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;

            PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(auth);
            provider.VerifyPhoneNumber(
              new Firebase.Auth.PhoneAuthOptions
              {
                  PhoneNumber = phoneNumber,
                  TimeoutInMilliseconds = _timeoutInMilliseconds, // Timeout duration for the auto-sms-retrieval
                  ForceResendingToken = resendToken,
              },
              codeSent: (id, token) => {
                  // Verification code was successfully sent via SMS.
                  // `id` contains the verification id that will need to passed in with the code from the user when calling GetCredential().
                  // `token` can be used if the user requests the code be sent again, to tie the two requests together.
                  _statusText.Show(_statusCodeSentSucceeded, _statusCodeSentSucceededColor);

                  _resendToken = token;
                  _firebaseCode = id; // NO NEED to SAVE '_firebaseCode' (verification id) with PlayerPrefs - BECAUSE it is too cumbersome have to display Validation UI and check various condition to display properly.

                  _onOTPCodeSentSucceeded?.Invoke();
              },
              verificationFailed: (error) => {
                  // The verification code was not sent. `error` contains a human readable explanation of the problem.
                  Debug.LogError($"Registration encountered an error: {error}");
                  _statusText.Show($"{_statusCodeSentFailed}\n{error}", _statusCodeSentFailedColor);

                  _onCallerFailed?.Invoke(null);
              },
              verificationCompleted: (credential) => {
#if UNITY_ANDROID || UNITY_EDITOR
                  // Auto-sms-retrieval or instant validation has succeeded (Android only).
                  // There is no need to input the verification code.
                  // `credential` can be used instead of calling GetCredential().
                  SignupOrLoginAsyncWithPhoneNumber(credential);

                  _statusText.Show(_statusCodeAutoVerifyCompleted, _statusCodeAutoVerifyCompletedColor);
#endif
              },
              codeAutoRetrievalTimeOut: (id) => {
#if UNITY_ANDROID || UNITY_EDITOR
                  // Called when the auto-sms-retrieval has timed out, based on the given timeout parameter.
                  // This callback is used in the context of Firebase Phone Authentication on Android.
                  // This callback is triggered when the specified timeout period elapses and Firebase is unable to automatically retrieve the SMS verification code.
                  // `id` contains the verification id of the request that timed out.
                  _statusText.Show(_statusCodeAutoRetrievalTimeOut, _statusCodeAutoRetrievalTimeOutColor);
#endif
              });

            _statusText.Show(_statusCodeConnecting, _statusCodeConnectingColor);
            // TODO Open Processing UI to block Interaction
        }

        private void SignupOrLoginAsyncWithPhoneNumber(string firebaseCode, string typedCode)
        {
            PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(FirebaseAuth.DefaultInstance);
            PhoneAuthCredential credential = provider.GetCredential(firebaseCode, typedCode);

            SignupOrLoginAsyncWithPhoneNumber(credential);
        }

        private async void SignupOrLoginAsyncWithPhoneNumber(PhoneAuthCredential credential)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            AuthResult result = null;
            try
            {
                _isRunningOnBackground = true;
                result = await auth.SignInAndRetrieveDataWithCredentialAsync(credential);
            }
            catch (Firebase.FirebaseException e)
            {
                Debug.LogError($"Phone Authentication encountered an error: ({e.ErrorCode})\n{e.Message}");
                _statusText.Show($"{_callerStatusErrored} Error Code ({e.ErrorCode})\n{e.Message}", _callerStatusErroredColor);

                _onCallerFailed?.Invoke(e);
                _onVerifyFailed?.Invoke(e);

                _isRunningOnBackground = false;
            }

            _statusText.Show(_callerStatusSucceeded, _callerStatusSucceededColor);

            _onCallerSucceeded?.Invoke(result.User); // Pass back to the caller
            _onVerifySucceeded?.Invoke(result.User); // Call To Close Verfication Popup UI
            _myUserData.SetRole(EUserRole.Member);

            _isRunningOnBackground = false;
        }
        #endregion
    }
}