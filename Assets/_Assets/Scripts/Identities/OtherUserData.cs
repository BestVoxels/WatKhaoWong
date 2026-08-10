using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using WatKhaoWong.CoreItems;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Identities
{
    /// <summary>
    /// --NOTE--
    /// Can't use Inheritance because 'MyUserData.cs' MUST inherit from Monobehavior BUT 'OtherUserData.cs' MUST NOT inherit from Monobehavior.
    /// SO have to use Composition for 'MyUserData.cs' & 'OtherUserData.cs' instead of Inheritance (which is to avoid over Composition anyways).
    /// ALSO 'MyUserData.cs' & 'OtherUserData.cs' implement interface 'IUserData.cs' to use Polymorphism concept, so both classes can be under 'IUserData.cs'.
    /// --------
    /// </summary>
    public class OtherUserData : IUserData
    {
        #region --Fields-- (In Class)
        private readonly Data _data = new Data();
        private readonly string _userKeyID;

        private SavingWrapper _savingWrapper;
        private ServerTime _serverTime;
        #endregion



        #region --Fields-- (Constant)
        private const string DefaultProfileIconID = "ffa11251-7731-400e-94ec-ef2c11e177bc"; // 'Character Empty' Item
        #endregion



        #region --Methods-- (Legacy) ~GETTER/UPDATER~
        public string GetUserKeyID() => _userKeyID;

        public string GetUserNameText() => _data.GetUserNameText();

        public string GetMemberSinceText() => _data.GetMemberSinceText();

        public EUserRole GetRole() => _data.GetRole();

        public string GetTitleText() => _data.GetTitleText();

        public string GetLevelText() => _data.GetLevelText();

        public string GetTotalTMPointsText() => _data.GetTotalTMPointsText();

        public string GetTodayTMPointsText() => _data.GetTodayTMPointsText();

        public string GetChallengeTMPointsText() => _data.GetChallengeTMPointsText();

        public string GetTotalChallengeTMWonText() => _data.GetTotalChallengeTMWonText();

        public int GetTotalTMPoints() => _data.TotalTMPoints;

        public int GetTodayTMPoints() => _data.TodayTMPoints;

        public int GetChallengeTMPoints() => _data.ChallengeTMPoints;

        public int GetTotalChallengeTMWon() => _data.TotalChallengeTMWon;

        public int GetTMPointCapRequest() => _data.TMPointCapRequest;

        public int GetTMPointCap() => _data.TMPointCap;

        public int GetTMPointCapRound() => _data.TMPointCapRound;

        public bool GetIsCustomTMPointCap() => _data.IsCustomTMPointCap;

        public DateTime GetFirstUploadTimeOfDayTM() => _data.FirstUploadTimeOfDayTM;

        public ProfileIconItem GetProfileIcon()
        {
            if (_data.ProfileIcon == null)
                _data.ProfileIcon = BaseItem.GetFromID(DefaultProfileIconID.ToString()) as ProfileIconItem;

            return _data.GetProfileIcon();
        }

        public void UpdateProfileIcon(ProfileIconInspector oldUI, ProfileIconItem newIcon, float multiplierRatioForDecorator)
        {
            _data.UpdateProfileIcon(oldUI, newIcon, multiplierRatioForDecorator);
        }
        #endregion



        #region --Methods-- (Legacy) ~SETTER~
        public void SaveProfileIcon(ProfileIconItem input)
        {
            _data.ProfileIcon = input;

            _savingWrapper.SaveToUser(_userKeyID, EValueNode.ProfileIconID, _data.ProfileIcon.ItemID);
        }
        #endregion



        #region --Constructors-- (Legacy) ~LOADER~
        public OtherUserData(DataSnapshot bigData)
        {
            _savingWrapper = UnityEngine.Object.FindAnyObjectByType<SavingWrapper>();
            _serverTime = UnityEngine.Object.FindAnyObjectByType<ServerTime>();

            _userKeyID = bigData.Key;

            var data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.FirstName)).Value;
            if (data != null)
                _data.FirstName = data.ToString();

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.LastName)).Value;
            if (data != null)
                _data.LastName = data.ToString();

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.MemberSince)).Value;
            if (data != null)
                if (data.ToString().TryParseGregorian(out DateTime result))
                    _data.MemberSince = result;

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.ProfileIconID)).Value;
            if (data != null)
                _data.ProfileIcon = BaseItem.GetFromID(data.ToString()) as ProfileIconItem;

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.Role)).Value;
            if (data != null)
                _data.Role = (EUserRole)Enum.Parse(typeof(EUserRole), data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.Title)).Value;
            if (data != null)
                _data.Title = data.ToString();

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.Level)).Value;
            if (data != null)
                _data.Level = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.TotalTMPoint)).Value;
            if (data != null)
                _data.TotalTMPoints = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.TodayTMPoint)).Value;
            if (data != null)
                _data.TodayTMPoints = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.ChallengeTMPoint)).Value;
            if (data != null)
                _data.ChallengeTMPoints = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.ChallengeTMWon)).Value;
            if (data != null)
                _data.TotalChallengeTMWon = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.TMPointCapRequest)).Value;
            if (data != null)
                _data.TMPointCapRequest = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.TMPointCap)).Value;
            if (data != null)
                _data.TMPointCap = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.IsCustomTMPointCap)).Value;
            if (data != null)
                _data.IsCustomTMPointCap = bool.Parse(data.ToString());

            // Belongs to Meditation Retreat data
            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.TempleGuideConfirmed)).Value;
            if (data != null)
                _data.TempleGuideConfirmed = bool.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(ECategoryNode.Users, EValueNode.FirstUploadTimeOfDayTM)).Value;
            if (data != null)
                if (data.ToString().TryParseGregorian(out DateTime result))
                    _data.FirstUploadTimeOfDayTM = result;

            string jsonData = bigData.Child(EParentNode.AccountStatus.ToString()).GetRawJsonValue();
            if (jsonData != null)
                _data.AccountStatus = JsonConvert.DeserializeObject<AccountStatus>(jsonData);
            
            jsonData = bigData.Child(EParentNode.ActiveStay.ToString()).GetRawJsonValue();
            if (jsonData != null)
                _data.ActiveStay = JsonConvert.DeserializeObject<ActiveStay>(jsonData);

            jsonData = bigData.Child(EParentNode.NationalIDInfo.ToString()).GetRawJsonValue();
            if (jsonData != null)
                _data.NationalIDInfo = JsonConvert.DeserializeObject<NationalIDInfo>(jsonData);

            jsonData = bigData.Child(EParentNode.PassportInfo.ToString()).GetRawJsonValue();
            if (jsonData != null)
                _data.PassportInfo = JsonConvert.DeserializeObject<PassportInfo>(jsonData);
        }
        #endregion



        #region --Methods-- (Meditation Retreat) ~GETTER/UPDATER~
        public bool GetTempleGuideConfirmed() => _data.TempleGuideConfirmed;


        // -Stay Entry-
        public async Task<StayEntry> GetActiveStayEntry()
        {
            if (!IsStayEntryExists())
            {
                // ActiveStay activeStay = await GetDataActiveStay();
                if (!IsActiveStayExists()) return null;

                Enum.TryParse(_data.ActiveStay.StatusInfo.Status, true, out EStayStatus eStatus);
                switch (eStatus)
                {
                    case EStayStatus.Pending:
                        await LoadMyEntryFromStayRequests();
                        break;

                    case EStayStatus.Scheduled:
                        await LoadMyEntryFromScheduledStay();
                        break;

                    case EStayStatus.Active:
                        await LoadMyEntryFromActiveStay();
                        break;
                }
                
                if (!IsStayEntryExists()) return null; // Incase can't find my 'StayEntry' under 'StayRequests' Category
            }

            return _data.StayEntry;
        }
        public StayEntry GetActiveStayEntryNoLoad() => _data.StayEntry;


        // -Active Stay-
        public async Task<ActiveStay> GetDataActiveStay()
        {
            if (!IsActiveStayExists())
            {
                await LoadActiveStay();

                if (!IsActiveStayExists()) return null; // Incase can't find my 'ActiveStay' under MyUser Category
            }

            return _data.ActiveStay;
        }


        // -National ID-
        public async Task<NationalIDInfo> GetDataNationalIDInfo()
        {
            if (!IsNationalIDInfoExists())
            {
                await LoadNationalIDInfo();

                if (!IsNationalIDInfoExists()) return null; // Incase can't find my 'NationalIDInfo' under MyUser Category
            }

            return _data.NationalIDInfo;
        }
        public NationalIDInfo GetDataNationalIDInfoNoLoad() => _data.NationalIDInfo;


        // -Passport-
        public async Task<PassportInfo> GetDataPassportInfo()
        {
            if (!IsPassportInfoExists())
            {
                await LoadPassportInfo();

                if (!IsPassportInfoExists()) return null; // Incase can't find my 'PassportInfo' under MyUser Category
            }

            return _data.PassportInfo;
        }
        public PassportInfo GetDataPassportInfoNoLoad() => _data.PassportInfo;


        // -General Info-
        public async Task<GeneralInfo> GetDataGeneralInfo()
        {
            if (!IsGeneralInfoExists())
            {
                await LoadGeneralInfo();

                if (!IsGeneralInfoExists()) return null; // Incase can't find my 'GeneralInfo' under MyUser Category
            }

            return _data.GeneralInfo;
        }


        // -Account Status-
        public AccountStatus GetAccountStatus() => _data.GetAccountStatus();

        public void UpdateAccountStatus(AccountStatusInspector oldStatus, AccountStatus newStatus, Localizer localizer)
        {
            _data.UpdateAccountStatus(oldStatus, newStatus, localizer);
        }


        // -Mini Info-
        public void UpdateMiniInfo(MiniInfoInspector miniInfoInspector, NationalIDInfo nationalIDInfo, PassportInfo passportInfo, Localizer localizer, ServerTime serverTime)
        {
            _data.UpdateMiniInfo(miniInfoInspector, nationalIDInfo, passportInfo, localizer, serverTime);
        }


        // -More Info-
        public string GetAllUserNameText(NationalIDInfo nationalIDInfo, PassportInfo passportInfo)
        {
            return _data.GetAllUserNameText(nationalIDInfo, passportInfo);
        }

        public string GetAllUserNameTextCombined(NationalIDInfo nationalIDInfo, PassportInfo passportInfo)
        {
            return _data.GetAllUserNameTextCombined(nationalIDInfo, passportInfo);
        }

        public int GetAge(NationalIDInfo nationalIDInfo, PassportInfo passportInfo, ServerTime serverTime)
        {
            return _data.GetAge(nationalIDInfo, passportInfo, serverTime);
        }

        public string GetNationalIDAndPassportNumberCombined(NationalIDInfo nationalIDInfo, PassportInfo passportInfo)
        {
            return _data.GetNationalIDAndPassportNumberCombined(nationalIDInfo, passportInfo);
        }

        public string GetPlateNumberFromActiveStayEntry()
        {
            return _data.GetPlateNumber(GetActiveStayEntryNoLoad());
        }

        public string GetBuildingNameFromActiveStayEntry(Localizer localizer)
        {
            return _data.GetBuildingName(GetActiveStayEntryNoLoad(), localizer);
        }

        public string GetRoomNumberFromActiveStayEntry()
        {
            return _data.GetRoomNumber(GetActiveStayEntryNoLoad());
        }

        public string GetAccountStatusTextCombined(Localizer localizer)
        {
            return _data.GetAccountStatusTextCombined(GetAccountStatus(), localizer);
        }
        #endregion



        #region --Methods-- (Meditation Retreat) ~SETTER~
        // -Active Stay-
        /// <summary>
        /// NOT Allow Partial Adding, NEED all Info at once
        /// This is WHY parameter is using its Class as a Group.
        /// </summary>
        public async Task SetDataActiveStay(ActiveStay activeStay)
        {
            _data.ActiveStay = activeStay;

            await _savingWrapper.SaveDataToUser(_userKeyID, EParentNode.ActiveStay, _data.ActiveStay);
        }

        // -National ID-
        /// <summary>
        /// Allow Partial Adding, no need to have all Info at once, can add partial
        /// This is WHY parameter is one by one like this.
        /// </summary>
        public async Task SetDataNationalIDInfo(string id = null, string gd = null, string pf = null, string fName = null, string lName = null, string bDate = null, string iDate = null, string eDate = null, string hN = null, string subd = null, string d = null, string p = null, string c = null)
        {
            if (!IsNationalIDInfoExists()) await LoadNationalIDInfo();

            NationalIDInfo old = (_data.NationalIDInfo == null) ? new NationalIDInfo() : _data.NationalIDInfo;
            string nId = (id == null) ? old.NationalID : id;
            string gender = (gd == null) ? old.Gender : gd;
            string prefix = (pf == null) ? old.Prefix : pf;
            string firstName = (fName == null) ? old.FirstName : fName;
            string lastName = (lName == null) ? old.LastName : lName;
            string birthDate = (bDate == null) ? old.BirthDate : bDate;
            string issueDate = (iDate == null) ? old.IssueDate : iDate;
            string expireDate = (eDate == null) ? old.ExpireDate : eDate;
            string houseNumber = (hN == null) ? old.HouseNumber : hN;
            string subDistrict = (subd == null) ? old.Subdistrict : subd;
            string district = (d == null) ? old.District : d;
            string province = (p == null) ? old.Province : p;
            string country = (c == null) ? old.Country : c;

            _data.NationalIDInfo = new NationalIDInfo()
            {
                NationalID = nId,
                Gender = gender,
                Prefix = prefix,
                FirstName = firstName,
                LastName = lastName,
                BirthDate = birthDate,
                IssueDate = issueDate,
                ExpireDate = expireDate,
                HouseNumber = houseNumber,
                Subdistrict = subDistrict,
                District = district,
                Province = province,
                Country = country
            };

            await _savingWrapper.SaveDataToUser(_userKeyID, EParentNode.NationalIDInfo, _data.NationalIDInfo);
        }


        // -Passport-
        /// <summary>
        /// NOT Allow Partial Adding, NEED all Info at once
        /// This is WHY parameter is using its Class as a Group.
        /// </summary>
        public async Task SetDataPassportInfo(PassportInfo passportInfo)
        {
            _data.PassportInfo = passportInfo;

            await _savingWrapper.SaveDataToUser(_userKeyID, EParentNode.PassportInfo, _data.PassportInfo);
        }

        // -General Info-
        /// <summary>
        /// Allow Partial Adding, no need to have all Info at once, can add partial
        /// This is WHY parameter is one by one like this.
        /// </summary>
        public async Task SetDataGeneralInfo(string pN = null, string mC = null, string uPN = null, string r = null, string l = null, string fb = null, string ig = null, string tt = null)
        {
            if (!IsGeneralInfoExists()) await LoadGeneralInfo();

            GeneralInfo old = (_data.GeneralInfo == null) ? new GeneralInfo() : _data.GeneralInfo;
            string phoneNumber = (pN == null) ? old.PhoneNumber : pN;
            string medical = (mC == null) ? old.MedicalCondition : mC;

            EmergencyContact oldEmergencyContact = (old.EmergencyContact == null) ? new EmergencyContact() : old.EmergencyContact;
            string urgentPhoneNumber = (uPN == null) ? oldEmergencyContact.PhoneNumber : uPN;
            string relation = (r == null) ? oldEmergencyContact.Relation : r;

            SocialAccounts oldSocialAccounts = (old.SocialAccounts == null) ? new SocialAccounts() : old.SocialAccounts;
            string line = (l == null) ? oldSocialAccounts.Line : l;
            string facebook = (fb == null) ? oldSocialAccounts.Facebook : fb;
            string instagram = (ig == null) ? oldSocialAccounts.Instagram : ig;
            string tiktok = (tt == null) ? oldSocialAccounts.Tiktok : tt;

            _data.GeneralInfo = new GeneralInfo()
            {
                PhoneNumber = phoneNumber,
                MedicalCondition = medical,
                EmergencyContact = new EmergencyContact()
                {
                    PhoneNumber = urgentPhoneNumber,
                    Relation = relation
                },
                SocialAccounts = new SocialAccounts()
                {
                    Line = line,
                    Facebook = facebook,
                    Instagram = instagram,
                    Tiktok = tiktok
                }
            };

            await _savingWrapper.SaveDataToUser(_userKeyID, EParentNode.GeneralInfo, _data.GeneralInfo);
        }


        // -Account Status-
        /// <summary>
        /// Allow Partial Adding, no need to have all Info at once, can add partial
        /// This is WHY parameter is one by one like this.
        /// </summary>
        public async Task SetDataAccountStatus(bool updateCheckinAt, EAccountStatus? eStatus = null, DateTime? banEndDate = null, string notesText = null, string notesColor = null)
        {
            if (!IsAccountStatusExists()) await LoadAccountStatus();

            DateTime nowDate = await _serverTime.Now();

            AccountStatus oldStatus = (_data.AccountStatus == null) ? new AccountStatus() : _data.AccountStatus;
            string lastCheckinAtText = updateCheckinAt ? nowDate.ToGregorianString() : oldStatus.LastCheckinAt;
            string banEndDateText = (banEndDate == null) ? oldStatus.BanEndDate : banEndDate.ToGregorianString();

            StatusInfo oldStatusInfo = (oldStatus.StatusInfo == null) ? new StatusInfo() : oldStatus.StatusInfo;
            string statusText = (eStatus == null) ? oldStatusInfo.Status : eStatus.ToString();
            string statusUpdatedAtText = (eStatus == null || (oldStatusInfo.Status == eStatus.ToString() && oldStatusInfo.StatusUpdatedAt != null)) ? oldStatusInfo.StatusUpdatedAt : nowDate.ToGregorianString();

            NotesInfo oldNotesInfo = (oldStatus.NotesInfo == null) ? new NotesInfo() : oldStatus.NotesInfo;
            string notesInfoText = (notesText == null) ? oldNotesInfo.Text : notesText;
            string notesInfoColor = (notesColor == null) ? oldNotesInfo.Color : notesColor;

            _data.AccountStatus = new AccountStatus()
            {
                LastCheckinAt = lastCheckinAtText,
                StatusInfo = new StatusInfo()
                {
                    Status = statusText,
                    StatusUpdatedAt = statusUpdatedAtText
                },
                BanEndDate = banEndDateText,
                NotesInfo = new NotesInfo()
                {
                    Text = notesInfoText,
                    Color = notesInfoColor
                }
            };

            await _savingWrapper.SaveDataToUser(_userKeyID, EParentNode.AccountStatus, _data.AccountStatus);
        }
        #endregion



        #region --Methods-- (Meditation Retreat) ~Utilities~
        // -Stay Entry-
        public bool IsStayEntryExists() => !(_data.StayEntry == null);

        public void DeleteStayEntry() => _data.StayEntry = null;


        // -Active Stay-
        public bool IsActiveStayExists() => !(_data.ActiveStay == null);

        public void DeleteActiveStay() => _data.ActiveStay = null;


        // -National ID-
        public bool IsNationalIDInfoExists() => !(_data.NationalIDInfo == null);
        // -Passport-
        public bool IsPassportInfoExists() => !(_data.PassportInfo == null);
        // -General Info-
        public bool IsGeneralInfoExists() => !(_data.GeneralInfo == null);


        // -Account Status-
        public bool IsAccountStatusExists() => !(_data.AccountStatus == null);
        #endregion



        #region --Methods-- (Meditation Retreat) ~LOADER~
        // -Stay Entry-
        private async Task LoadMyEntryFromStayRequests()
        {
            if (_savingWrapper == null) await Task.Yield();
            
            _data.StayEntry = await _savingWrapper.LoadUserEntryFromStayRequests(_userKeyID);
        }
        private async Task LoadMyEntryFromScheduledStay()
        {
            if (_savingWrapper == null) await Task.Yield();

            _data.StayEntry = await _savingWrapper.LoadUserEntryFromScheduledStay(_userKeyID);
        }
        private async Task LoadMyEntryFromActiveStay()
        {
            if (_savingWrapper == null) await Task.Yield();

            _data.StayEntry = await _savingWrapper.LoadUserEntryFromActiveStay(_userKeyID);
        }


        // -Active Stay-
        private async Task LoadActiveStay()
        {
            if (_savingWrapper == null) await Task.Yield();

            _data.ActiveStay = await _savingWrapper.LoadDataFromUser<ActiveStay>(_userKeyID, EParentNode.ActiveStay);
        }


        // -National ID-
        private async Task LoadNationalIDInfo()
        {
            if (_savingWrapper == null) await Task.Yield();

            _data.NationalIDInfo = await _savingWrapper.LoadDataFromUser<NationalIDInfo>(_userKeyID, EParentNode.NationalIDInfo);
        }
        // -Passport-
        private async Task LoadPassportInfo()
        {
            if (_savingWrapper == null) await Task.Yield();

            _data.PassportInfo = await _savingWrapper.LoadDataFromUser<PassportInfo>(_userKeyID, EParentNode.PassportInfo);
        }
        // -General Info-
        private async Task LoadGeneralInfo()
        {
            if (_savingWrapper == null) await Task.Yield();

            _data.GeneralInfo = await _savingWrapper.LoadDataFromUser<GeneralInfo>(_userKeyID, EParentNode.GeneralInfo);
        }


        // -Account Status-
        private async Task LoadAccountStatus()
        {
            _data.AccountStatus = await _savingWrapper.LoadDataFromUser<AccountStatus>(_userKeyID, EParentNode.AccountStatus);
        }
        #endregion
    }
}