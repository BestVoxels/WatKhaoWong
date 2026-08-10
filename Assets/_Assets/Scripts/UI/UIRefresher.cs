using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using WatKhaoWong.Challenges;
using WatKhaoWong.Settings;
using WatKhaoWong.Identities;
using WatKhaoWong.Leaderboards;
using WatKhaoWong.Attributes;
using WatKhaoWong.Retreats;
using WatKhaoWong.Admin;

namespace WatKhaoWong.UI
{
    /// <summary>
    /// This component provides the Static Methods to Refresh the UI display partially or all, easy calling because of static.
    /// This script only refresh according to a specific GameObject's Data. (Ex. Player GameObject's Data of Health, active Shop, QuestList)
    ///
    /// TO USE:
    /// - Setup subscriber by this example : HealthDisplay.cs subscribe to UIDisplayManager.cs THEN we subscribe our Action with Health.cs here.
    /// - Calling Public Methods : simply calling ClassName.MethodName() without the need of reference to this class.
    /// - This component Must be destroyed to clear out subscribers. Can NOT put under 'PersistentObjects' prefab.
    /// </summary>
    public class UIRefresher : MonoBehaviour
    {
        #region --Events-- (Delegate as Action)
        public static event Action OnAllConditionCheckCalled;
        public static event Action OnHomeRefreshed;
        public static event Action OnPrayRefreshed;
        public static event Action OnManageMembersRefreshed;
        public static event Action OnMeditationRetreatRefreshed;
        public static event Action OnSettingRefreshed;
        //public static event Action OnAbbotHistoryRefreshed;
        public static event Action OnPopupRefreshed;
        public static event Action OnLeaderboardRefreshed;
        public static event Action OnApprovalBoardRefreshed;
        public static event Action OnFoundBoardRefreshed;
        public static event Action OnUserInfoRefreshed;
        public static event Action OnLocalizeDynamicString;
        public static event Action OnUIShowedHidByRoles;
        #endregion



        #region --Fields-- (In Class)
        private MyUserData _myUserData;
        private NotificationPopup _notificationPopup;
        private AccountPopup _accountPopup;
        private Leaderboard _leaderboard;
        private ApprovalBoard _approvalBoard;
        private FoundBoard _foundBoard;
        private UserInfo _userInfo;
        private Challenge _challenge;
        private RemoteConfigService _remoteConfigService;
        #endregion



        #region --Fields-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _myUserData = player.GetComponentInChildren<MyUserData>();
            _notificationPopup = player.GetComponentInChildren<NotificationPopup>();
            _accountPopup = player.GetComponentInChildren<AccountPopup>();
            _leaderboard = player.GetComponentInChildren<Leaderboard>();
            _approvalBoard = player.GetComponentInChildren<ApprovalBoard>();
            _foundBoard = player.GetComponentInChildren<FoundBoard>();
            _userInfo = player.GetComponentInChildren<UserInfo>();
            _challenge = player.GetComponentInChildren<Challenge>();
            _remoteConfigService = FindAnyObjectByType<RemoteConfigService>(FindObjectsInactive.Include);
        }

        private void OnEnable()
        {
            // IDENTITY SYSTEM
            _myUserData.OnMyUserDataUpdated += RefreshAllUI; // Just Refresh All cuz even LeaderboardUI still has to (MeRowUI will show correct result when MyUserData is loaded)

            // HOME SYSTEM
            // TODO lets see what to subscribe to for HOME SYSTEM

            // PRAY SYSTEM
            //_undoPopup.OnUploadSucceeded += () => { RefreshPrayUI(); };

            // SETTING SYSTEM
            _notificationPopup.OnNotificationSwitchChanged += () => { RefreshSettingUI(); };
            // NO NEED to do for Language since it changes using 'LocalizeStringEvent' component by itself.
            //_languagePopup.OnLanguageToggleIsOn += () => { RefreshSettingUI(); };

            // ABBOT HISTORY SYSTEM
            // TODO lets see what to subscribe to for HISTORY SYSTEM

            // SHARE POPUP SYSTEM
            _accountPopup.OnProfileIconChangedByClick += () => { RefreshPrayUI(); RefreshManageMembersUI(); RefreshUserInfoUI(); };

            // LEADERBOARD SYSTEM
            _leaderboard.OnLeaderboardCategoryChanged += () => { RefreshLeaderboardUI(); ShowHideUIByRoles(); };
            _leaderboard.OnLeaderboardScoreUpdated += RefreshLeaderboardUI;
            _challenge.OnDataUpdated += () => { RefreshPopupUI(); ShowHideUIByRoles(); RefreshLeaderboardUI(); };

            // APPROVAL BOARD SYSTEM
            _approvalBoard.OnCategoryChanged += () => { RefreshApprovalBoardUI(); };
            _approvalBoard.OnCallRefreshApprovalBoardUI += () => { RefreshApprovalBoardUI(); };

            // FOUND BOARD SYSTEM
            _foundBoard.OnCategoryChanged += () => { RefreshFoundBoardUI(); };
            _foundBoard.OnCallRefreshFoundBoardUI += () => { RefreshFoundBoardUI(); };

            // USER INFO SYSTEM
            _userInfo.OnTabChanged += () => { RefreshUserInfoUI(); };
            _userInfo.OnModeChanged += (mode) => { RefreshUserInfoUI(); };

            // CONDITION SYSTEM
            _leaderboard.OnConditionIsLeaderboardExistsUpdated += () => { CallAllConditionCheck(); RefreshLeaderboardUI(); };

            // LOCALIZATION SYSTEM
            LocalizationSettings.SelectedLocaleChanged += (obj) => LocalizeDynamicString();

            // REMOTE CONFIG SYSTEM
            _remoteConfigService.OnLoaded += ShowHideUIByRoles;
        }

