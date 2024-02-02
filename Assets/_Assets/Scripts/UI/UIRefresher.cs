using System;
using UnityEngine;
using WatKhaoWong.Prays;

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
        #endregion



        #region --Fields-- (In Class)
        // TODO lets see what to subscribe to for HOME SYSTEM
        private UndoPopup _undoPopup;
        // TODO lets see what to subscribe to for SETTING SYSTEM
        // TODO lets see what to subscribe to for HISTORY SYSTEM
        #endregion



        #region --Fields-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _undoPopup = player.GetComponentInChildren<UndoPopup>();
        }

        private void OnEnable()
        {
            // HOME SYSTEM
            // TODO lets see what to subscribe to for HOME SYSTEM

            // PRAY SYSTEM
            _undoPopup.OnUploadSucceed += () => { RefreshPrayUI(); };

            // SETTING SYSTEM
            // TODO lets see what to subscribe to for SETTING SYSTEM

            // HISTORY SYSTEM
            // TODO lets see what to subscribe to for HISTORY SYSTEM
        }

        private void OnDisable()
        {
            // NONE of the Above Delegates are static so don't have to Unsubscribe to make it more clean

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
            print("Refreshed All UI");
        }

        public static void RefreshHomeUI()
        {
            OnHomeRefreshed?.Invoke();
            print("Refreshed Home UI " + OnHomeRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshPrayUI()
        {
            OnPrayRefreshed?.Invoke();
            print("Refreshed Pray UI " + OnPrayRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshSettingUI()
        {
            OnSettingRefreshed?.Invoke();
            print("Refreshed Setting UI " + OnSettingRefreshed?.GetInvocationList().Length);
        }

        public static void RefreshHistoryUI()
        {
            OnHistoryRefreshed?.Invoke();
            print("Refreshed History UI " + OnHistoryRefreshed?.GetInvocationList().Length);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RemoveStaticDelegatesSubscribers()
        {
            OnHomeRefreshed = null;
            OnPrayRefreshed = null;
            OnSettingRefreshed = null;
            OnHistoryRefreshed = null;
        }
        #endregion
    }
}