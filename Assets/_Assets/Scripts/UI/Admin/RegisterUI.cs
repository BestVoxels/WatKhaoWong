using TMPro;
using UnityEngine.UI;
using UnityEngine;
using WatKhaoWong.Admin;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;
using Newtonsoft.Json.Linq;

namespace WatKhaoWong.UI.Admin
{
    public class RegisterUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        [Header("Register UI Stuffs - Panel 1")]
        [SerializeField] private GameObject _firstPanel;
        [Space]
        [SerializeField] private Button _addByReaderButton;
        [SerializeField] private Button _addManuallyButton;
        [Space]
        [Header("Register UI Stuffs - Panel 2 (For 'AddManuallyButton')")]
        [SerializeField] private GameObject _secondPanel;
        [Space]
        [SerializeField] private Button _useIdCardButton;
        [SerializeField] private Button _usePassportButton;
        [Space]
        [Header("Register UI Stuffs - Panel PreFillData")]
        [SerializeField] private GameObject _preFillPanel;
        [Space]
        [SerializeField] private GameObject _cardReaderIFPanel;
        [SerializeField] private TMP_InputField _cardReaderIF;
        [SerializeField] private InputFieldStatus _cardReaderIFS;
        [Space]
        [SerializeField] private GameObject _cardIdIFPanel;
        [SerializeField] private TMP_InputField _cardIdIF;
        [SerializeField] private InputFieldStatus _cardIdIFS;
        [Space]
        [SerializeField] private GameObject _passportIdIFPanel;
        [SerializeField] private TMP_InputField _passportIdIF;
        [SerializeField] private InputFieldStatus _passportIdIFS;
        [Space]
        [SerializeField] private Button _nextButton;
        [Space]
        [SerializeField] private GameObject _userIdIFPanel;
        [SerializeField] private TMP_InputField _userIdIF;
        [SerializeField] private InputFieldStatus _userIdIFS;
        [SerializeField] private GameObject _noUserIdButtonPanel;
        [SerializeField] private Button _noUserIdButton;
        [SerializeField] private GameObject _noUserRemarkPanel;
        [Space]
        [Header("Register UI Stuffs - Panel FillData")]
        [SerializeField] private GameObject _fillPanel;
        [Space]
        [Space]
        [SerializeField] private GameObject _nationalPanel;
        [Space]
        [SerializeField] private TMP_InputField _nationalIdIF;
        [SerializeField] private InputFieldStatus _nationalIdIFS;
        [Space]
        [SerializeField] private TMP_InputField _genderIF;
        [SerializeField] private InputFieldStatus _genderIFS;
        [Space]
        [SerializeField] private TMP_InputField _prefixIF;
        [SerializeField] private InputFieldStatus _prefixIFS;
        [Space]
        [SerializeField] private TMP_InputField _fNameIF;
        [SerializeField] private InputFieldStatus _fNameIFS;
        [Space]
        [SerializeField] private TMP_InputField _lNameIF;
        [SerializeField] private InputFieldStatus _lNameIFS;
        [Space]
        [SerializeField] private TMP_InputField _birthDateIF;
        [SerializeField] private InputFieldStatus _birthDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _issueDateIF;
        [SerializeField] private InputFieldStatus _issueDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _expireDateIF;
        [SerializeField] private InputFieldStatus _expireDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _houseNumberIF;
        [SerializeField] private InputFieldStatus _houseNumberIFS;
        [Space]
        [SerializeField] private TMP_InputField _subDistrictIF;
        [SerializeField] private InputFieldStatus _subDistrictIFS;
        [Space]
        [SerializeField] private TMP_InputField _districtIF;
        [SerializeField] private InputFieldStatus _districtIFS;
        [Space]
        [SerializeField] private TMP_InputField _provinceIF;
        [SerializeField] private InputFieldStatus _provinceIFS;
        [Space]
        [SerializeField] private TMP_InputField _countryIF;
        [SerializeField] private InputFieldStatus _countryIFS;
        [Space]
        [Space]
        // [Header("-Passport-")]
        [SerializeField] private GameObject _passportPanel;
        [Space]
        [SerializeField] private TMP_InputField _ppNumberIF;
        [SerializeField] private InputFieldStatus _ppNumberIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppNationalityIF;
        [SerializeField] private InputFieldStatus _ppNationalityIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppGenderIF;
        [SerializeField] private InputFieldStatus _ppGenderIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppFullNameIF;
        [SerializeField] private InputFieldStatus _ppFullNameIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppBirthDateIF;
        [SerializeField] private InputFieldStatus _ppBirthDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppIssueDateIF;
        [SerializeField] private InputFieldStatus _ppIssueDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppExpireDateIF;
        [SerializeField] private InputFieldStatus _ppExpireDateIFS;
        [Space]
        [Space]
        // [Header("-My Info-")]
        [SerializeField] private TMP_InputField _phoneNumberIF;
        [SerializeField] private InputFieldStatus _phoneNumberIFS;
        [Space]
        [SerializeField] private TMP_InputField _medicalIF;
        [SerializeField] private InputFieldStatus _medicalIFS;
        [Space]
        [Space]
        // [Header("-Emergency Contact-")]
        [SerializeField] private TMP_InputField _urgentPhoneNumberIF;
        [SerializeField] private InputFieldStatus _urgentPhoneNumberIFS;
        [Space]
        [SerializeField] private TMP_InputField _urgentPhoneRelateIF;
        [SerializeField] private InputFieldStatus _urgentPhoneRelateIFS;
        [Space]
        [Space]
        // [Header("-Social Media-")]
        [SerializeField] private TMP_InputField _lineIF;
        [Space]
        [SerializeField] private TMP_InputField _facebookIF;
        [Space]
        [SerializeField] private TMP_InputField _igIF;
        [Space]
        [SerializeField] private TMP_InputField _tiktokIF;
        [Space]
        [Space]
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        // For PreFill Panel
        private string _inputFieldText;
        private string _inputUserIdText;
        
