using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using WatKhaoWong.Prays;
using WatKhaoWong.Challenges;
using WatKhaoWong.Settings;
using WatKhaoWong.Identities;
using WatKhaoWong.Leaderboards;

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
        public static event Action OnSettingRefreshed;
        //public static event Action OnAbbotHistoryRefreshed;
        public static event Action OnPopupRefreshed;
        public static event Action OnLeaderboardRefreshed;
        public static event Action OnLocalizeDynamicString;
        public static event Action OnUIShowedHidByRoles;
        #endregion



        #region --Fields-- (In Class)
        private MyUserData _myUserData;
        private UndoPopup _undoPopup;
        private NotificationPopup _notificationPopup;
        private AccountPopup _accountPopup;
        private Leaderboard _leaderboard;
        private Challenge _challenge;
        #endregion



        #region --Fields-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _myUserData = player.GetComponentInChildren<MyUserData>();
            _undoPopup = player.GetComponentInChildren<UndoPopup>();
            _notificationPopup = player.GetComponentInChildren<NotificationPopup>();
            _accountPopup = player.GetComponentInChildren<AccountPopup>();
            _leaderboard = player.GetComponentInChildren<Leaderboard>();
            _challenge = player.GetComponentInChildren<Challenge>();
        }

        private void OnEnable()
        {
            // IDENTITY SYSTEM
            _myUserData.OnMyUserDataUpdated += RefreshAllUI; // Just Refresh All cuz even LeaderboardUI still has to (MeRowUI will show correct result when MyUserData is loaded)

            // HOME SYSTEM
            // TODO lets see what to subscribe to for HOME SYSTEM

            // PRAY SYSTEM
            _undoPopup.OnUploadSucceeded += () => { RefreshPrayUI(); };

            // SETTING SYSTEM
            _notificationPopup.OnNotificationSwitchChanged += () => { RefreshSettingUI(); };
            // NO NEED to do for Language since it changes using 'LocalizeStringEvent' component by itself.
            //_languagePopup.OnLanguageToggleIsOn += () => { RefreshSettingUI(); };

            // ABBOT HISTORY SYSTEM
            // TODO lets see what to subscribe to for HISTORY SYSTEM

            // SHARE POPUP SYSTEM
            _accountPopup.OnProfileIconChangedByClick += () => { RefreshPrayUI(); };

            // LEADERBOARD SYSTEM
            _leaderboard.OnLeaderboardCategoryChanged += () => { RefreshLeaderboardUI(); ShowHideUIByRoles(); };
            _leaderboard.OnLeaderboardScoreUpdated += RefreshLeaderboardUI;
            _challenge.OnDataUpdated += () => { RefreshPopupUI(); ShowHideUIByRoles(); RefreshLeaderboardUI(); };

            // CONDITION SYSTEM
            _leaderboard.OnConditionIsLeaderboardExistsUpdated += CallAllConditionCheck;

            // LOCALIZATION SYSTEM
            LocalizationSettings.SelectedLocaleChanged += (obj) => LocalizeDynamicString();
        }

        private void OnDisable()
        {
            // NONE of the Above Delegates are static so don't have to Unsubscribe to make it more clean / Also can't Unsubscribe anonymous function

            RemoveStaticDelegatesSubscribers();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC), (Subscriber)
        public static void RefreshAllUI()
        {
            RefreshHomeUI();
            RefreshPrayUI();
            RefreshSettingUI();
            //RefreshAbbotHistoryUI();
            RefreshPopupUI();
            RefreshLeaderboardUI();
            ShowHideUIByRoles();
            //print("Refreshed All UI");
        }

        public static void CallAllConditionCheck()
        {
            OnAllConditionCheckCalled?.Invoke();
            //print("CallAllConditionCheck : " + OnAllConditionCheckCalled?.GetInvocationList().Length);
        }

        public static void RefreshHomeUI()
        {
            OnHomeRefreshed?.Invoke();
            //print("Refreshed Home UI : " + OnHomeRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshPrayUI()
        {
            OnPrayRefreshed?.Invoke();
            //print("Refreshed Pray UI : " + OnPrayRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshSettingUI()
        {
            OnSettingRefreshed?.Invoke();
            //print("Refreshed Setting UI : " + OnSettingRefreshed?.GetInvocationList().Length);
        }

        //public static void RefreshAbbotHistoryUI()
        //{
        //    OnAbbotHistoryRefreshed?.Invoke();
        //    //print("Refreshed Abbot History UI : " + OnAbbotHistoryRefreshed?.GetInvocationList().Length);
        //}

        public static void RefreshPopupUI()
        {
            OnPopupRefreshed?.Invoke();
            //print("Refreshed Popup UI : " + OnPopupRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshLeaderboardUI()
        {
            OnLeaderboardRefreshed?.Invoke();
            //print("Refreshed Leaderboard UI : " + OnLeaderboardRefreshed?.GetInvocationList().Length);
        }

        public static void LocalizeDynamicString()
        {
            OnLocalizeDynamicString?.Invoke();
            //print("Localize Dynamic String : " + OnLocalizeDynamicString?.GetInvocationList().Length);
        }

        public static void ShowHideUIByRoles()
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
            OnSettingRefreshed = null;
            //OnAbbotHistoryRefreshed = null;
            OnPopupRefreshed = null;
            OnLeaderboardRefreshed = null;
            OnLocalizeDynamicString = null;
            OnUIShowedHidByRoles = null;
        }
        #endregion
    }
}