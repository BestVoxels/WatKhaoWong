using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Identities;
using WatKhaoWong.Retreats;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.UI.Retreats
{
    public class PersonalRowUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("General")]
        [SerializeField] private GameObject[] _toShowHideByMode;
        [SerializeField] private GameObject[] _toShowHideByModeOnlyAdmin;
        [SerializeField] private GameObject _iDCardSectionToShowHide;
        [SerializeField] private GameObject _passportSectionToShowHide;

        [Header("-ID Card-")]
        [SerializeField] private TMP_InputField _nationalIdIF;
        [SerializeField] private TMP_Text _nationalIdRT;
        [SerializeField] private InputFieldStatus _nationalIdIFS;
        [Space]
        [SerializeField] private TMP_InputField _genderIF;
        [SerializeField] private TMP_Text _genderRT;
        [SerializeField] private InputFieldStatus _genderIFS;
        [Space]
        [SerializeField] private TMP_InputField _prefixIF;
        [SerializeField] private TMP_Text _prefixRT;
        [SerializeField] private InputFieldStatus _prefixIFS;
        [Space]
        [SerializeField] private TMP_InputField _fNameIF;
        [SerializeField] private TMP_Text _fNameRT;
        [SerializeField] private InputFieldStatus _fNameIFS;
        [Space]
        [SerializeField] private TMP_InputField _lNameIF;
        [SerializeField] private TMP_Text _lNameRT;
        [SerializeField] private InputFieldStatus _lNameIFS;
        [Space]
        [SerializeField] private TMP_InputField _birthDateIF;
        [SerializeField] private TMP_Text _birthDateRT;
        [SerializeField] private InputFieldStatus _birthDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _issueDateIF;
        [SerializeField] private TMP_Text _issueDateRT;
        [SerializeField] private InputFieldStatus _issueDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _expireDateIF;
        [SerializeField] private TMP_Text _expireDateRT;
        [SerializeField] private InputFieldStatus _expireDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _houseNumberIF;
        [SerializeField] private TMP_Text _houseNumberRT;
        [SerializeField] private InputFieldStatus _houseNumberIFS;
        [Space]
        [SerializeField] private TMP_InputField _subDistrictIF;
        [SerializeField] private TMP_Text _subDistrictRT;
        [SerializeField] private InputFieldStatus _subDistrictIFS;
        [Space]
        [SerializeField] private TMP_InputField _districtIF;
        [SerializeField] private TMP_Text _districtRT;
        [SerializeField] private InputFieldStatus _districtIFS;
        [Space]
        [SerializeField] private TMP_InputField _provinceIF;
        [SerializeField] private TMP_Text _provinceRT;
        [SerializeField] private InputFieldStatus _provinceIFS;
        [Space]
        [SerializeField] private TMP_InputField _countryIF;
        [SerializeField] private TMP_Text _countryRT;
        [SerializeField] private InputFieldStatus _countryIFS;
        [Space]
        [SerializeField] private Button _idCardCB;

        [Header("-Passport-")]
        [SerializeField] private TMP_InputField _ppNumberIF;
        [SerializeField] private TMP_Text _ppNumberRT;
        [SerializeField] private InputFieldStatus _ppNumberIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppNationalityIF;
        [SerializeField] private TMP_Text _ppNationalityRT;
        [SerializeField] private InputFieldStatus _ppNationalityIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppGenderIF;
        [SerializeField] private TMP_Text _ppGenderRT;
        [SerializeField] private InputFieldStatus _ppGenderIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppFullNameIF;
        [SerializeField] private TMP_Text _ppFullNameRT;
        [SerializeField] private InputFieldStatus _ppFullNameIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppBirthDateIF;
        [SerializeField] private TMP_Text _ppBirthDateRT;
        [SerializeField] private InputFieldStatus _ppBirthDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppIssueDateIF;
        [SerializeField] private TMP_Text _ppIssueDateRT;
        [SerializeField] private InputFieldStatus _ppIssueDateIFS;
        [Space]
        [SerializeField] private TMP_InputField _ppExpireDateIF;
        [SerializeField] private TMP_Text _ppExpireDateRT;
        [SerializeField] private InputFieldStatus _ppExpireDateIFS;
        [Space]
        [SerializeField] private Button _passportCB;

        [Header("-My Info-")]
        [SerializeField] private TMP_InputField _phoneNumberIF;
        [SerializeField] private TMP_Text _phoneNumberRT;
        [SerializeField] private InputFieldStatus _phoneNumberIFS;
        [Space]
        [SerializeField] private TMP_InputField _medicalIF;
        [SerializeField] private TMP_Text _medicalRT;
        [SerializeField] private InputFieldStatus _medicalIFS;
        [Space]
        [SerializeField] private Button _myInfoCB;

        [Header("-Emergency Contact-")]
        [SerializeField] private TMP_InputField _urgentPhoneNumberIF;
        [SerializeField] private TMP_Text _urgentPhoneNumberRT;
        [SerializeField] private InputFieldStatus _urgentPhoneNumberIFS;
        [Space]
        [SerializeField] private TMP_InputField _urgentPhoneRelateIF;
        [SerializeField] private TMP_Text _urgentPhoneRelateRT;
        [SerializeField] private InputFieldStatus _urgentPhoneRelateIFS;
        [Space]
        [SerializeField] private Button _emergencyContactCB;

        [Header("-Social Media-")]
        [SerializeField] private TMP_InputField _lineIF;
        [SerializeField] private TMP_Text _lineRT;
        [Space]
        [SerializeField] private TMP_InputField _facebookIF;
        [SerializeField] private TMP_Text _facebookRT;
        [Space]
        [SerializeField] private TMP_InputField _igIF;
        [SerializeField] private TMP_Text _igRT;
        [Space]
        [SerializeField] private TMP_InputField _tiktokIF;
        [SerializeField] private TMP_Text _tiktokRT;
        [Space]
        [SerializeField] private Button _socialMediaCB;
        #endregion



        #region --Fields-- (In Class)
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

        // For -Viewer-
        private NationalIDInfo _nationalIDInfo;
        private PassportInfo _passportInfo;
        private GeneralInfo _generalInfo;

        private PersonalRow _personalRow;
        private MyUserData _myUserData;
        private UserInfo _userInfo;
        private InputFieldValidator _inputFieldValidator;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _personalRow = player.GetComponentInChildren<PersonalRow>();
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _userInfo = player.GetComponentInChildren<UserInfo>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();

            _userInfo.OnModeChanged += HandleUIByEditMode; // put here so it continues to works even when on PersonalInfo, GenearlInfo tabs

            _personalRow.OnNationalUploadedToServer += UploadNationalToServer;
            _personalRow.OnPassportUploadedToServer += UploadPassportToServer;
            _personalRow.OnGeneralUploadedToServer += UploadGeneralToServer;

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
            _idCardCB.onClick.AddListener(ConfirmIdCard);

            // -Passport-
            _ppNumberIF.onEndEdit.AddListener(inputText => IsPpNumberValidated());
            _ppNationalityIF.onEndEdit.AddListener(inputText => IsPpNationalityValidated());
            _ppGenderIF.onEndEdit.AddListener(inputText => IsPpGenderValidated());
            _ppFullNameIF.onEndEdit.AddListener(inputText => IsPpFullNameValidated());
            _ppBirthDateIF.onEndEdit.AddListener(inputText => IsPpBirthDateValidated());
            _ppIssueDateIF.onEndEdit.AddListener(inputText => IsPpIssueDateValidated());
            _ppExpireDateIF.onEndEdit.AddListener(inputText => IsPpExpireDateValidated());
            _passportCB.onClick.AddListener(ConfirmPassport);

            // -My Info-
            _phoneNumberIF.onEndEdit.AddListener(inputText => IsPhoneNumberValidated());
            _medicalIF.onEndEdit.AddListener(inputText => IsMedicalValidated());
            _myInfoCB.onClick.AddListener(ConfirmMyInfo);

            // -Emergency Contact-
            _urgentPhoneNumberIF.onEndEdit.AddListener(inputText => IsUrgentPhoneNumberValidated());
            _urgentPhoneRelateIF.onEndEdit.AddListener(inputText => IsUrgentPhoneRelateValidated());
            _emergencyContactCB.onClick.AddListener(ConfirmEmergencyContact);

            // -Social Media-
            _lineIF.onEndEdit.AddListener(inputText => _line = inputText);
            _facebookIF.onEndEdit.AddListener(inputText => _facebook = inputText);
            _igIF.onEndEdit.AddListener(inputText => _ig = inputText);
            _tiktokIF.onEndEdit.AddListener(inputText => _tiktok = inputText);
            _socialMediaCB.onClick.AddListener(ConfirmSocialMedia);

            UIRefresher.OnMeditationRetreatRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnEnable()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            SetTextWhenNationalIDExists();
            SetTextWhenPassportExists();
            SetTextWhenGeneralInfoExists();
        }

        private bool IsAdmin() => _myUserData.GetRole() == EUserRole.Admin;
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Viewer~
        private async void SetTextWhenNationalIDExists()
        {
            _nationalIDInfo = await _myUserData.GetDataNationalIDInfo();
            ShowHideIDCardSectionUI(true);

            if (_nationalIDInfo == null)
            {
                ShowHideIDCardSectionUI(false);
                return;
            }

            // -National ID-
            _nationalIdRT.text = _nationalIDInfo.NationalID;
            _genderRT.text = _nationalIDInfo.Gender;
            _prefixRT.text = _nationalIDInfo.Prefix;
            _fNameRT.text = _nationalIDInfo.FirstName;
            _lNameRT.text = _nationalIDInfo.LastName;
            _birthDateRT.text = _nationalIDInfo.BirthDate;
            _issueDateRT.text = _nationalIDInfo.IssueDate;
            _expireDateRT.text = _nationalIDInfo.ExpireDate;
            _houseNumberRT.text = _nationalIDInfo.HouseNumber;
            _subDistrictRT.text = _nationalIDInfo.Subdistrict;
            _districtRT.text = _nationalIDInfo.District;
            _provinceRT.text = _nationalIDInfo.Province;
            _countryRT.text = _nationalIDInfo.Country;
        }

        private async void SetTextWhenPassportExists()
        {
            _passportInfo = await _myUserData.GetDataPassportInfo();
            ShowHidePassportSectionUI(true);

            if (_passportInfo == null)
            {
                ShowHidePassportSectionUI(false);
                return;
            }

            // -Passport-
            _ppNumberRT.text = _passportInfo.PassportNumber;
            _ppNationalityRT.text = _passportInfo.Nationality;
            _ppGenderRT.text = _passportInfo.Gender;
            _ppFullNameRT.text = _passportInfo.FullName;
            _ppBirthDateRT.text = _passportInfo.BirthDate;
            _ppIssueDateRT.text = _passportInfo.IssueDate;
            _ppExpireDateRT.text = _passportInfo.ExpireDate;
        }

        private async void SetTextWhenGeneralInfoExists()
        {
            _generalInfo = await _myUserData.GetDataGeneralInfo();

            if (_generalInfo == null) return;

            // -My Info-
            _phoneNumberRT.text = _generalInfo.PhoneNumber;
            _medicalRT.text = _generalInfo.MedicalCondition;

            // -Emergency Contact-
            if (_generalInfo.EmergencyContact != null)
            {
                _urgentPhoneNumberRT.text = _generalInfo.EmergencyContact.PhoneNumber;
                _urgentPhoneRelateRT.text = _generalInfo.EmergencyContact.Relation;
            }

            // -Social Media-
            if (_generalInfo.SocialAccounts != null)
            {
                _lineRT.text = _generalInfo.SocialAccounts.Line;
                _facebookRT.text = _generalInfo.SocialAccounts.Facebook;
                _igRT.text = _generalInfo.SocialAccounts.Instagram;
                _tiktokRT.text = _generalInfo.SocialAccounts.Tiktok;
            }
        }

        private void ShowHideIDCardSectionUI(bool toShow)
        {
            if (toShow == false && IsAdmin()) return;

            _iDCardSectionToShowHide.SetActive(toShow);
        }

        private void ShowHidePassportSectionUI(bool toShow)
        {
            if (toShow == false && IsAdmin()) return;

            _passportSectionToShowHide.SetActive(toShow);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Setter~
        // -ID Card-
        private bool ValidateIdCard()
        {
            bool status = true;

            if (IsAdmin())
            {
                if (!IsNationalIdValidated()) status = false;
                if (!IsGenderValidated()) status = false;
                if (!IsPrefixValidated()) status = false;
                if (!IsFNameValidated()) status = false;
                if (!IsLNameValidated()) status = false;
                if (!IsBirthDateValidated()) status = false;
                if (!IsIssueDateValidated()) status = false;
                if (!IsExpireDateValidated()) status = false;
            }
            if (!IsHouseNumberValidated()) status = false;
            if (!IsSubDistrictValidated()) status = false;
            if (!IsDistrictValidated()) status = false;
            if (!IsProvinceValidated()) status = false;
            if (!IsCountryValidated()) status = false;

            return status;
        }

        // -Passport-
        private bool ValidatePassport()
        {
            bool status = true;

            if (IsAdmin())
            {
                if (!IsPpNumberValidated()) status = false;
                if (!IsPpNationalityValidated()) status = false;
                if (!IsPpGenderValidated()) status = false;
                if (!IsPpFullNameValidated()) status = false;
                if (!IsPpBirthDateValidated()) status = false;
                if (!IsPpIssueDateValidated()) status = false;
                if (!IsPpExpireDateValidated()) status = false;
            }
            else
                status = false;

            return status;
        }

        // -My Info-
        private bool ValidateMyInfo()
        {
            bool status = true;

            if (!IsPhoneNumberValidated()) status = false;
            if (!IsMedicalValidated()) status = false;

            return status;
        }

        // -Emergency Contact-
        private bool ValidateEmergencyContact()
        {
            bool status = true;

            if (!IsUrgentPhoneNumberValidated()) status = false;
            if (!IsUrgentPhoneRelateValidated()) status = false;

            return status;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void HandleUIByEditMode(EViewEditMode mode)
        {
            bool isEditing = mode == EViewEditMode.Edit;

            // Show Hide GameObjects - ALL excepets Passport since there is nothing Member can do or edit fields
            foreach (GameObject each in _toShowHideByMode)
            {
                each.SetActive(isEditing);
            }

            // Show Hide GameObjects - Passport
            foreach (GameObject each in _toShowHideByModeOnlyAdmin)
            {
                if (IsAdmin())
                    each.SetActive(isEditing);
                else
                    each.SetActive(false);
            }

            // Show InputFields & Hide ResultTexts - National ID
            _nationalIdIF.gameObject.SetActive(isEditing && IsAdmin());
            _nationalIdRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _genderIF.gameObject.SetActive(isEditing && IsAdmin());
            _genderRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _prefixIF.gameObject.SetActive(isEditing && IsAdmin());
            _prefixRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _fNameIF.gameObject.SetActive(isEditing && IsAdmin());
            _fNameRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _lNameIF.gameObject.SetActive(isEditing && IsAdmin());
            _lNameRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _birthDateIF.gameObject.SetActive(isEditing && IsAdmin());
            _birthDateRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _issueDateIF.gameObject.SetActive(isEditing && IsAdmin());
            _issueDateRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _expireDateIF.gameObject.SetActive(isEditing && IsAdmin());
            _expireDateRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _houseNumberIF.gameObject.SetActive(isEditing);
            _houseNumberRT.gameObject.SetActive(!isEditing);

            _subDistrictIF.gameObject.SetActive(isEditing);
            _subDistrictRT.gameObject.SetActive(!isEditing);

            _districtIF.gameObject.SetActive(isEditing);
            _districtRT.gameObject.SetActive(!isEditing);

            _provinceIF.gameObject.SetActive(isEditing);
            _provinceRT.gameObject.SetActive(!isEditing);

            _countryIF.gameObject.SetActive(isEditing);
            _countryRT.gameObject.SetActive(!isEditing);

            // Show InputFields & Hide ResultTexts - Passport
            _ppNumberIF.gameObject.SetActive(isEditing && IsAdmin());
            _ppNumberRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _ppNationalityIF.gameObject.SetActive(isEditing && IsAdmin());
            _ppNationalityRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _ppGenderIF.gameObject.SetActive(isEditing && IsAdmin());
            _ppGenderRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _ppFullNameIF.gameObject.SetActive(isEditing && IsAdmin());
            _ppFullNameRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _ppBirthDateIF.gameObject.SetActive(isEditing && IsAdmin());
            _ppBirthDateRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _ppIssueDateIF.gameObject.SetActive(isEditing && IsAdmin());
            _ppIssueDateRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            _ppExpireDateIF.gameObject.SetActive(isEditing && IsAdmin());
            _ppExpireDateRT.gameObject.SetActive(IsAdmin() ? !isEditing : true);

            // Show InputFields & Hide ResultTexts - My Info
            _phoneNumberIF.gameObject.SetActive(isEditing);
            _phoneNumberRT.gameObject.SetActive(!isEditing);

            _medicalIF.gameObject.SetActive(isEditing);
            _medicalRT.gameObject.SetActive(!isEditing);

            // Show InputFields & Hide ResultTexts - Emergency Contact
            _urgentPhoneNumberIF.gameObject.SetActive(isEditing);
            _urgentPhoneNumberRT.gameObject.SetActive(!isEditing);

            _urgentPhoneRelateIF.gameObject.SetActive(isEditing);
            _urgentPhoneRelateRT.gameObject.SetActive(!isEditing);

            // Show InputFields & Hide ResultTexts - Social Media
            _lineIF.gameObject.SetActive(isEditing);
            _lineRT.gameObject.SetActive(!isEditing);

            _facebookIF.gameObject.SetActive(isEditing);
            _facebookRT.gameObject.SetActive(!isEditing);

            _igIF.gameObject.SetActive(isEditing);
            _igRT.gameObject.SetActive(!isEditing);

            _tiktokIF.gameObject.SetActive(isEditing);
            _tiktokRT.gameObject.SetActive(!isEditing);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Viewer~
        private void UploadNationalToServer(NationalIDInfo nationalIDInfo)
        {
            _nationalIDInfo = nationalIDInfo;

            SetTextWhenNationalIDExists();
        }

        private void UploadPassportToServer(PassportInfo passportInfo)
        {
            _passportInfo = passportInfo;

            SetTextWhenPassportExists();
        }

        private void UploadGeneralToServer(GeneralInfo generalInfo)
        {
            _generalInfo = generalInfo;

            SetTextWhenGeneralInfoExists();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Setter~
        // -ID Card-
        private bool IsNationalIdValidated() => _inputFieldValidator.ValidateNotNull(
            _nationalIdIF.text, _nationalIdIFS, out _nationalId,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsGenderValidated() => _inputFieldValidator.ValidateNotNull(
            _genderIF.text, _genderIFS, out _gender,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsPrefixValidated() => _inputFieldValidator.ValidateNotNull(
            _prefixIF.text, _prefixIFS, out _prefix,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsFNameValidated() => _inputFieldValidator.ValidateNotNull(
            _fNameIF.text, _fNameIFS, out _fName,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsLNameValidated() => _inputFieldValidator.ValidateNotNull(
            _lNameIF.text, _lNameIFS, out _lName,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsBirthDateValidated() => _inputFieldValidator.ValidateNotNull(
            _birthDateIF.text, _birthDateIFS, out _birthDate,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsIssueDateValidated() => _inputFieldValidator.ValidateNotNull(
            _issueDateIF.text, _issueDateIFS, out _issueDate,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsExpireDateValidated() => _inputFieldValidator.ValidateNotNull(
            _expireDateIF.text, _expireDateIFS, out _expireDate,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsHouseNumberValidated() => _inputFieldValidator.ValidateNotNull(
            _houseNumberIF.text, _houseNumberIFS, out _houseNumber,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsSubDistrictValidated() => _inputFieldValidator.ValidateNotNull(
            _subDistrictIF.text, _subDistrictIFS, out _subDistrict,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsDistrictValidated() => _inputFieldValidator.ValidateNotNull(
            _districtIF.text, _districtIFS, out _district,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsProvinceValidated() => _inputFieldValidator.ValidateNotNull(
            _provinceIF.text, _provinceIFS, out _province,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsCountryValidated() => _inputFieldValidator.ValidateNotNull(
            _countryIF.text, _countryIFS, out _country,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private void ConfirmIdCard()
        {
            if (ValidateIdCard())
            {
                _personalRow.OnIdCardValidateSucceeded(_nationalId, _gender, _prefix, _fName, _lName, _birthDate, _issueDate, _expireDate, _houseNumber, _subDistrict, _district, _province, _country);
            }
            else
            {
                _personalRow.OnIdCardValidateFailed();
            }
        }

        // -Passport-
        private bool IsPpNumberValidated() => _inputFieldValidator.ValidateNotNull(
            _ppNumberIF.text, _ppNumberIFS, out _ppNumber,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsPpNationalityValidated() => _inputFieldValidator.ValidateNotNull(
            _ppNationalityIF.text, _ppNationalityIFS, out _ppNationality,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsPpGenderValidated() => _inputFieldValidator.ValidateNotNull(
            _ppGenderIF.text, _ppGenderIFS, out _ppGender,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsPpFullNameValidated() => _inputFieldValidator.ValidateNotNull(
            _ppFullNameIF.text, _ppFullNameIFS, out _ppFullName,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsPpBirthDateValidated() => _inputFieldValidator.ValidateNotNull(
            _ppBirthDateIF.text, _ppBirthDateIFS, out _ppBirthDate,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsPpIssueDateValidated() => _inputFieldValidator.ValidateNotNull(
            _ppIssueDateIF.text, _ppIssueDateIFS, out _ppIssueDate,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private bool IsPpExpireDateValidated() => _inputFieldValidator.ValidateNotNull(
            _ppExpireDateIF.text, _ppExpireDateIFS, out _ppExpireDate,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private void ConfirmPassport()
        {
            if (ValidatePassport())
            {
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

                _personalRow.OnPassportValidateSucceeded(passportInfo);
            }
            else
            {
                _personalRow.OnPassportValidateFailed();
            }
        }

        // -My Info-
        private bool IsPhoneNumberValidated() => _inputFieldValidator.ValidateSignupPhoneNumber(
            _phoneNumberIF.text, _phoneNumberIFS, out _phoneNumber,
            _userInfo.MinimumPhoneNumberLength, _userInfo.MaximumPhoneNumberLength,
            (string.Empty, default),
            (_userInfo.StatusInvalidPhoneNumber.GetLocalizedString(), _userInfo.StatusInvalidPhoneNumberColor),
            (_userInfo.StatusPhoneNumberTooShort.GetLocalizedString(), _userInfo.StatusPhoneNumberTooShortColor),
            (_userInfo.StatusPhoneNumberTooLong.GetLocalizedString(), _userInfo.StatusPhoneNumberTooLongColor));

        private bool IsMedicalValidated() => _inputFieldValidator.ValidateNotNull(
            _medicalIF.text, _medicalIFS, out _medical,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private void ConfirmMyInfo()
        {
            if (ValidateMyInfo())
            {
                _personalRow.OnMyInfoValidateSucceeded(_phoneNumber, _medical);
            }
            else
            {
                _personalRow.OnMyInfoValidateFailed();
            }
        }

        // -Emergency Contact-
        private bool IsUrgentPhoneNumberValidated() => _inputFieldValidator.ValidateSignupPhoneNumber(
            _urgentPhoneNumberIF.text, _urgentPhoneNumberIFS, out _urgentPhoneNumber,
            _userInfo.MinimumPhoneNumberLength, _userInfo.MaximumPhoneNumberLength,
            (string.Empty, default),
            (_userInfo.StatusInvalidPhoneNumber.GetLocalizedString(), _userInfo.StatusInvalidPhoneNumberColor),
            (_userInfo.StatusPhoneNumberTooShort.GetLocalizedString(), _userInfo.StatusPhoneNumberTooShortColor),
            (_userInfo.StatusPhoneNumberTooLong.GetLocalizedString(), _userInfo.StatusPhoneNumberTooLongColor));

        private bool IsUrgentPhoneRelateValidated() => _inputFieldValidator.ValidateNotNull(
            _urgentPhoneRelateIF.text, _urgentPhoneRelateIFS, out _urgentPhoneRelate,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private void ConfirmEmergencyContact()
        {
            if (ValidateEmergencyContact())
            {
                _personalRow.OnEmergencyContactValidateSucceeded(_urgentPhoneNumber, _urgentPhoneRelate);
            }
            else
            {
                _personalRow.OnEmergencyContactValidateFailed();
            }
        }

        // -Social Media-
        private void ConfirmSocialMedia()
        {
            _personalRow.OnSocialMediaValidateSucceeded(_line, _facebook, _ig, _tiktok);
        }
        #endregion
    }
}