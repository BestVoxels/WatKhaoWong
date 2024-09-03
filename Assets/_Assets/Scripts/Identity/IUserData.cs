namespace WatKhaoWong.Identity
{
    /// <summary>
    /// --NOTE--
    /// 'MyUserData.cs' & 'OtherUserData.cs' implement interface 'IUserData.cs' to use Polymorphism concept, so both classes can be under 'IUserData.cs'.
    /// --------
    /// </summary>
    public interface IUserData
    {
        public string GetUserNameText();

        public string GetMemberSinceText();

        public ProfileIconItem GetProfileIcon();

        public EUserRole GetRole();

        public string GetLevelText();

        public string GetTotalTMPointsText();

        public string GetTodayTMPointsText();

        public string GetChallengeTMPointsText();

        public string GetTotalChallengeTMWonText();

        public void UpdateProfileIcon(ProfileIconInspector oldUI, ProfileIconItem newIcon, float multiplierRatioForDecorator);
    }
}