        // For -Setter- ID Card
        private string _nationalId = null;
        private string _gender = null;
        private string _prefix = null;
        private string _fName = null;
        private string _lName = null;
        private string _birthDate = null;
        private string _issueDate = null;
        private string _expireDate = null;
        private string _houseNumber;
        private string _subDistrict;
        private string _district;
        private string _province;
        private string _country;

        // For -Setter- Passport
        private string _ppNumber = null;
        private string _ppNationality = null;
        private string _ppGender = null;
        private string _ppFullName = null;
        private string _ppBirthDate = null;
        private string _ppIssueDate = null;
        private string _ppExpireDate = null;

        // For -Setter- My Info
        private string _phoneNumber;
        private string _medical;

        // For -Setter- Emergency Contact
        private string _urgentPhoneNumber;
        private string _urgentPhoneRelate;

        // For -Setter- Social Media
        private string _line;
        private string _facebook;
        private string _ig;
        private string _tiktok;
        
        private Register _register;
        private StatusText _statusText;
        private InputFieldValidator _inputFieldValidator;
        #endregion



        #region --Fields-- (Constant)
        private const float WaitUIToTurnOffOnStartTime = 3.5f;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _register = GameObject.FindWithTag("Player").GetComponentInChildren<Register>();
            _statusText = FindAnyObjectByType<StatusText>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            // Panel 1 & Panel 2
            _addByReaderButton.onClick.AddListener(AddByReaderButton);
            _addManuallyButton.onClick.AddListener(AddManuallyButton);
            _useIdCardButton.onClick.AddListener(UseIdCardButton);
            _usePassportButton.onClick.AddListener(UsePassportButton);
            
            // PreFill Panel
            _cardReaderIF.onEndEdit.AddListener(inputText => IsCardReaderValidated());
            _cardIdIF.onEndEdit.AddListener(inputText => IsCardIdValidated());
            _passportIdIF.onEndEdit.AddListener(inputText => IsPassportValidated());
            _userIdIF.onEndEdit.AddListener(inputText => IsUserIdValidated());
            _nextButton.onClick.AddListener(NextButton);
            _noUserIdButton.onClick.AddListener(NoUserIdButton);

