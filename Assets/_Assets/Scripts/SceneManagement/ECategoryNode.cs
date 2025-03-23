namespace WatKhaoWong.SceneManagement
{
    /// <summary>
    /// MOST Outer Root Node on Firebase JSON Tree Structure
    /// ***** IMPORTANT!!! When Add new enum also Add new 'Switch() Case' at 'SavingWrapper.cs' *****
    /// 
    /// **Share Category**
    /// - means Many Users use this Node to load data SO it needs to get updates when any user open app from background. Check Reason why at 'Leaderboard.cs/OnApplicationFocus()'
    /// </summary>
    public enum ECategoryNode
    {
        Users,
        LeaderboardStats, // Share Category
        LeaderboardTMToday,
        LeaderboardTMChallenge,
        LeaderboardTMChallengeWinner,
        ServerStats, // Share Category
        RemoteConfig // Share Category
    }
}