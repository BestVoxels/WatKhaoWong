using System;
using UnityEngine;
using Firebase.Auth;
using WatKhaoWong.Saving;

namespace WatKhaoWong.Identity
{
    public class AccountRole : MonoBehaviour, ISaveable
    {
        #region --Events-- (Delegate as Action)
        public event Action OnRoleChanged;
        #endregion



        #region --Fields-- (In Class)
        [SerializeField] private EAccountRole _role = EAccountRole.Member;
        #endregion



        #region --Properties-- (With Backing Fields)
        public EAccountRole Role
        {
            get => _role;

            set
            {
                _role = value; // Role MUST be changed before calls "OnRoleChanged?.Invoke()" so UI can update properly.
                print("***Role Changed***");

                OnRoleChanged?.Invoke();
            }
        }
        #endregion



        #region --Methods-- (Built In)
        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.G))
        //    {
        //        Role = EAccountRole.Guest;
        //    }
        //    if (Input.GetKeyDown(KeyCode.M))
        //    {
        //        Role = EAccountRole.Member;
        //    }
        //    if (Input.GetKeyDown(KeyCode.A))
        //    {
        //        Role = EAccountRole.Admin;
        //    }
        //}

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void SetRoleToGuestIfNoAuthen()
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser == null)
                Role = EAccountRole.Guest;
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            return Role;
        }

        void ISaveable.RestoreState(object state)
        {
            Role = (EAccountRole)state;

            SetRoleToGuestIfNoAuthen();
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void HandleStateChanged(object obj, EventArgs args) => SetRoleToGuestIfNoAuthen();
        #endregion
    }
}