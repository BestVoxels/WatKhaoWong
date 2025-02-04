using System;
using UnityEngine;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.Attributes
{
    /// <summary>
    /// This component provides the Properties to Remote Config Data for classes to use.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    /// </summary>
    public class RemoteConfigService : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        //[Header("Remote Config Stuffs")]
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnLoaded;
        #endregion



        #region --Fields-- (In Class)
        private SavingWrapper _savingWrapper;
        #endregion



        #region --Properties-- (Auto)
        public string LiveAppVersioniOS { get; private set; } = null;
        public string LiveAppVersionAndroid { get; private set; } = null;
        public string LinkToUpdateAppiOS { get; private set; } = null;
        public string LinkToUpdateAppAndroid { get; private set; } = null;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void Start()
        {
            ForceLoadConfigWithoutAuth();
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
                ForceLoadConfigWithoutAuth();
        }

        //private async void Start()
        //{
        //    bool existResult = await _savingWrapper.IsSaveExists(ECategoryNode.RemoteConfig, EValueNode.LinkToUpdateAppAndroid);
        //    print(existResult);

        //    if (!existResult)
        //    {
        //        _savingWrapper.ForceSave(ECategoryNode.RemoteConfig, EValueNode.LinkToUpdateAppAndroid, "https://www.bestvoxels.com/blog");
        //        print("ForceSave is working");
        //    }
        //}
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async void ForceLoadConfigWithoutAuth()
        {
#if UNITY_IOS
            var data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.LiveAppVersioniOS);
            if (data != null)
                LiveAppVersioniOS = data.Value.ToString();

            data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.LinkToUpdateAppiOS);
            if (data != null)
                LinkToUpdateAppiOS = data.Value.ToString();

#elif UNITY_ANDROID
            var data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.LiveAppVersionAndroid);
            if (data != null)
                LiveAppVersionAndroid = data.Value.ToString();

            data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.LinkToUpdateAppAndroid);
            if (data != null)
                LinkToUpdateAppAndroid = data.Value.ToString();
#endif

            OnLoaded?.Invoke();
        }
#endregion
    }
}