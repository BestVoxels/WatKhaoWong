namespace WatKhaoWong.Utils.Conditions
{
    /// <summary>
    /// Assign Number to indicates substring amount for 'Types' under 'EConditionValue'.
    /// 
    /// Ex 1) 'RoleAdmin' before we can do 'Enum.Parse()' we have to get proper string from 'RoleAdmin' first.
    ///        So use number '4' to start 'RoleAdmin' string, now we get 'Admin' so we can now do 'Enum.Parse()'
    /// Ex 2) 'LeaderboardCategoryAllTime' before we can do 'Enum.Parse()' we have to get proper string from 'LeaderboardCategoryAllTime' first.
    ///        So use number '19' to start 'LeaderboardCategoryAllTime' string, now we get 'Admin' so we can now do 'Enum.Parse()'
    ///
    /// Check 'MyUserData.cs' for code example.
    /// </summary>
    public enum EConditionType : byte
    {
        // THESE are ONLY for 'MyUser' since 'OtherUser' can't take into processing since 'OtherUser' doesn't attached on any GameObject in Scene.
        IsAuthenticated,
        IsChallengeStatusEquals = 15,
        IsRoleEquals = 4,
        IsLeaderboardCategoryEquals = 19,
        IsLeaderboardExists,
        HasAllTimeTMPoint,
        HasTodayTMPoint,
        HasChallengeTMPoint,
        HasChallengeTMWon,
        AllowAccountDeletion

        //// ---DEBUGGER PURPOSE--- search for 'EConditionType.cs | MyUserData.cs | ShowHideUIByCondition.cs'
        //True,
        //False
    }
}