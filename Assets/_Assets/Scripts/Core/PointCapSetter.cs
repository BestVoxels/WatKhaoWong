using UnityEngine;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;

namespace WatKhaoWong.Core
{
    public class PointCapSetter : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private MyUserData _myUserData;
        private RemoteConfigService _remoteConfigService;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
            _remoteConfigService = FindAnyObjectByType<RemoteConfigService>();
        }

        private void OnEnable()
        {
            _remoteConfigService.OnLoaded += SetTMPointCap;
            _myUserData.OnRoleUpdated += SetTMPointCap;
        }

        private void Start()
        {
            SetTMPointCap();
        }

        private void OnDisable()
        {
            _remoteConfigService.OnLoaded -= SetTMPointCap;
            _myUserData.OnRoleUpdated -= SetTMPointCap;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private async void SetTMPointCap()
        {
            bool isMyUserDataSaveLoaded = await _myUserData.LoadCompletionSource.Task;

            if (isMyUserDataSaveLoaded == false)
            {
                Debug.LogError("Could not continue SetTMPointCap() on PointCapSetter.cs because MyUserData.cs LoadSave() is not completed.");
                return;
            }

            // Set TMPointCap
            int TMPointCap = _myUserData.GetRole() switch
            {
                EUserRole.Admin => _remoteConfigService.TMPointCapForAdmin,
                EUserRole.Phra => _remoteConfigService.TMPointCapForPhra,
                EUserRole.DhammaForces => _remoteConfigService.TMPointCapForDhammaForces,
                EUserRole.DhammaPractitioner => _remoteConfigService.TMPointCapForDhammaPractitioner,
                EUserRole.LayPeople => _remoteConfigService.TMPointCapForLayPeople,
                _ => -1
            };

            if (!_myUserData.GetIsCustomTMPointCap())
                _myUserData.ForceSetTMPointCap(TMPointCap);

            // Set TMPointCapRound
            _myUserData.ForceSetTMPointCapRound(_remoteConfigService.TMPointCapRound);
        }
        #endregion
    }
}