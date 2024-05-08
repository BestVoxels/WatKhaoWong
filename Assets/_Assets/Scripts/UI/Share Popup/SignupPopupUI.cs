using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WatKhaoWong.SharePopup;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.UI.InputFields;

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
        private InputFieldValidator _inputFieldValidator;
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
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _firstNameInputFieldStatus = _firstNameInputField.GetComponent<InputFieldStatus>();
            _lastNameInputFieldStatus = _lastNameInputField.GetComponent<InputFieldStatus>();
            _userNameInputFieldStatus = _userNameInputField.GetComponent<InputFieldStatus>();
            _passwordInputFieldStatus = _passwordInputField.GetComponent<InputFieldStatus>();
            _confirmPasswordInputFieldStatus = _confirmPasswordInputField.GetComponent<InputFieldStatus>();

            _closeButton.onClick.AddListener(Close);

            _firstNameInputField.onEndEdit.AddListener(inputText => IsFirstNameValidated());
            _lastNameInputField.onEndEdit.AddListener(inputText => IsLastNameValidated());
            _userNameInputField.onEndEdit.AddListener(inputText => IsUserNameValidated());
            _passwordInputField.onEndEdit.AddListener(inputText => IsPasswordValidated());
            _confirmPasswordInputField.onEndEdit.AddListener(inputText => IsConfirmPasswordValidated());

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

            if (!IsFirstNameValidated()) status = false;
            if (!IsLastNameValidated()) status = false;
            if (!IsUserNameValidated()) status = false;
            if (!IsPasswordValidated()) status = false;
            if (!IsConfirmPasswordValidated()) status = false;

            return status;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerSignupPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private bool IsFirstNameValidated() => _inputFieldValidator.ValidateFirstName(
            _firstNameInputField.text, _firstNameInputFieldStatus, out _firstName,
            _playerSignupPopup.MinimumFirstNameLength,
            (string.Empty, default),
            (_playerSignupPopup.StatusFirstNameTooShort, _playerSignupPopup.StatusFirstNameTooShortColor));

        private bool IsLastNameValidated() => _inputFieldValidator.ValidateLastName(
            _lastNameInputField.text, _lastNameInputFieldStatus, out _lastName,
            _playerSignupPopup.MinimumLastNameLength,
            (string.Empty, default),
            (_playerSignupPopup.StatusLastNameTooShort, _playerSignupPopup.StatusLastNameTooShortColor));

        private bool IsUserNameValidated() => _inputFieldValidator.ValidateUserName(
            _userNameInputField.text, _userNameInputFieldStatus, out _userName,
            (string.Empty, default));

        private bool IsPasswordValidated() => _inputFieldValidator.ValidatePassword(
            _passwordInputField.text, _passwordInputFieldStatus, out _password,
            _playerSignupPopup.MinimumPasswordLength,
            (string.Empty, default),
            (_playerSignupPopup.StatusPasswordTooShort, _playerSignupPopup.StatusPasswordTooShortColor));

        private bool IsConfirmPasswordValidated() => _inputFieldValidator.ValidateConfirmPassword(
            _confirmPasswordInputField.text, _confirmPasswordInputFieldStatus, out _,
            _password,
            (string.Empty, default),
            (_playerSignupPopup.StatusConfirmPasswordNotMatch, _playerSignupPopup.StatusConfirmPasswordNotMatchColor));

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