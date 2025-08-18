using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.Retreats
{
    public class AccommodationFormUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        [Header("AccommodationForm UI Stuffs")]
        // TODO Status Texts
        //[SerializeField] private TMP_InputField _phoneNumberInputField;
        //[SerializeField] private TMP_InputField _medicalInputField;

        // Row Menu Section
        // TODO Dropdown
        [SerializeField] private Button _setTimeButton;
        // TODO Input Field
        // TODO switch

        //[Space]
        //[SerializeField] private TMP_InputField _urgentPhoneNumberInputField;
        //[SerializeField] private TMP_InputField _urgentPhoneRelateInputField;
        //[Space]
        //[SerializeField] private TMP_InputField _lineInputField;
        //[SerializeField] private TMP_InputField _fbInputField;
        //[SerializeField] private TMP_InputField _igInputField;
        //[SerializeField] private TMP_InputField _tiktokInputField;

        [Space]
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        //private string _phoneNumber;
        //private string _medical;
        //private string _urgentPhoneNumber;
        //private string _relation;
        //private string _line, _fb, _ig, _tiktok;

        private AccommodationForm _accommodationForm;
        //private InputFieldValidator _inputFieldValidator;
        //private InputFieldStatus _phoneNumberInputFieldStatus;
        //private InputFieldStatus _medicalInputFieldStatus;
        //private InputFieldStatus _urgentPhoneNumberInputFieldStatus;
        //private InputFieldStatus _urgentPhoneRelateInputFieldStatus;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _accommodationForm = GameObject.FindWithTag("Player").GetComponentInChildren<AccommodationForm>();
            //_inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            //_phoneNumberInputFieldStatus = _phoneNumberInputField.GetComponent<InputFieldStatus>();
            //_medicalInputFieldStatus = _medicalInputField.GetComponent<InputFieldStatus>();
            //_urgentPhoneNumberInputFieldStatus = _urgentPhoneNumberInputField.GetComponent<InputFieldStatus>();
            //_urgentPhoneRelateInputFieldStatus = _urgentPhoneRelateInputField.GetComponent<InputFieldStatus>();


            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            //_phoneNumberInputField.onEndEdit.AddListener(inputText => IsPhoneNumberValidated());
            //_medicalInputField.onEndEdit.AddListener(inputText => IsMedicalValidated());
            //_urgentPhoneNumberInputField.onEndEdit.AddListener(inputText => IsUrgentPhoneNumberValidated());
            //_urgentPhoneRelateInputField.onEndEdit.AddListener(inputText => IsRelationValidated());
            //_lineInputField.onEndEdit.AddListener(inputText => _line = inputText);
            //_fbInputField.onEndEdit.AddListener(inputText => _fb = inputText);
            //_igInputField.onEndEdit.AddListener(inputText => _ig = inputText);
            //_tiktokInputField.onEndEdit.AddListener(inputText => _tiktok = inputText);
            _setTimeButton.onClick.AddListener(SetTime);


            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        //#region --Methods-- (Custom PRIVATE)
        //private bool Validate()
        //{
        //    bool status = true;

        //    if (!IsPhoneNumberValidated()) status = false;
        //    if (!IsMedicalValidated()) status = false;
        //    if (!IsUrgentPhoneNumberValidated()) status = false;
        //    if (!IsRelationValidated()) status = false;

        //    return status;
        //}
        //#endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _accommodationForm.OnBackButtonClick();
        private void ChangeLang() => _accommodationForm.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void SetTime() => _accommodationForm.OnSetTimeButtonClick();

        //private bool IsPhoneNumberValidated() => _inputFieldValidator.ValidateSignupPhoneNumber(
        //    _phoneNumberInputField.text, _phoneNumberInputFieldStatus, out _phoneNumber,
        //    _accommodationForm.MinimumPhoneNumberLength, _accommodationForm.MaximumPhoneNumberLength,
        //    (string.Empty, default),
        //    (_accommodationForm.StatusInvalidPhoneNumber.GetLocalizedString(), _accommodationForm.StatusInvalidPhoneNumberColor),
        //    (_accommodationForm.StatusPhoneNumberTooShort.GetLocalizedString(), _accommodationForm.StatusPhoneNumberTooShortColor),
        //    (_accommodationForm.StatusPhoneNumberTooLong.GetLocalizedString(), _accommodationForm.StatusPhoneNumberTooLongColor));

        //private bool IsUrgentPhoneNumberValidated() => _inputFieldValidator.ValidateSignupPhoneNumber(
        //    _urgentPhoneNumberInputField.text, _urgentPhoneNumberInputFieldStatus, out _urgentPhoneNumber,
        //    _accommodationForm.MinimumPhoneNumberLength, _accommodationForm.MaximumPhoneNumberLength,
        //    (string.Empty, default),
        //    (_accommodationForm.StatusInvalidPhoneNumber.GetLocalizedString(), _accommodationForm.StatusInvalidPhoneNumberColor),
        //    (_accommodationForm.StatusPhoneNumberTooShort.GetLocalizedString(), _accommodationForm.StatusPhoneNumberTooShortColor),
        //    (_accommodationForm.StatusPhoneNumberTooLong.GetLocalizedString(), _accommodationForm.StatusPhoneNumberTooLongColor));

        //private bool IsMedicalValidated() => _inputFieldValidator.ValidateNotNull(
        //    _medicalInputField.text, _medicalInputFieldStatus, out _medical,
        //    (_accommodationForm.StatusMustBeFilled.GetLocalizedString(), _accommodationForm.StatusMustBeFilledColor));

        //private bool IsRelationValidated() => _inputFieldValidator.ValidateNotNull(
        //    _urgentPhoneRelateInputField.text, _urgentPhoneRelateInputFieldStatus, out _relation,
        //    (_accommodationForm.StatusMustBeFilled.GetLocalizedString(), _accommodationForm.StatusMustBeFilledColor));

        private void Confirm()
        {
            if (true) // Validate()
            {
                _accommodationForm.OnValidateSucceeded(default, default, default, default, default, default, default, default);
            }
            else
            {
                _accommodationForm.OnValidateFailed();
            }
        }
        #endregion
    }
}