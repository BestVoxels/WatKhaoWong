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
        public static event Action OnPrayRefreshed;
        #endregion



        #region --Fields-- (In Class)
        private ConfirmPopup _confirmPopup;
        #endregion



        #region --Fields-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _confirmPopup = player.GetComponentInChildren<ConfirmPopup>();
        }

        private void OnEnable()
        {
            // PRAY SYSTEM
            _confirmPopup.OnUploadSucceed += () => { RefreshPrayUI(); };
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
            RefreshPrayUI();
            print("Refreshed All UI");
        }

        public static void RefreshPrayUI()
        {
            OnPrayRefreshed?.Invoke();
            print("Refreshed Pray UI " + OnPrayRefreshed?.GetInvocationList().Length);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RemoveStaticDelegatesSubscribers()
        {
            OnPrayRefreshed = null;
        }
        #endregion
    }
}