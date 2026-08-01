using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WatKhaoWong.Identities;
using WatKhaoWong.Admin;
using UnityEngine.Pool;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.SceneManagement;
using System;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Retreats;
using Firebase.Database;

namespace WatKhaoWong.UI.Admin
{
    public class ApprovalRowUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Buttons Stuffs")]
        [SerializeField] private Button _rowButton;
        [SerializeField] private Button _rejectButton;
        [SerializeField] private Button _approveButton;

        [Header("UI Stuffs")]
        [SerializeField] private TMP_Text _rankText;
        [SerializeField] private ProfileIconInspector _icon;
        [SerializeField] private MiniInfoInspector _miniInfoInspectorUI;
        [SerializeField] private AccountStatusInspector _accountStatusUI;
        [SerializeField] private TMP_Text _activityResultText;
        [SerializeField] private TMP_Text _stayDurationResultText;
        [SerializeField] private TMP_Text _plateNumberResultText;
        [SerializeField] private GameObject _buttonsPanel;
        #endregion



        #region --Fields-- (In Class)
        private ApprovalRow _row;
        private StayEntry _stayEntry;
        private string _keyId;
        private IUserData _userData;
        private ApprovalNoPopupUI _approvalNoPopupUI;
        private ApprovalYesPopupUI _approvalYesPopupUI;
        private AccommodationSetTimePopup _setTimePopup;
        private IObjectPool<ApprovalRowUI> _rowUIPool;
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
            _rejectButton.onClick.AddListener(ClickReject);
            _approveButton.onClick.AddListener(ClickAccept);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void OnCreatedByPool(IObjectPool<ApprovalRowUI> rowUIPool)
        {
            _rowUIPool = rowUIPool;
        }

        public void Release()
        {
            _rowUIPool.Release(this);
        }

        public void Setup((StayEntry stayEntry, string keyId, DataSnapshot dataSnapShot) rowData, ushort rankNumber, EApprovalCategory category, CacheData cacheData)
        {
            _stayEntry = rowData.stayEntry;
            _keyId = rowData.keyId;
            OtherUserData otherUserData = new OtherUserData(rowData.dataSnapShot);
            _userData = otherUserData;

            RefreshUI(rankNumber, category);

            _row = cacheData.ApprovalRow;
            _setTimePopup = cacheData.SetTimePopup;
            _userInfo = cacheData.UserInfo;

            _localizer = cacheData.Localizer;
            _serverTime = cacheData.ServerTime;
            _approvalNoPopupUI = cacheData.ApprovalNoPopupUI;
            _approvalYesPopupUI = cacheData.ApprovalYesPopupUI;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async void RefreshUI(ushort rankNumber, EApprovalCategory category)
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
            
            // Manage Buttons Panel
            if (category == EApprovalCategory.Pending)
                _buttonsPanel.SetActive(true);
            else
                _buttonsPanel.SetActive(false);

            // Activity
            _activityResultText.text = _localizer.LocalizeActivityType(_stayEntry.Activity);

            // Stay Duration
            _stayEntry.StayInfo.StartDate.TryParseGregorian(out DateTime startDate);
            _stayEntry.StayInfo.EndDate.TryParseGregorian(out DateTime endDate);
            _stayDurationResultText.text = _setTimePopup.FormatButtonString(startDate, endDate, _row.DayFormat);

            // PlateNumber
            if (IsHasCarFromResult(_stayEntry))
                _plateNumberResultText.text = _stayEntry.Transportation.CarPlateNumber;
            else
                _plateNumberResultText.text = _row.NoDataText.GetLocalizedString();
        }

        private bool IsHasCarFromResult(StayEntry stayEntry)
        {
            if (stayEntry == null) return false;

            return ((EHasCar)Enum.Parse(typeof(EHasCar), stayEntry.Transportation.HasCar)) == EHasCar.Has;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RowClick()
        {
            _row.OnRowClick();

            _userInfo.Setup(_userData);
        }

        private void ClickReject()
        {
            _approvalNoPopupUI.Setup(_stayEntry, _keyId, _userData);
            _row.OnClickReject();
        }

        private void ClickAccept()
        {
            _approvalYesPopupUI.Setup(_stayEntry, _keyId, _userData);
            _row.OnClickAccept();
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        public class CacheData
        {
            public GameObject Player { get; set; }
            public ApprovalRow ApprovalRow { get; set; }
            public AccommodationSetTimePopup SetTimePopup { get; set; }
            public UserInfo UserInfo { get; set; }
            public Localizer Localizer { get; set; }
            public ServerTime ServerTime { get; set; }
            public ApprovalNoPopupUI ApprovalNoPopupUI { get; set; }
            public ApprovalYesPopupUI ApprovalYesPopupUI { get; set; }
        }
        #endregion
    }
}