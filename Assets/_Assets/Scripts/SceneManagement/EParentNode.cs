namespace WatKhaoWong.SceneManagement
{
    /// <summary>
    /// ***** IMPORTANT!!! When Add new enum also Add new 'Switch() Case' at 'SavingWrapper.cs' *****
    /// </summary>
    public enum EParentNode
    {
        Progression,
        Stats,
        TMPoints,
        AccountStatus,
        Agreement,
        GeneralInfo,    // use 'DataNode' class with '.SetRawJsonValueAsync()'
        NationalIDInfo,    // use 'DataNode' class with '.SetRawJsonValueAsync()'
        PassportInfo,    // use 'DataNode' class with '.SetRawJsonValueAsync()'
        ActiveStay,    // use 'DataNode' class with '.SetRawJsonValueAsync()'
        PastStay,    // use 'DataNode' class with '.SetRawJsonValueAsync()'
        Images
    }
}