            // Fill Panel
            // -ID Card-
            _nationalIdIF.onEndEdit.AddListener(inputText => IsNationalIdValidated());
            _genderIF.onEndEdit.AddListener(inputText => IsGenderValidated());
            _prefixIF.onEndEdit.AddListener(inputText => IsPrefixValidated());
            _fNameIF.onEndEdit.AddListener(inputText => IsFNameValidated());
            _lNameIF.onEndEdit.AddListener(inputText => IsLNameValidated());
            _birthDateIF.onEndEdit.AddListener(inputText => IsBirthDateValidated());
            _issueDateIF.onEndEdit.AddListener(inputText => IsIssueDateValidated());
            _expireDateIF.onEndEdit.AddListener(inputText => IsExpireDateValidated());
            _houseNumberIF.onEndEdit.AddListener(inputText => IsHouseNumberValidated());
            _subDistrictIF.onEndEdit.AddListener(inputText => IsSubDistrictValidated());
            _districtIF.onEndEdit.AddListener(inputText => IsDistrictValidated());
            _provinceIF.onEndEdit.AddListener(inputText => IsProvinceValidated());
            _countryIF.onEndEdit.AddListener(inputText => IsCountryValidated());

            // -Passport-
            _ppNumberIF.onEndEdit.AddListener(inputText => IsPpNumberValidated());
            _ppNationalityIF.onEndEdit.AddListener(inputText => IsPpNationalityValidated());
            _ppGenderIF.onEndEdit.AddListener(inputText => IsPpGenderValidated());
            _ppFullNameIF.onEndEdit.AddListener(inputText => IsPpFullNameValidated());
            _ppBirthDateIF.onEndEdit.AddListener(inputText => IsPpBirthDateValidated());
            _ppIssueDateIF.onEndEdit.AddListener(inputText => IsPpIssueDateValidated());
            _ppExpireDateIF.onEndEdit.AddListener(inputText => IsPpExpireDateValidated());

            // -My Info-
            _phoneNumberIF.onEndEdit.AddListener(inputText => IsPhoneNumberValidated());
            _medicalIF.onEndEdit.AddListener(inputText => IsMedicalValidated());

            // -Emergency Contact-
            _urgentPhoneNumberIF.onEndEdit.AddListener(inputText => IsUrgentPhoneNumberValidated());
            _urgentPhoneRelateIF.onEndEdit.AddListener(inputText => IsUrgentPhoneRelateValidated());

            // -Social Media-
            _lineIF.onEndEdit.AddListener(inputText => _line = inputText);
            _facebookIF.onEndEdit.AddListener(inputText => _facebook = inputText);
            _igIF.onEndEdit.AddListener(inputText => _ig = inputText);
            _tiktokIF.onEndEdit.AddListener(inputText => _tiktok = inputText);
            _confirmButton.onClick.AddListener(ConfirmButton);

