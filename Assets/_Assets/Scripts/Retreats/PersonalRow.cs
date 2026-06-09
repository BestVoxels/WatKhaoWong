using System;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Retreats
{
    public class PersonalRow : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Status Setter Event")]
        [SerializeField] private UnityEvent _onIdCardValidateSucceeded;
        [SerializeField] private UnityEvent _onIdCardValidateFailed;
        [Space]
        [SerializeField] private UnityEvent _onPassportValidateSucceeded;
        [SerializeField] private UnityEvent _onPassportValidateFailed;
        [Space]
        [SerializeField] private UnityEvent _onMyInfoValidateSucceeded;
        [SerializeField] private UnityEvent _onMyInfoValidateFailed;
        [Space]
        [SerializeField] private UnityEvent _onEmergencyContactValidateSucceeded;
        [SerializeField] private UnityEvent _onEmergencyContactValidateFailed;
        [Space]
        [SerializeField] private UnityEvent _onSocialMediaValidateSucceeded;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<NationalIDInfo> OnNationalUploadedToServer;
        public event Action<PassportInfo> OnPassportUploadedToServer;
        public event Action<GeneralInfo> OnGeneralUploadedToServer;
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
        private PassportInfo _passportInfo;

        // For -Setter- My Info
        private string _phoneNumber, _medical;

        // For -Setter- Emergency Contact
        private string _urgentPhoneNumber, _urgentPhoneRelate;

        // For -Setter- Social Media
        private string _line, _facebook, _ig, _tiktok;

        private MyUserData _myUserData;
        private IUserData _userData;
        private UserInfo _userInfo;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _userInfo = player.GetComponentInChildren<UserInfo>();
            _statusText = FindAnyObjectByType<StatusText>();

            _userData = _myUserData;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnIdCardValidateSucceeded(IUserData userData, string nationalId, string gender, string prefix, string fName, string lName, string birthDate, string issueDate, string expireDate, string houseNumber, string subDistrict, string district, string province, string country)
        {
            _userData = userData;

            _nationalId = nationalId;
            _gender = gender;
            _prefix = prefix;
            _fName = fName;
            _lName = lName;
            _birthDate = birthDate;
            _issueDate = issueDate;
            _expireDate = expireDate;
            _houseNumber = houseNumber;
            _subDistrict = subDistrict;
            _district = district;
            _province = province;
            _country = country;

            _onIdCardValidateSucceeded?.Invoke();
        }
        public void OnIdCardValidateFailed()
        {
            _onIdCardValidateFailed?.Invoke();
        }


        public void OnPassportValidateSucceeded(IUserData userData, PassportInfo passportInfo)
        {
            _userData = userData;

            _passportInfo = passportInfo;

            _onPassportValidateSucceeded?.Invoke();
        }
        public void OnPassportValidateFailed()
        {
            _onPassportValidateFailed?.Invoke();
        }


        public void OnMyInfoValidateSucceeded(IUserData userData, string phoneNumber, string medical)
        {
            _userData = userData;

            _phoneNumber = phoneNumber;
            _medical = medical;

            _onMyInfoValidateSucceeded?.Invoke();
        }
        public void OnMyInfoValidateFailed()
        {
            _onMyInfoValidateFailed?.Invoke();
        }


        public void OnEmergencyContactValidateSucceeded(IUserData userData, string urgentPhoneNumber, string urgentPhoneRelate)
        {
            _userData = userData;

            _urgentPhoneNumber = urgentPhoneNumber;
            _urgentPhoneRelate = urgentPhoneRelate;

            _onEmergencyContactValidateSucceeded?.Invoke();
        }
        public void OnEmergencyContactValidateFailed()
        {
            _onEmergencyContactValidateFailed?.Invoke();
        }


        public void OnSocialMediaValidateSucceeded(IUserData userData, string line, string facebook, string ig, string tiktok)
        {
            _userData = userData;

            _line = line;
            _facebook = facebook;
            _ig = ig;
            _tiktok = tiktok;

            _onSocialMediaValidateSucceeded?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UploadSideWork()
        {
            _statusText.Show(_userInfo.StatusChangesSaved.GetLocalizedString(), _userInfo.StatusChangesSavedColor);
        }

        private bool IsAdmin() => _myUserData.GetRole() == EUserRole.Admin;
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public async void UploadIdCardToServer()
        {
            // Upload to Server -> 'User Themselves' National ID Info
            await _userData.SetDataNationalIDInfo(_nationalId, _gender, _prefix, _fName, _lName, _birthDate, _issueDate, _expireDate, _houseNumber, _subDistrict, _district, _province, _country);

            // Incase Subscriber class wanted to do something when uploaded
            OnNationalUploadedToServer?.Invoke(await _userData.GetDataNationalIDInfo()); // Get Data from server so it shows other value that is null as well

            UploadSideWork();
        }

        public async void UploadPassportToServer()
        {
            if (!IsAdmin()) return;

            // Upload to Server -> 'User Themselves' Passport Info
            await _userData.SetDataPassportInfo(_passportInfo);

            // Incase Subscriber class wanted to do something when uploaded
            OnPassportUploadedToServer?.Invoke(await _userData.GetDataPassportInfo()); // Get Data from server so it shows other value that is null as well

            UploadSideWork();
        }

        public async void UploadMyInfoToServer()
        {
            // Upload to Server -> 'User Themselves' General Info
            await _userData.SetDataGeneralInfo(_phoneNumber, _medical, null, null, null, null, null, null);

            // Incase Subscriber class wanted to do something when uploaded
            OnGeneralUploadedToServer?.Invoke(await _userData.GetDataGeneralInfo()); // Get Data from server so it shows other value that is null as well

            UploadSideWork();
        }

        public async void UploadEmergencyContactToServer()
        {
            // Upload to Server -> 'User Themselves' General Info
            await _userData.SetDataGeneralInfo(null, null, _urgentPhoneNumber, _urgentPhoneRelate, null, null, null, null);

            // Incase Subscriber class wanted to do something when uploaded
            OnGeneralUploadedToServer?.Invoke(await _userData.GetDataGeneralInfo()); // Get Data from server so it shows other value that is null as well

            UploadSideWork();
        }

        public async void UploadSocialMediaToServer()
        {
            // Upload to Server -> 'User Themselves' General Info
            await _userData.SetDataGeneralInfo(null, null, null, null, _line, _facebook, _ig, _tiktok);

            // Incase Subscriber class wanted to do something when uploaded
            OnGeneralUploadedToServer?.Invoke(await _userData.GetDataGeneralInfo()); // Get Data from server so it shows other value that is null as well

            UploadSideWork();
        }
        #endregion
    }
}