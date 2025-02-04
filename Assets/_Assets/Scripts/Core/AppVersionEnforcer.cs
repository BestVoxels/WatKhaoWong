using UnityEngine;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Core
{
    public class AppVersionEnforcer : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private RemoteConfigService _remoteConfigService;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _remoteConfigService = FindAnyObjectByType<RemoteConfigService>();
        }

        private void OnEnable()
        {
            _remoteConfigService.OnLoaded += EnforceAppVersion;
        }

        private void OnDisable()
        {
            _remoteConfigService.OnLoaded -= EnforceAppVersion;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void EnforceAppVersion()
        {
#if UNITY_IOS
            if (string.IsNullOrWhiteSpace(_remoteConfigService.LiveAppVersioniOS))
            {
                return;
            }
#elif UNITY_ANDROID
            if (string.IsNullOrWhiteSpace(_remoteConfigService.LiveAppVersionAndroid))
            {
                return;
            }
#endif

#if UNITY_IOS
            if (!_remoteConfigService.LiveAppVersioniOS.Equals(Application.version))
            {
                Application.Quit();
            }
#elif UNITY_ANDROID
            if (!_remoteConfigService.LiveAppVersionAndroid.Equals(Application.version))
            {
                Application.Quit();
            }
#endif
        }
        #endregion
    }
}