namespace WatKhaoWong.SceneManagement
{
    /// <summary>
    /// ***** IMPORTANT!!! When Add new enum also Add new 'Switch() Case' at 'SavingWrapper.cs' *****
    ///
    /// use with '.SetValueAsync()' not like the one in 'DataNode' class that has to use with '.SetRawJsonValueAsync()'
    /// </summary>
    public enum EValueNode
    {
        FirstName,
        LastName,
        MemberSince,
        ProfileIconID,
        Role,
        Title,
        Level,
        XP,
        TodayTMPoint,
        TotalTMPoint,
        ChallengeTMPoint,
        ChallengeTMWon,
        FirstUploadTimeOfDayTM,
        FirstUploadTimeOfChallengeTM,
        ChallengeTMStartDate,
        ChallengeTMEndDate,
        ChallengeTMDuration,
        ChallengeTMStatus,
        TimeStamp,
        LiveAppVersioniOS,
        LiveAppVersionAndroid,
        InReviewAppVersioniOS,
        InReviewAppVersionAndroid,
        LinkToUpdateAppiOS,
        LinkToUpdateAppAndroid,
        TMPointCapRequest,
        TMPointCap,
        IsCustomTMPointCap,
        TMPointCapRound,
        TMPointCapForAdmin,
        TMPointCapForPhra,
        TMPointCapForDhammaForces,
        TMPointCapForDhammaPractitioner,
        TMPointCapForLayPeople,
        AllowAccountDeletion,
        State,
        TempleGuideConfirmed,
        TempleGuideConfirmedAt,
        KeyId
    }
}