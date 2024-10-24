using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WatKhaoWong.Authentication;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.Authentication
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

        [Header("Show Hide Password UI Stuffs")]
        [SerializeField] private RectTransform _popupRectTransform;
        [SerializeField] private float _shrinkPopupRectHeight;
        #endregion



        #region --Fields-- (In Class)
        private EAuthType _authType;

        private string _firstName;
        private string _lastName;
        private string _phoneNumber;
        private string _email;
        private string _password;

        private Vector2 _defaultPopupSizeDelta;

        private SignupPopup _playerSignupPopup;
        private StatusText _statusText;
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
            _statusText = FindAnyObjectByType<StatusText>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _firstNameInputFieldStatus = _firstNameInputField.GetComponent<InputFieldStatus>();
            _lastNameInputFieldStatus = _lastNameInputField.GetComponent<InputFieldStatus>();
            _userNameInputFieldStatus = _userNameInputField.GetComponent<InputFieldStatus>();
            _passwordInputFieldStatus = _passwordInputField.GetComponent<InputFieldStatus>();
            _confirmPasswordInputFieldStatus = _confirmPasswordInputField.GetComponent<InputFieldStatus>();

            BindUIWithFunction();
        }

        private void Start()
        {
            _defaultPopupSizeDelta = _popupRectTransform.sizeDelta;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void BindUIWithFunction()
        {
            _closeButton.onClick.AddListener(Close);

            _firstNameInputField.onEndEdit.AddListener(inputText => IsFirstNameValidated());
            _lastNameInputField.onEndEdit.AddListener(inputText => IsLastNameValidated());
            _userNameInputField.onEndEdit.AddListener(inputText => _inputFieldValidator.CheckAuthTypeCallback(inputText, authType =>
            {
                switch (authType)
                {
                    case EAuthType.PhoneNumber:
                        IsPhoneNumberValidated();

                        ShowPasswordUI(false);
                        ShrinkPopupHeight();
                        ClearPasswordInputText();
                        break;

                    case EAuthType.EmailPassword:
                        IsEmailValidated();

                        ShowPasswordUI(true);
                        ResetorePopupHeight();
                        break;

                    case EAuthType.Unknown:
                        _statusText.Show(_playerSignupPopup.StatusWrongFormat.GetLocalizedString(), _playerSignupPopup.StatusWrongFormatColor);
                        _userNameInputFieldStatus.SetError();

                        ShowPasswordUI(true);
                        ResetorePopupHeight();
                        break;
                }
            }));
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

        private bool Validate()
        {
            bool status = true;

            if (!IsFirstNameValidated()) status = false;
            if (!IsLastNameValidated()) status = false;

            _inputFieldValidator.CheckAuthTypeCallback(_userNameInputField.text, authType =>
            {
                switch (authType)
                {
                    case EAuthType.PhoneNumber:
                        if (!IsPhoneNumberValidated()) status = false;
                        break;
                    case EAuthType.EmailPassword:
                        if (!IsEmailValidated()) status = false;
                        if (!IsPasswordValidated()) status = false;
                        if (!IsConfirmPasswordValidated()) status = false;
                        break;
                    case EAuthType.Unknown:
                        status = false;
                        break;
                }

                _authType = authType;
            });

            return status;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Password UI~
        private void ShowPasswordUI(bool showStatus)
        {
            _passwordInputField.gameObject.SetActive(showStatus);
            _confirmPasswordInputField.gameObject.SetActive(showStatus);
        }

        private void ClearPasswordInputText()
        {
            _passwordInputField.text = string.Empty;
            _passwordInputFieldStatus.SetNormal();

            _confirmPasswordInputField.text = string.Empty;
            _confirmPasswordInputFieldStatus.SetNormal();
        }

        private void ResetorePopupHeight()
        {
            _popupRectTransform.sizeDelta = _defaultPopupSizeDelta;
        }

        private void ShrinkPopupHeight()
        {
            _popupRectTransform.sizeDelta = new Vector2(_defaultPopupSizeDelta.x, _shrinkPopupRectHeight);
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
            (_playerSignupPopup.StatusFirstNameTooShort.GetLocalizedString(_playerSignupPopup.MinimumFirstNameLength), _playerSignupPopup.StatusFirstNameTooShortColor));

        private bool IsLastNameValidated() => _inputFieldValidator.ValidateLastName(
            _lastNameInputField.text, _lastNameInputFieldStatus, out _lastName,
            _playerSignupPopup.MinimumLastNameLength,
            (string.Empty, default),
            (_playerSignupPopup.StatusLastNameTooShort.GetLocalizedString(_playerSignupPopup.MinimumLastNameLength), _playerSignupPopup.StatusLastNameTooShortColor));

        private bool IsPhoneNumberValidated() => _inputFieldValidator.ValidateSignupPhoneNumber(
            _userNameInputField.text, _userNameInputFieldStatus, out _phoneNumber,
            _playerSignupPopup.MinimumPhoneNumberLength, _playerSignupPopup.MaximumPhoneNumberLength,
            (string.Empty, default),
            (_playerSignupPopup.StatusInvalidPhoneNumber.GetLocalizedString(), _playerSignupPopup.StatusInvalidPhoneNumberColor),
            (_playerSignupPopup.StatusPhoneNumberTooShort.GetLocalizedString(), _playerSignupPopup.StatusPhoneNumberTooShortColor),
            (_playerSignupPopup.StatusPhoneNumberTooLong.GetLocalizedString(), _playerSignupPopup.StatusPhoneNumberTooLongColor));

        private bool IsEmailValidated() => _inputFieldValidator.ValidateSignupEmail(
            _userNameInputField.text, _userNameInputFieldStatus, out _email,
            (string.Empty, default),
            (_playerSignupPopup.StatusInvalidEmail.GetLocalizedString(), _playerSignupPopup.StatusInvalidEmailColor));

        private bool IsPasswordValidated() => _inputFieldValidator.ValidateSignupPassword(
            _passwordInputField.text, _passwordInputFieldStatus, out _password,
            _playerSignupPopup.MinimumPasswordLength,
            (string.Empty, default),
            (_playerSignupPopup.StatusPasswordTooShort.GetLocalizedString(_playerSignupPopup.MinimumPasswordLength), _playerSignupPopup.StatusPasswordTooShortColor));

        private bool IsConfirmPasswordValidated() => _inputFieldValidator.ValidateConfirmPassword(
            _confirmPasswordInputField.text, _confirmPasswordInputFieldStatus, out _,
            _password,
            (string.Empty, default),
            (_playerSignupPopup.StatusConfirmPasswordNotMatch.GetLocalizedString(), _playerSignupPopup.StatusConfirmPasswordNotMatchColor));

        private void InformText(PointerEventData data) => _playerSignupPopup.OnInformTextClick();

        private void LoginText(PointerEventData data) => _playerSignupPopup.OnLoginTextClick();

        private void Confirm()
        {
            if (Validate())
            {
                _playerSignupPopup.OnValidateSucceeded(_authType, _firstName, _lastName, _phoneNumber, _email, _password);
            }
            else
            {
                _playerSignupPopup.OnValidateFailed();
            }
        }
        #endregion
    }
}