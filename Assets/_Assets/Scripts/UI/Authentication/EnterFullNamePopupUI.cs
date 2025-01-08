using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Authentication;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.Authentication
{
    public class EnterFullNamePopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Signup Popup UI Stuffs")]
        [SerializeField] private TMP_InputField _firstNameInputField;
        [SerializeField] private TMP_InputField _lastNameInputField;
        [Space]
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private string _firstName;
        private string _lastName;
        
        private EnterFullNamePopup _playerEnterFullNamePopup;
        private InputFieldValidator _inputFieldValidator;
        private InputFieldStatus _firstNameInputFieldStatus;
        private InputFieldStatus _lastNameInputFieldStatus;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerEnterFullNamePopup = GameObject.FindWithTag("Player").GetComponentInChildren<EnterFullNamePopup>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _firstNameInputFieldStatus = _firstNameInputField.GetComponent<InputFieldStatus>();
            _lastNameInputFieldStatus = _lastNameInputField.GetComponent<InputFieldStatus>();

            _firstNameInputField.onEndEdit.AddListener(inputText => IsFirstNameValidated());
            _lastNameInputField.onEndEdit.AddListener(inputText => IsLastNameValidated());

            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool Validate()
        {
            bool status = true;

            if (!IsFirstNameValidated()) status = false;
            if (!IsLastNameValidated()) status = false;

            return status;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private bool IsFirstNameValidated() => _inputFieldValidator.ValidateFirstName(
            _firstNameInputField.text, _firstNameInputFieldStatus, out _firstName,
            _playerEnterFullNamePopup.MinimumFirstNameLength,
            (string.Empty, default),
            (_playerEnterFullNamePopup.StatusFirstNameTooShort.GetLocalizedString(_playerEnterFullNamePopup.MinimumFirstNameLength), _playerEnterFullNamePopup.StatusFirstNameTooShortColor));

        private bool IsLastNameValidated() => _inputFieldValidator.ValidateLastName(
            _lastNameInputField.text, _lastNameInputFieldStatus, out _lastName,
            _playerEnterFullNamePopup.MinimumLastNameLength,
            (string.Empty, default),
            (_playerEnterFullNamePopup.StatusLastNameTooShort.GetLocalizedString(_playerEnterFullNamePopup.MinimumLastNameLength), _playerEnterFullNamePopup.StatusLastNameTooShortColor));

        private void Confirm()
        {
            if (Validate())
            {
                _playerEnterFullNamePopup.OnEnterSucceeded(_firstName, _lastName);
            }
            else
            {
                _playerEnterFullNamePopup.OnValidateFailed();
            }
        }
        #endregion
    }
}