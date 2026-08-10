using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WatKhaoWong.Identities;
using WatKhaoWong.Admin;
using UnityEngine.Pool;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Retreats;

namespace WatKhaoWong.UI.Admin
{
    public class FoundRowUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Buttons Stuffs")]
        [SerializeField] private Button _rowButton;

        [Header("UI Stuffs")]
        [SerializeField] private TMP_Text _rankText;
        [SerializeField] private ProfileIconInspector _icon;
        [SerializeField] private MiniInfoInspector _miniInfoInspectorUI;
        [SerializeField] private AccountStatusInspector _accountStatusUI;
        #endregion



        #region --Fields-- (In Class)
        private FoundRow _row;
        private IUserData _userData;
        private IObjectPool<FoundRowUI> _rowUIPool;
        private Localizer _localizer;
        private ServerTime _serverTime;
        private UserInfo _userInfo;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 112f / 135f;  // Formula : [CHANGE THIS] RowUI Profile's Size  %  [FIX] Inventory Profile's Size (original looks)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _rowButton.onClick.AddListener(RowClick);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void OnCreatedByPool(IObjectPool<FoundRowUI> rowUIPool)
        {
            _rowUIPool = rowUIPool;
        }

        public void Release()
        {
            _rowUIPool.Release(this);
        }

        public async void Setup(IUserData userData, ushort rankNumber, EFoundCategory category, CacheData cacheData)
        {
            if (!await MyUserData.IsAdmin()) return;

            _userData = userData;

            RefreshUI(rankNumber, category);

            _row = cacheData.FoundRow;
            _userInfo = cacheData.UserInfo;

            _localizer = cacheData.Localizer;
            _serverTime = cacheData.ServerTime;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async void RefreshUI(ushort rankNumber, EFoundCategory category)
        {
            if (_userData == default)
            {
                Debug.LogError("CUSTOM Error : RowUI.cs is created BUT havn't Setup() yet! Must call Setup() method first!");
                return;
            }
            
            _rankText.text = rankNumber.ToString();

            _userData.UpdateProfileIcon(_icon, _userData.GetProfileIcon(), MultiplierRatioForDecorator);

            _userData.UpdateMiniInfo(_miniInfoInspectorUI, await _userData.GetDataNationalIDInfo(), await _userData.GetDataPassportInfo(), _localizer, _serverTime);

            _userData.UpdateAccountStatus(_accountStatusUI, _userData.GetAccountStatus(), _localizer);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RowClick()
        {
            _row.OnRowClick();

            _userInfo.Setup(_userData);
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        public class CacheData
        {
            public GameObject Player { get; set; }
            public FoundRow FoundRow { get; set; }
            public UserInfo UserInfo { get; set; }
            public Localizer Localizer { get; set; }
            public ServerTime ServerTime { get; set; }
        }
        #endregion
    }
}