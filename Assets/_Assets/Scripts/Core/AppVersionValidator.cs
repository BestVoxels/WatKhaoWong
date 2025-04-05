using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Core
{
    public class AppVersionValidator : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("App Version Validator Event (iOS)")]
        [SerializeField] private UnityEvent _onAppVersionMatchIOS;
        [SerializeField] private UnityEvent _onAppVersionNotMatchIOS;
        [SerializeField] private UnityEvent _onAppVersionNotFoundIOS;

        [Space]

        [Header("App Version Validator Event (Android)")]
        [SerializeField] private UnityEvent _onAppVersionMatchAndroid;
        [SerializeField] private UnityEvent _onAppVersionNotMatchAndroid;
        [SerializeField] private UnityEvent _onAppVersionNotFoundAndroid;

        [Space]

        [Header("For Setup")]
        [SerializeField] private UnityEvent<string> _linkToUpdateAppIOS;
        [SerializeField] private UnityEvent<string> _linkToUpdateAppAndroid;
        #endregion



        #region --Fields-- (In Class)
        private RemoteConfigService _remoteConfigService;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _remoteConfigService = FindAnyObjectByType<RemoteConfigService>(FindObjectsInactive.Include);
        }

        private void OnEnable()
        {
            _remoteConfigService.OnLoaded += ValidateAppVersion;
        }

        private void OnDisable()
        {
            _remoteConfigService.OnLoaded -= ValidateAppVersion;
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void OpenLinkToUpdateAppiOS()
        {
            _linkToUpdateAppIOS?.Invoke(_remoteConfigService.LinkToUpdateAppiOS);
        }

        public void OpenLinkToUpdateAppAndroid()
        {
            _linkToUpdateAppAndroid?.Invoke(_remoteConfigService.LinkToUpdateAppAndroid);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void ValidateAppVersion()
        {
#if UNITY_IOS
            if (string.IsNullOrWhiteSpace(_remoteConfigService.LiveAppVersioniOS) || string.IsNullOrWhiteSpace(_remoteConfigService.InReviewAppVersioniOS))
            {
                _onAppVersionNotFoundIOS?.Invoke();
                return;
            }
#elif UNITY_ANDROID
            if (string.IsNullOrWhiteSpace(_remoteConfigService.LiveAppVersionAndroid) || string.IsNullOrWhiteSpace(_remoteConfigService.InReviewAppVersionAndroid))
            {
                _onAppVersionNotFoundAndroid?.Invoke();
                return;
            }
#endif

#if UNITY_IOS
            if (_remoteConfigService.LiveAppVersioniOS.Equals(Application.version) || _remoteConfigService.InReviewAppVersioniOS.Equals(Application.version))
            {
                _onAppVersionMatchIOS?.Invoke();
            }
            else
            {
                _onAppVersionNotMatchIOS?.Invoke();
            }
#elif UNITY_ANDROID
            if (_remoteConfigService.LiveAppVersionAndroid.Equals(Application.version) || _remoteConfigService.InReviewAppVersionAndroid.Equals(Application.version))
            {
                _onAppVersionMatchAndroid?.Invoke();
            }
            else
            {
                _onAppVersionNotMatchAndroid?.Invoke();
            }
#endif
        }
        #endregion
    }
}