using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Utils.Core;
using UnityEngine.Localization;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Identities;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Functions;

namespace WatKhaoWong.Admin
{
    public class Register : Page
    {
        #region --Fields-- (Inspector)
        [Header("Status Text")]
        [SerializeField] private LocalizedString _memberNotFound;
        [SerializeField] private Color32 _memberNotFoundColor;
        [SerializeField] private LocalizedString _uploadedSucceeded;
        [SerializeField] private Color32 _uploadedSucceededColor;
        [SerializeField] private LocalizedString _accountCreatedSucceeded;
        [SerializeField] private Color32 _accountCreatedSucceededColor;
        [SerializeField] private LocalizedString _accountCreatedFailed;
        [SerializeField] private Color32 _accountCreatedFailedColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Status Text")]
        [field: SerializeField] public LocalizedString StatusMustBeFilled { get; private set; }
        [field: SerializeField] public Color32 StatusMustBeFilledColor { get; private set; }
        [field: SerializeField] public LocalizedString FormatNotCorrect { get; private set; }
        [field: SerializeField] public Color32 FormatNotCorrectColor { get; private set; }

        [field: Header("Phone Number on Fill Panel")]
        [field: SerializeField] public byte MinimumPhoneNumberLength { get; private set; } = 9;
        [field: SerializeField] public byte MaximumPhoneNumberLength { get; private set; } = 10;
        [field: Space]
        [field: SerializeField] public LocalizedString StatusInvalidPhoneNumber { get; private set; }
        [field: SerializeField] public Color32 StatusInvalidPhoneNumberColor { get; private set; }
        [field: SerializeField] public LocalizedString StatusPhoneNumberTooShort { get; private set; }
        [field: SerializeField] public Color32 StatusPhoneNumberTooShortColor { get; private set; }
        [field: SerializeField] public LocalizedString StatusPhoneNumberTooLong { get; private set; }
        [field: SerializeField] public Color32 StatusPhoneNumberTooLongColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Register UI Event")]
        [SerializeField] private UnityEvent _onMemberExistsPopupShowed;
        [SerializeField] private UnityEvent _onUsedAppBeforePopupShowed;
        [SerializeField] private UnityEvent _onCantFindIdPopupShowed;
        [SerializeField] private UnityEvent _onBeforeConfirmPopupShowed;
        [SerializeField] private UnityEvent _onNewAccountPopupShowed;
        #endregion



        #region --Events-- (Delegate as Action)
        public static event Action OnRefreshUI;
        #endregion



        #region --Properties-- (Auto)
        public static ObservableStack<ERegisterPanel> CurPanel { get; set; } = new();
        public static bool IsCardReader { get; set; } = false;
        public static bool IsCardId { get; set; } = false;
        public static bool IsPassport { get; set; } = false;
        #endregion



        #region --Properties-- (With Backing Fields)
        public static EHasUserId HasUserId { get; set; } = EHasUserId.NoData;
        #endregion



        #region --Fields-- (In Class)
        private NationalIDInfo _nationalIDInfo;
        private PassportInfo _passportInfo;
        private GeneralInfo _generalInfo;
        private List<IUserData> _cachedUserData = new();

