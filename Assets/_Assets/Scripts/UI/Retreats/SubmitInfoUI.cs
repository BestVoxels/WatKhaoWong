using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.Retreats
{
    public class SubmitInfoUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        [Header("SubmitInfo UI Stuffs")]
        [SerializeField] private TMP_InputField _phoneNumberInputField;
        [SerializeField] private TMP_InputField _medicalInputField;
        [Space]
        [SerializeField] private TMP_InputField _urgentPhoneNumberInputField;
        [SerializeField] private TMP_InputField _urgentPhoneRelateInputField;
        [Space]
        [SerializeField] private TMP_InputField _lineInputField;
        [SerializeField] private TMP_InputField _fbInputField;
        [SerializeField] private TMP_InputField _igInputField;
        [SerializeField] private TMP_InputField _tiktokInputField;
        [Space]
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private string _phoneNumber;
        private string _medical;
        private string _urgentPhoneNumber;
        private string _relation;
        private string _line, _fb, _ig, _tiktok;

        private SubmitInfo _submitInfo;
        private InputFieldValidator _inputFieldValidator;
        private InputFieldStatus _phoneNumberInputFieldStatus;
        private InputFieldStatus _medicalInputFieldStatus;
        private InputFieldStatus _urgentPhoneNumberInputFieldStatus;
        private InputFieldStatus _urgentPhoneRelateInputFieldStatus;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _submitInfo = GameObject.FindWithTag("Player").GetComponentInChildren<SubmitInfo>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _phoneNumberInputFieldStatus = _phoneNumberInputField.GetComponent<InputFieldStatus>();
            _medicalInputFieldStatus = _medicalInputField.GetComponent<InputFieldStatus>();
            _urgentPhoneNumberInputFieldStatus = _urgentPhoneNumberInputField.GetComponent<InputFieldStatus>();
            _urgentPhoneRelateInputFieldStatus = _urgentPhoneRelateInputField.GetComponent<InputFieldStatus>();


            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            _phoneNumberInputField.onEndEdit.AddListener(inputText => IsPhoneNumberValidated());
            _medicalInputField.onEndEdit.AddListener(inputText => IsMedicalValidated());
            _urgentPhoneNumberInputField.onEndEdit.AddListener(inputText => IsUrgentPhoneNumberValidated());
            _urgentPhoneRelateInputField.onEndEdit.AddListener(inputText => IsRelationValidated());
            _lineInputField.onEndEdit.AddListener(inputText => _line = inputText);
            _fbInputField.onEndEdit.AddListener(inputText => _fb = inputText);
            _igInputField.onEndEdit.AddListener(inputText => _ig = inputText);
            _tiktokInputField.onEndEdit.AddListener(inputText => _tiktok = inputText);

            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool Validate()
        {
            bool status = true;

            if (!IsPhoneNumberValidated()) status = false;
            if (!IsMedicalValidated()) status = false;
            if (!IsUrgentPhoneNumberValidated()) status = false;
            if (!IsRelationValidated()) status = false;

            return status;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _submitInfo.OnBackButtonClick();
        private void ChangeLang() => _submitInfo.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private bool IsPhoneNumberValidated() => _inputFieldValidator.ValidateSignupPhoneNumber(
            _phoneNumberInputField.text, _phoneNumberInputFieldStatus, out _phoneNumber,
            _submitInfo.MinimumPhoneNumberLength, _submitInfo.MaximumPhoneNumberLength,
            (string.Empty, default),
            (_submitInfo.StatusInvalidPhoneNumber.GetLocalizedString(), _submitInfo.StatusInvalidPhoneNumberColor),
            (_submitInfo.StatusPhoneNumberTooShort.GetLocalizedString(), _submitInfo.StatusPhoneNumberTooShortColor),
            (_submitInfo.StatusPhoneNumberTooLong.GetLocalizedString(), _submitInfo.StatusPhoneNumberTooLongColor));

        private bool IsUrgentPhoneNumberValidated() => _inputFieldValidator.ValidateSignupPhoneNumber(
            _urgentPhoneNumberInputField.text, _urgentPhoneNumberInputFieldStatus, out _urgentPhoneNumber,
            _submitInfo.MinimumPhoneNumberLength, _submitInfo.MaximumPhoneNumberLength,
            (string.Empty, default),
            (_submitInfo.StatusInvalidPhoneNumber.GetLocalizedString(), _submitInfo.StatusInvalidPhoneNumberColor),
            (_submitInfo.StatusPhoneNumberTooShort.GetLocalizedString(), _submitInfo.StatusPhoneNumberTooShortColor),
            (_submitInfo.StatusPhoneNumberTooLong.GetLocalizedString(), _submitInfo.StatusPhoneNumberTooLongColor));

        private bool IsMedicalValidated() => _inputFieldValidator.ValidateNotNull(
            _medicalInputField.text, _medicalInputFieldStatus, out _medical,
            (_submitInfo.StatusMustBeFilled.GetLocalizedString(), _submitInfo.StatusMustBeFilledColor));

        private bool IsRelationValidated() => _inputFieldValidator.ValidateNotNull(
            _urgentPhoneRelateInputField.text, _urgentPhoneRelateInputFieldStatus, out _relation,
            (_submitInfo.StatusMustBeFilled.GetLocalizedString(), _submitInfo.StatusMustBeFilledColor));

        private void Confirm()
        {
            if (Validate())
            {
                _submitInfo.OnValidateSucceeded(_phoneNumber, _medical, _urgentPhoneNumber, _relation, _line, _fb, _ig, _tiktok);
            }
            else
            {
                _submitInfo.OnValidateFailed();
            }
        }
        #endregion
    }
}