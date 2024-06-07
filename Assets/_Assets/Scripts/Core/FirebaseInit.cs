using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace WatKhaoWong.Core
{
    public class FirebaseInit : MonoBehaviour
    {
        #region --Methods-- (Built In)
        private void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            });
        }
        #endregion
    }
}