using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WatKhaoWong.SharePopup;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.UI.System;

namespace WatKhaoWong.UI.SharePopup
{
    public class SignupPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Signup Popup UI Stuffs")]
        [SerializeField] private TMP_InputField _firstNameInputField;
        [SerializeField] private TMP_InputField _lastNameInputField;
        [SerializeField] private TMP_InputField _userNameInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private TMP_InputField _confirmPasswordInputField;
        [Space]
        [SerializeField] private EventTrigger _informTextEventTrigger;
        [SerializeField] private EventTrigger _loginTextEventTrigger;
        [Space]
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private string _firstName;
        private string _lastName;
        private string _userName;
        private string _password;

        private SignupPopup _playerSignupPopup;
        private StatusText _statusText;
        private InputFieldStatus _firstNameInputFieldStatus;
        private InputFieldStatus _lastNameInputFieldStatus;
        private InputFieldStatus _userNameInputFieldStatus;
        private InputFieldStatus _passwordInputFieldStatus;
        private InputFieldStatus _confirmPasswordInputFieldStatus;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerSignupPopup = GameObject.FindWithTag("Player").GetComponentInChildren<SignupPopup>();
            _statusText = FindAnyObjectByType<StatusText>();
            _firstNameInputFieldStatus = _firstNameInputField.GetComponent<InputFieldStatus>();
            _lastNameInputFieldStatus = _lastNameInputField.GetComponent<InputFieldStatus>();
            _userNameInputFieldStatus = _userNameInputField.GetComponent<InputFieldStatus>();
            _passwordInputFieldStatus = _passwordInputField.GetComponent<InputFieldStatus>();
            _confirmPasswordInputFieldStatus = _confirmPasswordInputField.GetComponent<InputFieldStatus>();

            _closeButton.onClick.AddListener(Close);

            _firstNameInputField.onEndEdit.AddListener(inputText => ValidateFirstNameInputField(inputText));
            _lastNameInputField.onEndEdit.AddListener(inputText => ValidateLastNameInputField(inputText));
            _userNameInputField.onEndEdit.AddListener(inputText => ValidateUserNameInputField(inputText));
            _passwordInputField.onEndEdit.AddListener(inputText => ValidatePasswordInputField(inputText));
            _confirmPasswordInputField.onEndEdit.AddListener(inputText => ValidateConfirmPasswordInputField(inputText));

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => InformText((PointerEventData)data));
            _informTextEventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => LoginText((PointerEventData)data));
            _loginTextEventTrigger.triggers.Add(entry);

            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool Validate()
        {
            bool status = true;

            if (!ValidateFirstNameInputField(_firstNameInputField.text)) status = false;
            if (!ValidateLastNameInputField(_lastNameInputField.text)) status = false;
            if (!ValidateUserNameInputField(_userNameInputField.text)) status = false;
            if (!ValidatePasswordInputField(_passwordInputField.text)) status = false;
            if (!ValidateConfirmPasswordInputField(_confirmPasswordInputField.text)) status = false;

            return status;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerSignupPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private bool ValidateFirstNameInputField(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                _firstNameInputFieldStatus.SetError();
                _firstName = string.Empty;
                return false;
            }
            // TODO CHECK Name is not relates to something bad or pornography
            // TODO CHECK Name is not too short
            // Facebook Example : 
            // 1. Usernames can only contain alphanumeric characters (A-Z, 0-9) and full stops ("."). They can't contain generic terms or domain extensions (e.g.,.com, net), including country extensions
            // 2. Usernames must be at least 5 characters long.
            // TODO maybe  COMBINE  ValidateFirstNameInputField() & ValidateLastNameInputField() into one method.

            _firstNameInputFieldStatus.SetNormal();
            _firstName = inputText;
            return true;
        }

        private bool ValidateLastNameInputField(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                _lastNameInputFieldStatus.SetError();
                _lastName = string.Empty;
                return false;
            }
            // TODO CHECK Name is not relates to something bad or pornography
            // TODO CHECK Name is not too short
            // Facebook Example : 
            // 1. Usernames can only contain alphanumeric characters (A-Z, 0-9) and full stops ("."). They can't contain generic terms or domain extensions (e.g.,.com, net), including country extensions
            // 2. Usernames must be at least 5 characters long.
            // TODO maybe  COMBINE  ValidateFirstNameInputField() & ValidateLastNameInputField() into one method.

            _lastNameInputFieldStatus.SetNormal();
            _lastName = inputText;
            return true;
        }

        private bool ValidateUserNameInputField(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                _userNameInputFieldStatus.SetError();
                _userName = string.Empty;
                return false;
            }
            // TODO CHECK IF Email or Phone Number is valid.
            // Facebook check if email Domain is Valid,
            // Example: wfek.com is valid website, WhateverNames@wfek.com is consider valid.
            // BUT asdfakljs.com is NOT valid website, WhateverNames@asdfakljs.com is NOT valid.

            _userNameInputFieldStatus.SetNormal();
            _userName = inputText;
            return true;
        }

        private bool ValidatePasswordInputField(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                _passwordInputFieldStatus.SetError();
                _password = string.Empty;
                return false;
            }
            // MinimumPassword Length
            else if (inputText.Length < _playerSignupPopup.MinimumPasswordLength)
            {
                _passwordInputFieldStatus.SetError();
                _password = string.Empty;
                _statusText.Show(_playerSignupPopup.StatusPasswordTooShort, _playerSignupPopup.StatusPasswordTooShortColor);
                return false;
            }
            // TODO TooEasy Password

            _passwordInputFieldStatus.SetNormal();
            _password = inputText;
            return true;
        }

        private bool ValidateConfirmPasswordInputField(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                _confirmPasswordInputFieldStatus.SetError();
                return false;
            }
            // PASSWORD MISMATCH
            else if (!_password.Equals(inputText))
            {
                _confirmPasswordInputFieldStatus.SetError();
                _statusText.Show(_playerSignupPopup.StatusConfirmPasswordNotMatch, _playerSignupPopup.StatusConfirmPasswordNotMatchColor);
                return false;
            }

            _confirmPasswordInputFieldStatus.SetNormal();
            return true;
        }

        private void InformText(PointerEventData data) => _playerSignupPopup.OnInformTextClick();

        private void LoginText(PointerEventData data) => _playerSignupPopup.OnLoginTextClick();

        private void Confirm()
        {
            if (Validate())
            {
                // TODO pass on these values below to other script or server maybe?
                //_firstName, _lastName, _userName, _password

                _playerSignupPopup.OnConfirmButtonClick();
            }
            else
            {
                _playerSignupPopup.OnConfirmButtonCantClick();
            }
        }
        #endregion
    }
}