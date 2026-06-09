using System;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Authentication
{
    public class FullNamePromptHandler : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("UI Event")]
        [SerializeField] private UnityEvent _onUserNameMissing;
        #endregion



        #region --Fields-- (In Class)
        private SignupPopup _playerSignupPopup;
        private SavingWrapper _savingWrapper;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerSignupPopup = GameObject.FindWithTag("Player").GetComponentInChildren<SignupPopup>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged; // This will trigger on Start() too so don't have to call LoadSave() on Start()
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async void LoadSave()
        {
            var data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.Role);

            // Detects if "Authenticated & No Role Data", so we can prompt them to enter info.
            if (data == null && FirebaseUtils.IsAuthenticated() && _playerSignupPopup.IsSigningUp == false)
            {
                _onUserNameMissing?.Invoke();
                _playerSignupPopup.IsSigningUp = false;
            }
        }
        #endregion



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake(). And at time of assiging to 'FirebaseAuth.DefaultInstance.StateChanged'
        /// </summary>
        private void HandleStateChanged(object obj, EventArgs args)
        {
            LoadSave(); // So Don't have to call on Awake()
        }
        #endregion
    }
}