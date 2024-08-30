namespace WatKhaoWong.SceneManagement
{
    /// <summary>
    /// MOST Outer Root Node on Firebase JSON Tree Structure
    /// ***** IMPORTANT!!! When Add new enum also Add new 'Switch() Case' at 'SavingWrapper.cs' *****
    /// </summary>
    public enum ECategoryNode
    {
        Users,
        LeaderboardStats,
        LeaderboardTMToday
    }
}