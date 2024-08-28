namespace WatKhaoWong.Utils.Conditions
{
    /// <summary>
    /// Instead of using 'Admin' we do 'RoleAdmin' so that it is clearer on Editor Inspector to choose and see.
    /// BUT there is a problem when we do 'Enum.Parse()' we have to substring first.
    /// In this case get amount to substring from 'EConditionType'. Check 'MyUserData.cs' for code example.
    /// </summary>
    public enum EConditionValue
    {
        None,
        RoleAdmin,
        RoleMember,
        RoleGuest,
        LeaderboardCategoryAllTime,
        LeaderboardCategoryToday,
        LeaderboardCategoryChallenge
        // More Value here... - check RPG project for example could also be number like 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, ...
    }
}