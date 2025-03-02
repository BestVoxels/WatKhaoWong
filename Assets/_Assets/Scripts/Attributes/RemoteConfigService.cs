using System;
using Firebase.Auth;
using UnityEngine;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Conditions;

namespace WatKhaoWong.Attributes
{
    /// <summary>
    /// This component provides the Properties to Remote Config Data for classes to use.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    /// </summary>
    public class RemoteConfigService : MonoBehaviour, IConditionEvaluator
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
        public bool AllowAccountDeletion { get; private set; } = false;
        public int TMPointCapForAdmin { get; private set; } = -1;
        public int TMPointCapForPhra { get; private set; } = -1;
        public int TMPointCapForDhammaForces { get; private set; } = -1;
        public int TMPointCapForDhammaPractitioner { get; private set; } = -1;
        public int TMPointCapForLayPeople { get; private set; } = -1;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
        }

        private void Start()
        {
            ForceLoadConfigWithoutAuth();
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
                ForceLoadConfigWithoutAuth();
        }

        //private async void Start()
        //{
        //    bool existResult = await _savingWrapper.IsSaveExists(ECategoryNode.RemoteConfig, EValueNode.AllowAccountDeletion);
        //    print(existResult);

        //    if (!existResult)
        //    {
        //        _savingWrapper.ForceSave(ECategoryNode.RemoteConfig, EValueNode.AllowAccountDeletion, true);
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

            data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.AllowAccountDeletion);
            if (data != null)
                AllowAccountDeletion = bool.Parse(data.Value.ToString());

            data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.TMPointCapForAdmin);
            if (data != null)
                TMPointCapForAdmin = int.Parse(data.Value.ToString());

            data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.TMPointCapForPhra);
            if (data != null)
                TMPointCapForPhra = int.Parse(data.Value.ToString());

            data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.TMPointCapForDhammaForces);
            if (data != null)
                TMPointCapForDhammaForces = int.Parse(data.Value.ToString());

            data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.TMPointCapForDhammaPractitioner);
            if (data != null)
                TMPointCapForDhammaPractitioner = int.Parse(data.Value.ToString());

            data = await _savingWrapper.ForceLoad(ECategoryNode.RemoteConfig, EValueNode.TMPointCapForLayPeople);
            if (data != null)
                TMPointCapForLayPeople = int.Parse(data.Value.ToString());

            OnLoaded?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        /// </summary>
        private void HandleStateChanged(object obj, EventArgs args)
        {
            ForceLoadConfigWithoutAuth();
        }
        #endregion



        #region --Methods-- (Interface)
        bool? IConditionEvaluator.Evaluate(EConditionType conditionType, EConditionValue[] conditionValues)
        {
            switch (conditionType)
            {
                case EConditionType.AllowAccountDeletion:
                    return AllowAccountDeletion;
            }

            return null;
        }
        #endregion
    }
}