            Register.CurPanel.OnChanged += RefreshUI;
            Register.OnRefreshUI += RefreshUI;
        }

        private async void Start()
        {
            if (!await MyUserData.IsAdmin()) return;

            InitCurPanel();
            RefreshUI();
        }

        private async void OnEnable()
        {
            if (Time.time < WaitUIToTurnOffOnStartTime) return; // Prevent OnEnable() on first Start when UI are seting itself which then it will hide itself. We only want OnEnable() when user open UI.
            if (!await MyUserData.IsAdmin()) return;

            InitCurPanel();
            RefreshUI();
            _register.InitUserDataFromServer();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            // Disable all Panels
            _firstPanel.SetActive(false);
            _secondPanel.SetActive(false);
            _preFillPanel.SetActive(false);
            _fillPanel.SetActive(false);

            // Open based on CurrentPanel
            switch (Register.CurPanel.Peek())
            {
                case ERegisterPanel.Panel1:
                    _firstPanel.SetActive(true);
                    ResetUIData();
                    _register.ResetData();
                    RemoveDataFromInputFields();
                    break;
                case ERegisterPanel.Panel2:
                    _secondPanel.SetActive(true);
                    ResetUIData();
                    _register.ResetData();
                    RemoveDataFromInputFields();
                    break;
                case ERegisterPanel.PanelPreFill:
                    _preFillPanel.SetActive(true);
                    break;
                case ERegisterPanel.PanelFill:
                    _fillPanel.SetActive(true);
                    break;
            }

            // Manage PanelPreFill UI
            if (Register.CurPanel.Peek() == ERegisterPanel.PanelPreFill)
            {
                // No User ID UIs
                if (Register.HasUserId == EHasUserId.NoData || Register.HasUserId == EHasUserId.HasForSure || Register.HasUserId == EHasUserId.NotHas)
                {
                    _userIdIFPanel.SetActive(false);
                    _noUserIdButtonPanel.SetActive(false);
                    _noUserRemarkPanel.SetActive(false);
                }
                else if (Register.HasUserId == EHasUserId.HasNotSure)
                {
                    _userIdIFPanel.SetActive(true);
                    _noUserIdButtonPanel.SetActive(false);
                    _noUserRemarkPanel.SetActive(true);
                }

                // Input Fields
                _cardReaderIFPanel.SetActive(Register.IsCardReader);
                _cardIdIFPanel.SetActive(Register.IsCardId);
                _passportIdIFPanel.SetActive(Register.IsPassport);
            }

            // Manage PanelFill UI
            if (Register.CurPanel.Peek() == ERegisterPanel.PanelFill)
            {
                if (Register.IsCardId || Register.IsCardReader)
                {
                    _nationalPanel.SetActive(true);
                    _passportPanel.SetActive(false);
                }
                else if (Register.IsPassport)
                {
                    _nationalPanel.SetActive(false);
                    _passportPanel.SetActive(true);
                }
            }
        }

        private void InitCurPanel()
        {
            if (Register.CurPanel.Count == 0)
                Register.CurPanel.Push(ERegisterPanel.Panel1);
        }

        private void ResetUIData()
        {
            Register.IsCardReader = false;
            Register.IsCardId = false;
            Register.IsPassport = false;
            Register.HasUserId = EHasUserId.NoData;

            // Removes text on Input Fields
            _cardReaderIF.text = string.Empty;
            _cardIdIF.text = string.Empty;
            _passportIdIF.text = string.Empty;
            _userIdIF.text = string.Empty;
        }

        private bool ValidatePreFillPanel()
        {
            bool status = true;

            if (Register.IsCardReader && !IsCardReaderValidated()) status = false;
            if (Register.IsCardId && !IsCardIdValidated()) status = false;
            if (Register.IsPassport && !IsPassportValidated()) status = false;
            if (Register.HasUserId == EHasUserId.HasNotSure && !IsUserIdValidated())
            {
                _noUserIdButtonPanel.SetActive(true);
                status = false;
            }

            return status;
        }

        private bool ValidateFillPanel()
        {
            bool status = true;

            if (Register.IsCardReader || Register.IsCardId)
            {
                if (!IsNationalIdValidated()) status = false;
                if (!IsGenderValidated()) status = false;
                if (!IsPrefixValidated()) status = false;
                if (!IsFNameValidated()) status = false;
                if (!IsLNameValidated()) status = false;
                if (!IsBirthDateValidated()) status = false;
                if (!IsIssueDateValidated()) status = false;
                if (!IsExpireDateValidated()) status = false;
                if (!IsHouseNumberValidated()) status = false;
                if (!IsSubDistrictValidated()) status = false;
                if (!IsDistrictValidated()) status = false;
                if (!IsProvinceValidated()) status = false;
                if (!IsCountryValidated()) status = false;
            }
            
            if (Register.IsPassport)
            {
                if (!IsPpNumberValidated()) status = false;
                if (!IsPpNationalityValidated()) status = false;
                if (!IsPpGenderValidated()) status = false;
                if (!IsPpFullNameValidated()) status = false;
                if (!IsPpBirthDateValidated()) status = false;
                if (!IsPpIssueDateValidated()) status = false;
                if (!IsPpExpireDateValidated()) status = false;
            }

            if (!IsPhoneNumberValidated()) status = false;
            if (!IsMedicalValidated()) status = false;

            if (!IsUrgentPhoneNumberValidated()) status = false;
            if (!IsUrgentPhoneRelateValidated()) status = false;

            return status;
        }

        private void FillDataToInputFields(string inputText)
        {
            if (Register.IsCardReader)
            {
                NationalIDInfo data = GetNationalIdDataByCardInput(inputText);
                _nationalIdIF.text = data.NationalID;
                _genderIF.text = data.Gender;
                _prefixIF.text = data.Prefix;
                _fNameIF.text = data.FirstName;
                _lNameIF.text = data.LastName;
                _birthDateIF.text = data.BirthDate;
                _issueDateIF.text = data.IssueDate;
                _expireDateIF.text = data.ExpireDate;
                _houseNumberIF.text = data.HouseNumber;
                _subDistrictIF.text = data.Subdistrict;
                _districtIF.text = data.District;
                _provinceIF.text = data.Province;
                _countryIF.text = data.Country;
            }

            if (Register.IsCardId)
            {
                _nationalIdIF.text = inputText;
            }

            if (Register.IsPassport)
            {
                _ppNumberIF.text = inputText;
            }
        }

        private NationalIDInfo GetNationalIdDataByCardInput(string inputText)
        {
            JObject data = null;

            try
            {
                data = JObject.Parse("{" + inputText + "}"); // TODO see if JSON from CardReader has "{ ... }" or not if not we add like this. IF Already has remove "{" and "}"
            }
            catch
            {
                _statusText.Show(_register.FormatNotCorrect.GetLocalizedString(), _register.FormatNotCorrectColor);
            }
            if (data == null) return null;

            return new NationalIDInfo()
            {
                NationalID = data["cid"]?.ToString() ?? "",
                Gender = data["gender"]?.ToString() ?? "",
                Prefix = data["thPrefixName"]?.ToString() ?? "",
                FirstName = data["thFirstName"]?.ToString() ?? "",
                LastName = data["thLastName"]?.ToString() ?? "",
                BirthDate = data["birth"]?.ToString() ?? "",
                IssueDate = data["issue"]?.ToString() ?? "",
                ExpireDate = data["expire"]?.ToString() ?? "",
                HouseNumber = data["addressHouseNumber"]?.ToString() ?? "",
                Subdistrict = data["addressTambon"]?.ToString() ?? "",
                District = data["addressDistrict"]?.ToString() ?? "",
                Province = data["addressProvince"]?.ToString() ?? "",
                Country = data["addressCountry"]?.ToString() ?? ""
            };
        }

        private void RemoveDataFromInputFields()
        {
            // National ID
            _nationalIdIF.text = string.Empty;
            _genderIF.text = string.Empty;
            _prefixIF.text = string.Empty;
            _fNameIF.text = string.Empty;
            _lNameIF.text = string.Empty;
            _birthDateIF.text = string.Empty;
            _issueDateIF.text = string.Empty;
            _expireDateIF.text = string.Empty;
            _houseNumberIF.text = string.Empty;
            _subDistrictIF.text = string.Empty;
            _districtIF.text = string.Empty;
            _provinceIF.text = string.Empty;
            _countryIF.text = string.Empty;

            // Passport
            _ppNumberIF.text = string.Empty;
            _ppNationalityIF.text = string.Empty;
            _ppGenderIF.text = string.Empty;
            _ppFullNameIF.text = string.Empty;
            _ppBirthDateIF.text = string.Empty;
            _ppIssueDateIF.text = string.Empty;
            _ppExpireDateIF.text = string.Empty;

            // My Info
            _phoneNumberIF.text = string.Empty;
            _medicalIF.text = string.Empty;
            // Emergency Contact
            _urgentPhoneNumberIF.text = string.Empty;
            _urgentPhoneRelateIF.text = string.Empty;
            // Social Media
            _lineIF.text = string.Empty;
            _facebookIF.text = string.Empty;
            _igIF.text = string.Empty;
            _tiktokIF.text = string.Empty;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back()
        {
            if (Register.CurPanel.Peek() == ERegisterPanel.Panel1)
            {
                _register.OnBackButtonClick();
                return;
            }
            
            Register.CurPanel.Pop();
        }
        private void ChangeLang() => _register.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber) ~Panel1 / Panel2 / PreFill~
        private bool IsCardReaderValidated() => _inputFieldValidator.ValidateNotNull(
            _cardReaderIF.text, _cardReaderIFS, out _inputFieldText,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsCardIdValidated() => _inputFieldValidator.ValidateNotNull(
            _cardIdIF.text, _cardIdIFS, out _inputFieldText,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsPassportValidated() => _inputFieldValidator.ValidateNotNull(
            _passportIdIF.text, _passportIdIFS, out _inputFieldText,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsUserIdValidated() => _inputFieldValidator.ValidateNotNull(
            _userIdIF.text, _userIdIFS, out _inputUserIdText,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private void AddByReaderButton()
        {
            Register.IsCardReader = true;
            Register.CurPanel.Push(ERegisterPanel.PanelPreFill);
        }

        private void AddManuallyButton()
        {
            Register.CurPanel.Push(ERegisterPanel.Panel2);
        }

        private void UseIdCardButton()
        {
            Register.IsCardId = true;
            Register.CurPanel.Push(ERegisterPanel.PanelPreFill);
        }

        private void UsePassportButton()
        {
            Register.IsPassport = true;
            Register.CurPanel.Push(ERegisterPanel.PanelPreFill);
        }

        private async void NextButton()
        {
            _nextButton.interactable = false;
            if (ValidatePreFillPanel())
            {
                if (Register.HasUserId == EHasUserId.HasNotSure)
                {
                    if (await _register.IsUserIdValid(_inputUserIdText))
                    {
                        // Continue proceed
                        Register.HasUserId = EHasUserId.HasForSure;
                        Register.CurPanel.Push(ERegisterPanel.PanelFill);
                    }
                    else
                    {
                        _noUserIdButtonPanel.SetActive(true);
                    }
                    FillDataToInputFields(_inputFieldText);
                    _nextButton.interactable = true;
                    return;
                }

                bool isMemberExists = false;

                if (Register.IsCardReader)
                {
                    NationalIDInfo nationalData = GetNationalIdDataByCardInput(_inputFieldText);
                    if (nationalData == null)
                    {
                        _nextButton.interactable = true;
                        return;
                    }
                    
                    isMemberExists = await _register.IsMemberExistsByNationalId(nationalData.NationalID);
                }
                else if (Register.IsCardId)
                {
                    isMemberExists = await _register.IsMemberExistsByNationalId(_inputFieldText);
                }
                else if (Register.IsPassport)
                {
                    isMemberExists = await _register.IsMemberExistsByPassport(_inputFieldText);
                }

                if (isMemberExists)
                {
                    _register.OnMemberExistsPopupShowed();
                }
                else
                {
                    _register.OnUsedAppBeforePopupShowed();
                    FillDataToInputFields(_inputFieldText);
                }
            }
            _nextButton.interactable = true;
        }

        private void NoUserIdButton()
        {
            _register.OnCantFindIdPopupShowed();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Fill~
        // -ID Card-
        private bool IsNationalIdValidated() => _inputFieldValidator.ValidateNotNull(
            _nationalIdIF.text, _nationalIdIFS, out _nationalId,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsGenderValidated() => _inputFieldValidator.ValidateNotNull(
            _genderIF.text, _genderIFS, out _gender,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsPrefixValidated() => _inputFieldValidator.ValidateNotNull(
            _prefixIF.text, _prefixIFS, out _prefix,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsFNameValidated() => _inputFieldValidator.ValidateNotNull(
            _fNameIF.text, _fNameIFS, out _fName,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsLNameValidated() => _inputFieldValidator.ValidateNotNull(
            _lNameIF.text, _lNameIFS, out _lName,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsBirthDateValidated() => _inputFieldValidator.ValidateNotNull(
            _birthDateIF.text, _birthDateIFS, out _birthDate,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsIssueDateValidated() => _inputFieldValidator.ValidateNotNull(
            _issueDateIF.text, _issueDateIFS, out _issueDate,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsExpireDateValidated() => _inputFieldValidator.ValidateNotNull(
            _expireDateIF.text, _expireDateIFS, out _expireDate,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsHouseNumberValidated() => _inputFieldValidator.ValidateNotNull(
            _houseNumberIF.text, _houseNumberIFS, out _houseNumber,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsSubDistrictValidated() => _inputFieldValidator.ValidateNotNull(
            _subDistrictIF.text, _subDistrictIFS, out _subDistrict,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsDistrictValidated() => _inputFieldValidator.ValidateNotNull(
            _districtIF.text, _districtIFS, out _district,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsProvinceValidated() => _inputFieldValidator.ValidateNotNull(
            _provinceIF.text, _provinceIFS, out _province,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsCountryValidated() => _inputFieldValidator.ValidateNotNull(
            _countryIF.text, _countryIFS, out _country,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        // -Passport-
        private bool IsPpNumberValidated() => _inputFieldValidator.ValidateNotNull(
            _ppNumberIF.text, _ppNumberIFS, out _ppNumber,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsPpNationalityValidated() => _inputFieldValidator.ValidateNotNull(
            _ppNationalityIF.text, _ppNationalityIFS, out _ppNationality,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsPpGenderValidated() => _inputFieldValidator.ValidateNotNull(
            _ppGenderIF.text, _ppGenderIFS, out _ppGender,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsPpFullNameValidated() => _inputFieldValidator.ValidateNotNull(
            _ppFullNameIF.text, _ppFullNameIFS, out _ppFullName,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsPpBirthDateValidated() => _inputFieldValidator.ValidateNotNull(
            _ppBirthDateIF.text, _ppBirthDateIFS, out _ppBirthDate,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsPpIssueDateValidated() => _inputFieldValidator.ValidateNotNull(
            _ppIssueDateIF.text, _ppIssueDateIFS, out _ppIssueDate,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        private bool IsPpExpireDateValidated() => _inputFieldValidator.ValidateNotNull(
            _ppExpireDateIF.text, _ppExpireDateIFS, out _ppExpireDate,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));

        // -My Info-
        private bool IsPhoneNumberValidated() => _inputFieldValidator.ValidateSignupPhoneNumber(
            _phoneNumberIF.text, _phoneNumberIFS, out _phoneNumber,
            _register.MinimumPhoneNumberLength, _register.MaximumPhoneNumberLength,
            (string.Empty, default),
            (_register.StatusInvalidPhoneNumber.GetLocalizedString(), _register.StatusInvalidPhoneNumberColor),
            (_register.StatusPhoneNumberTooShort.GetLocalizedString(), _register.StatusPhoneNumberTooShortColor),
            (_register.StatusPhoneNumberTooLong.GetLocalizedString(), _register.StatusPhoneNumberTooLongColor));

        private bool IsMedicalValidated() => _inputFieldValidator.ValidateNotNull(
            _medicalIF.text, _medicalIFS, out _medical,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));
        
        
        // -Emergency Contact-
        private bool IsUrgentPhoneNumberValidated() => _inputFieldValidator.ValidateSignupPhoneNumber(
            _urgentPhoneNumberIF.text, _urgentPhoneNumberIFS, out _urgentPhoneNumber,
            _register.MinimumPhoneNumberLength, _register.MaximumPhoneNumberLength,
            (string.Empty, default),
            (_register.StatusInvalidPhoneNumber.GetLocalizedString(), _register.StatusInvalidPhoneNumberColor),
            (_register.StatusPhoneNumberTooShort.GetLocalizedString(), _register.StatusPhoneNumberTooShortColor),
            (_register.StatusPhoneNumberTooLong.GetLocalizedString(), _register.StatusPhoneNumberTooLongColor));

        private bool IsUrgentPhoneRelateValidated() => _inputFieldValidator.ValidateNotNull(
            _urgentPhoneRelateIF.text, _urgentPhoneRelateIFS, out _urgentPhoneRelate,
            (_register.StatusMustBeFilled.GetLocalizedString(), _register.StatusMustBeFilledColor));
        
        private void ConfirmButton()
        {
            if (ValidateFillPanel())
            {
                NationalIDInfo nationalIDInfo = new NationalIDInfo()
                {
                    NationalID = _nationalId,
                    Gender = _gender,
                    Prefix = _prefix,
                    FirstName = _fName,
                    LastName = _lName,
                    BirthDate = _birthDate,
                    IssueDate = _issueDate,
                    ExpireDate = _expireDate,
                    HouseNumber = _houseNumber,
                    Subdistrict = _subDistrict,
                    District = _district,
                    Province = _province,
                    Country = _country
                };
                
                PassportInfo passportInfo = new PassportInfo()
                {
                    PassportNumber = _ppNumber,
                    Nationality = _ppNationality,
                    Gender = _ppGender,
                    FullName = _ppFullName,
                    BirthDate = _ppBirthDate,
                    IssueDate = _ppIssueDate,
                    ExpireDate = _ppExpireDate
                };

                GeneralInfo generalInfo = new GeneralInfo()
                {
                    PhoneNumber = _phoneNumber,
                    MedicalCondition = _medical,
                    EmergencyContact = new EmergencyContact()
                    {
                        PhoneNumber = _urgentPhoneNumber,
                        Relation = _urgentPhoneRelate
                    },
                    SocialAccounts = new SocialAccounts()
                    {
                        Line = _line,
                        Facebook = _facebook,
                        Instagram = _ig,
                        Tiktok = _tiktok
                    }
                };

                _register.StoreData(nationalIDInfo, passportInfo, generalInfo);
                _register.OnBeforeConfirmPopupShowed();
            }
        }
        #endregion
    }
}