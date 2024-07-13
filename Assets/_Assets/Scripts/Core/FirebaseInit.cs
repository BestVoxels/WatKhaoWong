using Firebase;
using Firebase.Extensions;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Core
{
    public class FirebaseInit : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Firebase Event")]
        [SerializeField] private UnityEvent _onFirebaseInitialized;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            // NOTE : WITHOUT calling "CheckAndFixDependenciesAsync()" we CAN NOT call "FirebaseCATEGORY.DefaultInstance"
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogError($"Failed to initialize Firebase with {task.Exception}");
                    return;
                }

                _onFirebaseInitialized?.Invoke();
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            });
        }
        #endregion
    }
}