        private SavingWrapper _savingWrapper;
        private StatusText _statusText;
        private IUserData _userData = null;
        private ServerTime _serverTime;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            // GameObject player = GameObject.FindWithTag("Player");
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _statusText = FindAnyObjectByType<StatusText>();
            _serverTime = FindAnyObjectByType<ServerTime>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public async void InitUserDataFromServer()
        {
            _cachedUserData.Clear();
            await foreach (DataSnapshot dataSnapShot in _savingWrapper.LoadAllUsers())
            {
                IUserData data = new OtherUserData(dataSnapShot);
                _cachedUserData.Add(data);
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public async Task<bool> IsMemberExistsByNationalId(string nationalId)
        {
            while (_cachedUserData.Count == 0)
                await Task.Yield();
            
            foreach (IUserData each in _cachedUserData)
            {
                if (each.GetDataNationalIDInfoNoLoad() == null) continue;

                if (nationalId == each.GetDataNationalIDInfoNoLoad().NationalID)
                    return true;
            }
            
            return false;
        }

        public async Task<bool> IsMemberExistsByPassport(string passportNumber)
        {
            while (_cachedUserData.Count == 0)
                await Task.Yield();

            foreach (IUserData each in _cachedUserData)
            {
                if (each.GetDataPassportInfoNoLoad() == null) continue;

                if (passportNumber == each.GetDataPassportInfoNoLoad().PassportNumber)
                    return true;
            }

            return false;
        }

        public async Task<bool> IsUserIdValid(string userId)
        {
            while (_cachedUserData.Count == 0)
                await Task.Yield();

            foreach (IUserData each in _cachedUserData)
            {
                if (userId == each.GetUserKeyID())
                {
                    _userData = each;
                    return true;
                }
            }

            _statusText.Show(_memberNotFound.GetLocalizedString(), _memberNotFoundColor);
            return false;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnMemberExistsPopupShowed()
        {
            _onMemberExistsPopupShowed?.Invoke();
        }

        public void OnUsedAppBeforePopupShowed()
        {
            _onUsedAppBeforePopupShowed?.Invoke();
        }

        public void OnCantFindIdPopupShowed()
        {
            _onCantFindIdPopupShowed?.Invoke();
        }

        public void OnBeforeConfirmPopupShowed()
        {
            _onBeforeConfirmPopupShowed?.Invoke();
        }

        public void OnNewAccountPopupShowed()
        {
            _onNewAccountPopupShowed?.Invoke();
        }

        public void StoreData(NationalIDInfo nationalIDInfo, PassportInfo passportInfo, GeneralInfo generalInfo)
        {
            if (IsCardReader || IsCardId)
                _nationalIDInfo = nationalIDInfo;
                
            if (IsPassport)
                _passportInfo = passportInfo;
            
            _generalInfo = generalInfo;
        }

        public void ResetData()
        {
            _nationalIDInfo = null;
            _passportInfo = null;
            _generalInfo = null;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async void UploadDataToServer()
        {            
            if (IsCardReader || IsCardId)
            {
                // Upload to Server -> 'User Themselves' National ID Info
                await _userData.SetDataNationalIDInfo(_nationalIDInfo.NationalID, _nationalIDInfo.Gender, _nationalIDInfo.Prefix, _nationalIDInfo.FirstName, _nationalIDInfo.LastName, _nationalIDInfo.BirthDate, _nationalIDInfo.IssueDate, _nationalIDInfo.ExpireDate, _nationalIDInfo.HouseNumber, _nationalIDInfo.Subdistrict, _nationalIDInfo.District, _nationalIDInfo.Province, _nationalIDInfo.Country);
            }

            if (IsPassport)
            {
                // Upload to Server -> 'User Themselves' Passport Info
                await _userData.SetDataPassportInfo(_passportInfo);
            }

            // Upload to Server -> 'User Themselves' General Info
            await _userData.SetDataGeneralInfo(_generalInfo.PhoneNumber, _generalInfo.MedicalCondition, _generalInfo.EmergencyContact.PhoneNumber, _generalInfo.EmergencyContact.Relation, _generalInfo.SocialAccounts.Line, _generalInfo.SocialAccounts.Facebook, _generalInfo.SocialAccounts.Instagram, _generalInfo.SocialAccounts.Tiktok);

            _statusText.Show(_uploadedSucceeded.GetLocalizedString(), _uploadedSucceededColor);
        }

        private async void CreateUserWithPhoneNumber()
        {
            string userId = await CreateUserByAdminAsync(_generalInfo.PhoneNumber);
            if (userId == null) return;

            // Assign new Created user with default data, other data like "Account Status or Point Cap" will be set once user log in.
            string firstName = "";
            string lastName = "";
            if (IsCardReader || IsCardId)
            {
                firstName = _nationalIDInfo.FirstName;
                lastName = _nationalIDInfo.LastName;
            }
            if (IsPassport)
            {
                firstName = _passportInfo.FullName;
                lastName = _passportInfo.FullName;
            }
            _savingWrapper.SaveToUser(userId, EValueNode.FirstName, firstName);
            _savingWrapper.SaveToUser(userId, EValueNode.LastName, lastName);
            DateTime nowDate = await _serverTime.Now();
            _savingWrapper.SaveToUser(userId, EValueNode.MemberSince, nowDate.ToGregorianString());
            _savingWrapper.SaveToUser(userId, EValueNode.Role, EUserRole.LayPeople.ToString());
            _savingWrapper.SaveToUser(userId, EValueNode.Title, EUserTitle.LayPeople.ToString());

            // Assign _userData with newly created one.
            DataSnapshot dataSnapShot = await _savingWrapper.LoadOtherUser(userId);
            if (dataSnapShot == null) return;
            _userData = new OtherUserData(dataSnapShot);

            UploadDataToServer();
        }

        private async Task<string> CreateUserByAdminAsync(string phoneNumber)
        {
            FirebaseFunctions functions = FirebaseFunctions.GetInstance("asia-southeast1");

            var data = new Dictionary<string, object>
            {
                { "phoneNumber", phoneNumber }
            };
            try
            {
                HttpsCallableReference function = functions.GetHttpsCallable("createUserByAdmin");

                HttpsCallableResult result = await function.CallAsync(data);
                var resultData = (IDictionary<object, object>)result.Data;
                string newUserUid = resultData["uid"].ToString();

                _statusText.Show(_accountCreatedSucceeded.GetLocalizedString(phoneNumber), _accountCreatedSucceededColor);
                return newUserUid;
            }
            catch (Exception e)
            {
                Debug.LogError($"CreateUserByAdmin failed: {e}");
                _statusText.Show(_accountCreatedFailed.GetLocalizedString(), _accountCreatedFailedColor);
                return null;
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void OnUsedAppBeforePopupCanceled()
        {
            // YES - go back
            HasUserId = EHasUserId.HasNotSure;
            OnRefreshUI.Invoke();
        }

        public void OnUsedAppBeforePopupConfirmed()
        {
            // NO - proceed to next Panel
            HasUserId = EHasUserId.NotHas;
            CurPanel.Push(ERegisterPanel.PanelFill);
        }

        public void OnCantFindIdPopupCanceled()
        {
            // LET ME CHECK - go back
            HasUserId = EHasUserId.HasNotSure;
            OnRefreshUI.Invoke();
        }

        public void OnCantFindIdPopupConfirmed()
        {
            // NO ID - proceed to next Panel
            HasUserId = EHasUserId.NotHas;
            CurPanel.Push(ERegisterPanel.PanelFill);
        }

        public void OnBeforeConfirmPopupCanceled()
        {
            // Cancel - go back
            // No need to do anything here...
        }

        public void OnBeforeConfirmPopupConfirmed()
        {
            // Confirm - proceed upload to server

            if (HasUserId == EHasUserId.HasForSure)
            {
                UploadDataToServer();
            }
            else if (HasUserId == EHasUserId.NotHas)
            {
                CreateUserWithPhoneNumber();
            }
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        public class ObservableStack<T>
        {
            private readonly Stack<T> _stack = new();

            public event Action OnChanged;

            public int Count => _stack.Count;

            public void Push(T item)
            {
                _stack.Push(item);
                OnChanged?.Invoke();
            }

            public T Pop()
            {
                T item = _stack.Pop();
                OnChanged?.Invoke();
                return item;
            }

            public T Peek()
            {
                return _stack.Peek();
            }

            public void Clear()
            {
                _stack.Clear();
                OnChanged?.Invoke();
            }
        }
        #endregion
    }
}