using System;
using UnityEngine;
using Firebase.Auth;

namespace WatKhaoWong.Identity
{
    public class AccountRole : MonoBehaviour
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

        private void Start()
        {
            AssignUserRole();
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void AssignUserRole()
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser == null)
                Role = EAccountRole.Guest;

            if (FirebaseAuth.DefaultInstance.CurrentUser != null)
                Role = EAccountRole.Member;

            // TODO set Role to 'Admin' this have to load from Server
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void HandleStateChanged(object obj, EventArgs args) => AssignUserRole();
        #endregion
    }
}