        private void OnDisable()
        {
            // NONE of the Above Delegates are static so don't have to Unsubscribe to make it more clean / Also can't Unsubscribe anonymous function

            RemoveStaticDelegatesSubscribers();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC), (Subscriber)
        public void RefreshAllUI()
        {
            RefreshHomeUI();
            RefreshPrayUI();
            RefreshManageMembersUI();
            RefreshMeditationRetreatUI();
            RefreshSettingUI();
            //RefreshAbbotHistoryUI();
            RefreshPopupUI();
            // RefreshLeaderboardUI(); // No need waste performance. ONLY refresh when open page.
            // RefreshApprovalBoardUI(); // No need waste performance. ONLY refresh when open page.
            // RefreshFoundBoardUI(); // No need waste performance. ONLY refresh when open page.
            RefreshUserInfoUI();
            ShowHideUIByRoles();
            //print("Refreshed All UI");
        }

        public void CallAllConditionCheck()
        {
            OnAllConditionCheckCalled?.Invoke();
            //print("CallAllConditionCheck : " + OnAllConditionCheckCalled?.GetInvocationList().Length);
        }

        public void RefreshHomeUI()
        {
            OnHomeRefreshed?.Invoke();
            //print("Refreshed Home UI : " + OnHomeRefreshed?.GetInvocationList().Length);
        }

        public void RefreshPrayUI()
        {
            OnPrayRefreshed?.Invoke();
            //print("Refreshed Pray UI : " + OnPrayRefreshed?.GetInvocationList().Length);
        }

        public async void RefreshManageMembersUI()
        {
            if (!await MyUserData.IsAdmin()) return;

            OnManageMembersRefreshed?.Invoke();
            //print("Refreshed Manage Members UI : " + OnManageMembersRefreshed?.GetInvocationList().Length);
        }

        public void RefreshMeditationRetreatUI()
        {
            OnMeditationRetreatRefreshed?.Invoke();
            //print("Refreshed Manage Members UI : " + OnMeditationRetreatRefreshed?.GetInvocationList().Length);
        }

        public void RefreshSettingUI()
        {
            OnSettingRefreshed?.Invoke();
            //print("Refreshed Setting UI : " + OnSettingRefreshed?.GetInvocationList().Length);
        }

        //public void RefreshAbbotHistoryUI()
        //{
        //    OnAbbotHistoryRefreshed?.Invoke();
        //    //print("Refreshed Abbot History UI : " + OnAbbotHistoryRefreshed?.GetInvocationList().Length);
        //}

        public void RefreshPopupUI()
        {
            OnPopupRefreshed?.Invoke();
            //print("Refreshed Popup UI : " + OnPopupRefreshed?.GetInvocationList().Length);
        }

        public void RefreshLeaderboardUI()
        {
            OnLeaderboardRefreshed?.Invoke();
            //print("Refreshed Leaderboard UI : " + OnLeaderboardRefreshed?.GetInvocationList().Length);
        }

        public async void RefreshApprovalBoardUI()
        {
            if (!await MyUserData.IsAdmin()) return;

            OnApprovalBoardRefreshed?.Invoke();
            //print("Refreshed Approval Board UI : " + OnApprovalBoardRefreshed?.GetInvocationList().Length);
        }

        public async void RefreshFoundBoardUI()
        {
            if (!await MyUserData.IsAdmin()) return;

            OnFoundBoardRefreshed?.Invoke();
            //print("Refreshed Found Board UI : " + OnFoundBoardRefreshed?.GetInvocationList().Length);
        }

        public void RefreshUserInfoUI()
        {
            OnUserInfoRefreshed?.Invoke();
            //print("Refreshed UserInfo UI : " + OnUserInfoRefreshed?.GetInvocationList().Length);
        }

        public void LocalizeDynamicString()
        {
            OnLocalizeDynamicString?.Invoke();
            //print("Localize Dynamic String : " + OnLocalizeDynamicString?.GetInvocationList().Length);
        }

        public void ShowHideUIByRoles()
        {
            OnUIShowedHidByRoles?.Invoke();
            //print("Showed Hid UI By Roles : " + OnUIShowedHidByRoles?.GetInvocationList().Length);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RemoveStaticDelegatesSubscribers()
        {
            OnAllConditionCheckCalled = null;

            OnHomeRefreshed = null;
            OnPrayRefreshed = null;
            OnManageMembersRefreshed = null;
            OnMeditationRetreatRefreshed = null;
            OnSettingRefreshed = null;
            //OnAbbotHistoryRefreshed = null;
            OnPopupRefreshed = null;
            OnLeaderboardRefreshed = null;
            OnApprovalBoardRefreshed = null;
            OnFoundBoardRefreshed = null;
            OnUserInfoRefreshed = null;
            OnLocalizeDynamicString = null;
            OnUIShowedHidByRoles = null;
        }
        #endregion
    }
}