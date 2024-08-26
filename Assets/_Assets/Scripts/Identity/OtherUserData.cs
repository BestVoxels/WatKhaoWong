using Firebase.Database;
using System;
using WatKhaoWong.CoreItems;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.Identity
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



        #region --Constructors-- (PUBLIC)
        public OtherUserData(DataSnapshot bigData)
        {
            _data.FirstName = bigData.Child(SavingWrapper.GetValueNodePath(EValueNode.FirstName)).Value.ToString();

            _data.LastName = bigData.Child(SavingWrapper.GetValueNodePath(EValueNode.LastName)).Value.ToString();

            if (DateTime.TryParse(bigData.Child(SavingWrapper.GetValueNodePath(EValueNode.MemberSince)).Value.ToString(), out DateTime result))
                _data.MemberSince = result;

            string id = bigData.Child(SavingWrapper.GetValueNodePath(EValueNode.ProfileIconID)).Value.ToString();
            _data.ProfileIcon = BaseItem.GetFromID(id) as ProfileIconItem;

            string roleString = bigData.Child(SavingWrapper.GetValueNodePath(EValueNode.Role)).Value.ToString();
            if (roleString != null)
                _data.Role = (EUserRole)Enum.Parse(typeof(EUserRole), roleString);

            var data = bigData.Child(SavingWrapper.GetValueNodePath(EValueNode.Level)).Value;
            if (data != null)
                _data.Level = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(EValueNode.TotalTMPoint)).Value;
            if (data != null)
                _data.TotalTMPoints = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(EValueNode.TodayTMPoint)).Value;
            if (data != null)
                _data.TodayTMPoints = int.Parse(data.ToString());

            data = bigData.Child(SavingWrapper.GetValueNodePath(EValueNode.ChallengeWon)).Value;
            if (data != null)
                _data.TotalWonTMChallenge = int.Parse(data.ToString());
        }
        #endregion



        #region --Methods-- (Interface) ~Getter~
        public string GetUserNameText() => _data.GetUserNameText();

        public string GetMemberSinceText() => _data.GetMemberSinceText();

        public ProfileIconItem GetProfileIcon() => _data.GetProfileIcon();

        public EUserRole GetRole() => _data.GetRole();

        public string GetLevelText() => _data.GetLevelText();

        public string GetTotalTMPointsText() => _data.GetTotalTMPointsText();

        public string GetTodayTMPointsText() => _data.GetTodayTMPointsText();

        public string GetTotalWonTMChallengeText() => _data.GetTotalWonTMChallengeText();
        #endregion



        #region --Methods-- (Interface)
        public void UpdateProfileIcon(ProfileIconInspector oldUI, ProfileIconItem newIcon, float multiplierRatioForDecorator)
        {
            _data.UpdateProfileIcon(oldUI, newIcon, multiplierRatioForDecorator);
        }
        #endregion
    }
}