using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WatKhaoWong.SharePopup;
using WatKhaoWong.UI.System;
using WatKhaoWong.UI.InputFields;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.SharePopup
{
    public class LoginPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Login Popup UI Stuffs")]
        [SerializeField] private TMP_InputField _userNameInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [Space]
        [SerializeField] private EventTrigger _signupTextEventTrigger;
        [SerializeField] private EventTrigger _forgotTextEventTrigger;
        [Space]
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        // TODO Temp
        private string _userPassword = "thanitsakBoat";

        private LoginPopup _playerLoginPopup;
        private StatusText _statusText;
        private InputFieldValidator _inputFieldValidator;
        private InputFieldStatus _userNameInputFieldStatus;
        private InputFieldStatus _passwordInputFieldStatus;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerLoginPopup = GameObject.FindWithTag("Player").GetComponentInChildren<LoginPopup>();
            _statusText = FindAnyObjectByType<StatusText>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _userNameInputFieldStatus = _userNameInputField.GetComponentInChildren<InputFieldStatus>();
            _passwordInputFieldStatus = _passwordInputField.GetComponentInChildren<InputFieldStatus>();

            _closeButton.onClick.AddListener(Close);

            _userNameInputField.onEndEdit.AddListener(inputText => IsUserNameValidated());
            _passwordInputField.onEndEdit.AddListener(inputText => IsPasswordValidated());

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => SignupText((PointerEventData)data));
            _signupTextEventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => ForgotText((PointerEventData)data));
            _forgotTextEventTrigger.triggers.Add(entry);

            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool Validate()
        {
            bool status = true;

            if (!IsUserNameValidated()) status = false;
            if (!IsPasswordValidated()) status = false;

            return status;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerLoginPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private bool IsUserNameValidated() => _inputFieldValidator.ValidateLoginUserName(
            _userNameInputField.text, _userNameInputFieldStatus, out _,
            (string.Empty, default),
            (_playerLoginPopup.StatusInvalidUserName, _playerLoginPopup.StatusInvalidUserNameColor));

        private bool IsPasswordValidated() => _inputFieldValidator.ValidateLoginPassword(
            _passwordInputField.text, _passwordInputFieldStatus, out _,
            _userPassword,
            (string.Empty, default),
            (_playerLoginPopup.StatusInvalidPassword, _playerLoginPopup.StatusInvalidPasswordColor));

        private void SignupText(PointerEventData data) => _playerLoginPopup.OnSignupTextClick();

        private void ForgotText(PointerEventData data)
        {
            _statusText.Show(_playerLoginPopup.StatusForgotPassword, _playerLoginPopup.StatusForgotPasswordColor);

            _playerLoginPopup.OnForgotTextClick();
        }

        private void Confirm()
        {
            if (Validate())
            {
                // TODO do something later with server maybe?

                _playerLoginPopup.OnConfirmButtonClick();
            }
            else
            {
                _playerLoginPopup.OnConfirmButtonCantClick();
            }
        }
        #endregion
    }
}