using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Utils;

namespace WatKhaoWong.Retreats
{
    public class AccommodationForm : Page
    {
        #region --Fields-- (Inspector)
        [Header("Accommodation Form Status Text")]
        [SerializeField] private LocalizedString _statusSucceeded;
        [SerializeField] private Color32 _statusSucceededColor;
        [Space]
        [Header("For Printing Stuff")]
        [SerializeField] private LocalizedString _noData;
        [SerializeField] private LocalizedString _nationalIdType;
        [SerializeField] private LocalizedString _passportType;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Accommodation Form - Status Text")]
        [field: SerializeField] public LocalizedString StatusMustBeFilled { get; private set; }
        [field: SerializeField] public Color32 StatusMustBeFilledColor { get; private set; }

        [field: Header("Accommodation Form - Default Text to show when no Data")]
        [field: SerializeField] public LocalizedString NoDataText { get; private set; }

        [field: Header("Accommodation Form - Day Format on Button")]
        [field: SerializeField] public string DayFormat { get; private set; } = "d/M/yyyy";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Accommodation Form UI Event")]
        [SerializeField] private UnityEvent _onSetTimeButtonClick;
        [SerializeField] private UnityEvent _onValidateTextSucceeded;
        [SerializeField] private UnityEvent _onValidateTextFailed;
        [Space]
        [SerializeField] private UnityEvent _onPrintButtonClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<StayEntry, EStayStatus?> OnUploadedToServer;
        #endregion



        #region --Fields-- (In Class)
        private byte _activityIndex;
        private SetTimeData _setTimeData;
        private EHasCar _hasCar;
        private string _plateNumber;

        private MyUserData _myUserData;
        private AccommodationSetTimePopup _setTimePopup;
        private Localizer _localizer;
        private SavingWrapper _savingWrapper;
        private ServerTime _serverTime;
        private StatusText _statusText;
        private A4DocumentGenerator _a4DocumentGenerator;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _setTimePopup = player.GetComponentInChildren<AccommodationSetTimePopup>();
            
            _localizer = FindAnyObjectByType<Localizer>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _serverTime = FindAnyObjectByType<ServerTime>();
            _statusText = FindAnyObjectByType<StatusText>();
            _a4DocumentGenerator = FindAnyObjectByType<A4DocumentGenerator>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnSetTimeButtonClick()
        {
            _onSetTimeButtonClick?.Invoke();
        }

        public void OnValidateSucceeded(byte activityIndex, SetTimeData setTimeData, EHasCar hasCar, string plateNumber)
        {
            _activityIndex = activityIndex;
            _setTimeData = setTimeData;
            _hasCar = hasCar;
            _plateNumber = FilterPlateNumber(hasCar, plateNumber);
            
            _onValidateTextSucceeded?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateTextFailed?.Invoke();
        }

