using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WatKhaoWong.Authentication;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.Authentication
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

        [Header("Show Hide Password UI Stuffs")]
        [SerializeField] private RectTransform _popupRectTransform;
        [SerializeField] private float _shrinkPopupRectHeight;
        #endregion



        #region --Fields-- (In Class)
        private EAuthType _authType;

        private string _phoneNumber;
        private string _email;
        private string _password;

        private Vector2 _defaultPopupSizeDelta;

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
                        _statusText.Show(_playerLoginPopup.StatusWrongFormat, _playerLoginPopup.StatusWrongFormatColor);
                        _userNameInputFieldStatus.SetError();

                        ShowPasswordUI(true);
                        ResetorePopupHeight();
                        break;
                }
            }));
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

        private bool Validate()
        {
            bool status = true;

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
        }

        private void ClearPasswordInputText()
        {
            _passwordInputField.text = string.Empty;
            _passwordInputFieldStatus.SetNormal();
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
        private void Close() => _playerLoginPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private bool IsPhoneNumberValidated() => _inputFieldValidator.ValidateLoginPhoneNumber(
            _userNameInputField.text, _userNameInputFieldStatus, out _phoneNumber,
            _playerLoginPopup.MinimumPhoneNumberLength, _playerLoginPopup.MaximumPhoneNumberLength,
            (string.Empty, default),
            (_playerLoginPopup.StatusInvalidPhoneNumber, _playerLoginPopup.StatusInvalidPhoneNumberColor),
            (_playerLoginPopup.StatusPhoneNumberTooShort, _playerLoginPopup.StatusPhoneNumberTooShortColor),
            (_playerLoginPopup.StatusPhoneNumberTooLong, _playerLoginPopup.StatusPhoneNumberTooLongColor));

        private bool IsEmailValidated() => _inputFieldValidator.ValidateLoginEmail(
            _userNameInputField.text, _userNameInputFieldStatus, out _email,
            (string.Empty, default),
            (_playerLoginPopup.StatusInvalidEmail, _playerLoginPopup.StatusInvalidEmailColor));

        private bool IsPasswordValidated() => _inputFieldValidator.ValidateLoginPassword(
            _passwordInputField.text, _passwordInputFieldStatus, out _password,
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
                _playerLoginPopup.OnValidateSucceeded(_authType, _phoneNumber, _email, _password);
            }
            else
            {
                _playerLoginPopup.OnValidateFailed();
            }
        }
        #endregion
    }
}