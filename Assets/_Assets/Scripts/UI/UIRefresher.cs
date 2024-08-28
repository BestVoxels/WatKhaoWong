using System;
using UnityEngine;
using WatKhaoWong.Prays;
using WatKhaoWong.Settings;
using WatKhaoWong.SharePopup;
using WatKhaoWong.Identity;
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
        public static event Action OnHomeRefreshed;
        public static event Action OnPrayRefreshed;
        public static event Action OnSettingRefreshed;
        public static event Action OnHistoryRefreshed;
        public static event Action OnPopupRefreshed;
        public static event Action OnLeaderboardRefreshed;
        public static event Action OnUIShowedHidByRoles;
        #endregion



        #region --Fields-- (In Class)
        private MyUserData _myUserData;
        private UndoPopup _undoPopup;
        private NotificationPopup _notificationPopup;
        private AccountPopup _accountPopup;
        private Leaderboard _leaderboard;
        private ChallengePopup _challengePopup;
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
            _challengePopup = player.GetComponentInChildren<ChallengePopup>();
        }

        private void OnEnable()
        {
            // IDENTITY SYSTEM
            _myUserData.OnMyUserDataUpdated += RefreshAllUI; // Just Refresh All cuz even LeaderboardUI still has to (MeRowUI will show correct result when MyUserData is loaded)

            // HOME SYSTEM
            // TODO lets see what to subscribe to for HOME SYSTEM

            // PRAY SYSTEM
            _undoPopup.OnUploadSucceed += () => { RefreshPrayUI(); };

            // SETTING SYSTEM
            _notificationPopup.OnNotificationSwitchChanged += () => { RefreshSettingUI(); };
            // TODO lets see what to subscribe to for SETTING SYSTEM -> probably last one is _languagePopup when select each language toggle

            // HISTORY SYSTEM
            // TODO lets see what to subscribe to for HISTORY SYSTEM

            // SHARE POPUP SYSTEM
            _accountPopup.OnProfileIconChangedByClick += () => { RefreshPrayUI(); };

            // LEADERBOARD SYSTEM
            _leaderboard.OnCategoryChanged += ShowHideUIByRoles;
            _challengePopup.OnHasChallengeChanged += ShowHideUIByRoles;
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
            RefreshHistoryUI();
            RefreshPopupUI();
            RefreshLeaderboardUI();
            ShowHideUIByRoles();
            //print("Refreshed All UI");
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

        public static void RefreshHistoryUI()
        {
            OnHistoryRefreshed?.Invoke();
            //print("Refreshed History UI : " + OnHistoryRefreshed?.GetInvocationList().Length);
        }

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

        public static void ShowHideUIByRoles()
        {
            OnUIShowedHidByRoles?.Invoke();
            //print("Showed Hid UI By Roles : " + OnUIShowedHidByRoles?.GetInvocationList().Length);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RemoveStaticDelegatesSubscribers()
        {
            OnHomeRefreshed = null;
            OnPrayRefreshed = null;
            OnSettingRefreshed = null;
            OnHistoryRefreshed = null;
            OnPopupRefreshed = null;
            OnLeaderboardRefreshed = null;
            OnUIShowedHidByRoles = null;
        }
        #endregion
    }
}