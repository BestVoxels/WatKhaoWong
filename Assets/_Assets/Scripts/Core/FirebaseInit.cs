using Firebase;
using Firebase.Extensions;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Core
{
    /// <summary>
    /// This component should be created once and shared between all subsequent scenes.
    /// 
    /// *****
    /// FirebaseInit.cs script SHOULD Execute before default time under ‘Project Settings/Script Execution Order’. (Ex-Check from WatKhaoWong project)
    /// Reason is because WITHOUT calling "CheckAndFixDependenciesAsync()" we CAN NOT call "FirebaseCATEGORY.DefaultInstance" on some Android Device.
    /// Also might encounter ERROR MESSAGE as below.
    /// *****
    /// 
    /// ERROR MESSAGE:
    /// InvalidOperationException: Don't call Firebase functions before CheckDependencies has finished
    /// Firebase.FirebaseApp.ThrowIfCheckDependenciesRunning () (at /home/runner/work/firebase-unity-sdk/firebase-unity-sdk/linux_unity/app/swig/Firebase.App_fixed.cs:2571)
    /// </summary>
    public class FirebaseInit : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Firebase Event")]
        [SerializeField] private UnityEvent _onFirebaseInitialized;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
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