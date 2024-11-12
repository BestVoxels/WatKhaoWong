using Firebase.Database;
using System;
using WatKhaoWong.CoreItems;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;

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
        #endregion



        #region --Fields-- (Constant)
        private const string DefaultProfileIconID = "ffa11251-7731-400e-94ec-ef2c11e177bc"; // 'Character Empty' Item
        #endregion



        #region --Constructors-- (PUBLIC)
        public OtherUserData(DataSnapshot bigData)
        {
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
        }
        #endregion



        #region --Methods-- (Interface) ~Getter~
        public string GetUserNameText() => _data.GetUserNameText();

        public string GetMemberSinceText() => _data.GetMemberSinceText();

        public ProfileIconItem GetProfileIcon()
        {
            if (_data.ProfileIcon == null)
                _data.ProfileIcon = BaseItem.GetFromID(DefaultProfileIconID.ToString()) as ProfileIconItem;

            return _data.GetProfileIcon();
        }

        public EUserRole GetRole() => _data.GetRole();

        public string GetLevelText() => _data.GetLevelText();

        public string GetTotalTMPointsText() => _data.GetTotalTMPointsText();

        public string GetTodayTMPointsText() => _data.GetTodayTMPointsText();

        public string GetChallengeTMPointsText() => _data.GetChallengeTMPointsText();

        public string GetTotalChallengeTMWonText() => _data.GetTotalChallengeTMWonText();

        public int GetTotalTMPoints() => _data.TotalTMPoints;

        public int GetTodayTMPoints() => _data.TodayTMPoints;

        public int GetChallengeTMPoints() => _data.ChallengeTMPoints;

        public int GetTotalChallengeTMWon() => _data.TotalChallengeTMWon;
        #endregion



        #region --Methods-- (Interface)
        public void UpdateProfileIcon(ProfileIconInspector oldUI, ProfileIconItem newIcon, float multiplierRatioForDecorator)
        {
            _data.UpdateProfileIcon(oldUI, newIcon, multiplierRatioForDecorator);
        }
        #endregion
    }
}