        public void OnPrintButtonClick()
        {
            _onPrintButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private string FilterPlateNumber(EHasCar hasCar, string plateNumber)
        {
            if (hasCar == EHasCar.None)
                return null;

            return plateNumber;
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public async void UploadToServer()
        {
            DateTime nowDate = await _serverTime.Now();

            StayEntry stayEntry = new StayEntry()
            {
                UserId = FirebaseUtils.CurrentUserID,
                Activity = ((EActivityType)_activityIndex).ToString(),
                StayInfo = new StayInfo()
                {
                    IsStaying = _setTimeData.isStayingOvernight.ToString(),
                    StartDate = _setTimeData.startDate.ToGregorianString(),
                    EndDate = _setTimeData.endDate.ToGregorianString()
                },
                Transportation = new Transportation()
                {
                    HasCar = _hasCar.ToString(),
                    CarPlateNumber = _plateNumber
                },
                StatusInfo = new StatusInfo()
                {
                    Status = EStayStatus.Pending.ToString(), // This will always be 'Pending' because either 'Scheduled' or 'Active' Admin will decide on another UI Page.
                    StatusUpdatedAt = nowDate.ToGregorianString()
                },
                Reputation = EReputation.Normal.ToString()
            };

            // Upload to Server -> 'Stay Requests' Category
            string keyId = await _savingWrapper.SaveDataWithKey(ECategoryNode.StayRequests, stayEntry);

            // Upload to Server -> 'User Themselves' Active Stay
            ActiveStay activeStay = new ActiveStay()
            {
                KeyId = keyId,
                StatusInfo = new StatusInfo()
                {
                    Status = EStayStatus.Pending.ToString(), // This will always be 'Pending' because either 'Scheduled' or 'Active' Admin will decide on another UI Page.
                    StatusUpdatedAt = nowDate.ToGregorianString()
                }
            };
            await _myUserData.SetDataActiveStay(activeStay);

            // Let Subscriber class use 'stayEntry' data to update UI
            OnUploadedToServer?.Invoke(stayEntry, EStayStatus.Pending);

            _statusText.Show(_statusSucceeded.GetLocalizedString(), _statusSucceededColor);
        }

        public async void SetUserDataToGenerateA4()
        {
            NationalIDInfo nationalIDInfo = await _myUserData.GetDataNationalIDInfo();
            PassportInfo passportInfo = await _myUserData.GetDataPassportInfo();
            GeneralInfo generalInfo = await _myUserData.GetDataGeneralInfo();

            string fullName = _myUserData.GetAllUserNameText(nationalIDInfo, passportInfo);

            string cardType = _noData.GetLocalizedString();
            string idNumber = _noData.GetLocalizedString();
            string expireDate = _noData.GetLocalizedString();
            string birthDate = _noData.GetLocalizedString();
            string address = _noData.GetLocalizedString(); // ONLY this doesn't have on passport
            if (passportInfo != null)
            {
                cardType = _passportType.GetLocalizedString();
                
                if (passportInfo.PassportNumber != null)
                    idNumber = passportInfo.PassportNumber;

                if (passportInfo.ExpireDate != null)
                    expireDate = passportInfo.ExpireDate;
                
                if (passportInfo.BirthDate != null)
                    birthDate = passportInfo.BirthDate;
            }
            // Priority This since it will override 'passport'
            if (nationalIDInfo != null)
            {
                cardType = _nationalIdType.GetLocalizedString();
                
                if (nationalIDInfo.NationalID != null)
                    idNumber = nationalIDInfo.NationalID;

                if (nationalIDInfo.ExpireDate != null)
                    expireDate = nationalIDInfo.ExpireDate;
                
                if (nationalIDInfo.BirthDate != null)
                    birthDate = nationalIDInfo.BirthDate;

                if (nationalIDInfo.HouseNumber != null && nationalIDInfo.Subdistrict != null && nationalIDInfo.District != null && nationalIDInfo.Province != null && nationalIDInfo.Country != null)
                    address = $"{nationalIDInfo.HouseNumber}, {nationalIDInfo.Subdistrict}, {nationalIDInfo.District}, {nationalIDInfo.Province}, {nationalIDInfo.Country}";
            }

            string age = _localizer.FormatAge(_myUserData.GetAge(nationalIDInfo, passportInfo, _serverTime));
            
            string phoneNumber = (generalInfo.PhoneNumber == null) ? _noData.GetLocalizedString() : generalInfo.PhoneNumber;
            string medicalCondition = (generalInfo.MedicalCondition == null) ? _noData.GetLocalizedString() : generalInfo.MedicalCondition;
            string urgentPhoneNumber = _noData.GetLocalizedString();
            string urgentRelation = _noData.GetLocalizedString();
            if (generalInfo.EmergencyContact != null && generalInfo.EmergencyContact.PhoneNumber != null)
                urgentPhoneNumber = generalInfo.EmergencyContact.PhoneNumber;
            if (generalInfo.EmergencyContact != null && generalInfo.EmergencyContact.Relation != null)
                urgentRelation = generalInfo.EmergencyContact.Relation;
                
            string line = _noData.GetLocalizedString();
            string facebook = _noData.GetLocalizedString();
            string ig = _noData.GetLocalizedString();
            string tiktok = _noData.GetLocalizedString();
            if (generalInfo.SocialAccounts != null && generalInfo.SocialAccounts.Line != null)
                line = generalInfo.SocialAccounts.Line;
            if (generalInfo.SocialAccounts != null && generalInfo.SocialAccounts.Facebook != null)
                facebook = generalInfo.SocialAccounts.Facebook;
            if (generalInfo.SocialAccounts != null && generalInfo.SocialAccounts.Instagram != null)
                ig = generalInfo.SocialAccounts.Instagram;
            if (generalInfo.SocialAccounts != null && generalInfo.SocialAccounts.Tiktok != null)
                tiktok = generalInfo.SocialAccounts.Tiktok;

            StayEntry stayEntry = await _myUserData.GetActiveStayEntry();
            string building = ""; // Empty string wait for Admin to put by pen.
            string roomNumber = ""; // Empty string wait for Admin to put by pen.
            string stayDays = _noData.GetLocalizedString();
            string plateNumber = _noData.GetLocalizedString();
            if (stayEntry != null)
            {
                if (stayEntry.RoomInfo != null && stayEntry.RoomInfo.BuildingName != null)
                    building = _localizer.LocalizeBuildingName(stayEntry.RoomInfo.BuildingName);
                if (stayEntry.RoomInfo != null && stayEntry.RoomInfo.RoomNumber != null)
                    roomNumber = stayEntry.RoomInfo.RoomNumber;

                if (stayEntry.StayInfo != null)
                {
                    if (((EIsStaying)Enum.Parse(typeof(EIsStaying), stayEntry.StayInfo.IsStaying)) == EIsStaying.Staying)
                    {
                        stayEntry.StayInfo.StartDate.TryParseGregorian(out DateTime startDate);
                        stayEntry.StayInfo.EndDate.TryParseGregorian(out DateTime endDate);
                        stayDays = _setTimePopup.FormatDurationString(_setTimePopup.GetDuration(startDate, endDate));
                    }
                    else
                    {
                        stayDays = _setTimePopup.FormatDurationString(new TimeSpan(1, 0, 0, 0)); // 1 day
                        building = _noData.GetLocalizedString(); // no data cuz it is 1 day
                        roomNumber = _noData.GetLocalizedString(); // no data cuz it is 1 day
                    }
                }

                if (stayEntry.Transportation != null && ((EHasCar)Enum.Parse(typeof(EHasCar), stayEntry.Transportation.HasCar)) == EHasCar.Has)
                    plateNumber = stayEntry.Transportation.CarPlateNumber;
            }

            _a4DocumentGenerator.SetUserData(fullName, cardType, idNumber, expireDate, birthDate, age, address,
            phoneNumber, medicalCondition, urgentPhoneNumber, urgentRelation,
            line, facebook, ig, tiktok,
            building, roomNumber, stayDays, plateNumber);

            // _a4DocumentGenerator.SetNationalIdSprite(); // TODO

            _a4DocumentGenerator.GenerateA4();
        }
        #endregion
    }
}