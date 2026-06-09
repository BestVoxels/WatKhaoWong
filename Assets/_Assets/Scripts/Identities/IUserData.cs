using System;
using System.Threading.Tasks;
using WatKhaoWong.Attributes;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Localization;

namespace WatKhaoWong.Identities
{
    /// <summary>
    /// --NOTE--
    /// 'MyUserData.cs' & 'OtherUserData.cs' implement interface 'IUserData.cs' to use Polymorphism concept, so both classes can be under 'IUserData.cs'.
    /// --------
    /// </summary>
    public interface IUserData
    {
        // Legacy  ~GETTER/UPDATER~
        public string GetUserKeyID();

        public string GetUserNameText();

        public string GetMemberSinceText();

        public EUserRole GetRole();

        public string GetTitleText();

        public string GetLevelText();

        public string GetTotalTMPointsText();

        public string GetTodayTMPointsText();

        public string GetChallengeTMPointsText();

        public string GetTotalChallengeTMWonText();

        public int GetTotalTMPoints();

        public int GetTodayTMPoints();

        public int GetChallengeTMPoints();

        public int GetTotalChallengeTMWon();

        public int GetTMPointCapRequest();

        public int GetTMPointCap();

        public int GetTMPointCapRound();

        public bool GetIsCustomTMPointCap();

        public ProfileIconItem GetProfileIcon();
        public void UpdateProfileIcon(ProfileIconInspector oldUI, ProfileIconItem newIcon, float multiplierRatioForDecorator);

        // Legacy  ~SETTER~
        public void SaveProfileIcon(ProfileIconItem input);



        // Meditation Retreat   ~GETTER/UPDATER~
        public bool GetTempleGuideConfirmed();

        public Task<StayEntry> GetActiveStayEntry();

        public Task<ActiveStay> GetDataActiveStay();

        public Task<NationalIDInfo> GetDataNationalIDInfo();

        public Task<PassportInfo> GetDataPassportInfo();

        public Task<GeneralInfo> GetDataGeneralInfo();

        public AccountStatus GetAccountStatus();
        public void UpdateAccountStatus(AccountStatusInspector oldStatus, AccountStatus newStatus, Localizer localizer);

        public void UpdateMiniInfo(MiniInfoInspector miniInfoInspector, NationalIDInfo nationalIDInfo, PassportInfo passportInfo, Localizer localizer, ServerTime serverTime);

        // Meditation Retreat  ~SETTER~
        public Task SetDataActiveStay(ActiveStay activeStay);

        public Task SetDataNationalIDInfo(string id = null, string gd = null, string pf = null, string fName = null, string lName = null, string bDate = null, string iDate = null, string eDate = null, string hN = null, string subd = null, string d = null, string p = null, string c = null);

        public Task SetDataPassportInfo(PassportInfo passportInfo);

        public Task SetDataGeneralInfo(string pN = null, string mC = null, string uPN = null, string r = null, string l = null, string fb = null, string ig = null, string tt = null);

        public Task SetDataAccountStatus(bool updateCheckinAt, EAccountStatus? eStatus = null, DateTime? banEndDate = null, string notesText = null, string notesColor = null);

        // Meditation Retreat  ~Utilities~
        public void DeleteStayEntry();

        public bool IsActiveStayExists();
        public void DeleteActiveStay();
    }
}