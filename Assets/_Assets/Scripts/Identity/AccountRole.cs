using System;
using UnityEngine;
using Firebase.Auth;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Identity
{
    public class AccountRole : MonoBehaviour
    {
        #region --Events-- (Delegate as Action)
        public event Action OnRoleChanged;
        #endregion



        #region --Fields-- (In Class)
        [SerializeField] private EUserRole _role = EUserRole.Member;

        private SavingWrapper _savingWrapper;
        #endregion



        #region --Properties-- (With Backing Fields)
        public EUserRole Role
        {
            get => _role;

            set
            {
                _role = value; // Role MUST be changed before calls "OnRoleChanged?.Invoke()" so UI can update properly.

                _savingWrapper.Save(EValueNode.Role, _role.ToString());
                print("***Role Changed***");

                OnRoleChanged?.Invoke();
            }
        }
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
        private void AssignUserRole()
        {
            LoadSave();

            SetRoleToGuestIfNoAuthen();
        }

        private async void LoadSave()
        {
            var data = await _savingWrapper.Load(EValueNode.Role);

            if (data != null)
                Role = (EUserRole)Enum.Parse(typeof(EUserRole), data.Value.ToString());
        }

        private void SetRoleToGuestIfNoAuthen()
        {
            if (!FirebaseUtils.IsAuthenticated())
                Role = EUserRole.Guest;
        }
        #endregion



        #region --Methods-- (Interface)
        #endregion



        #region --Methods-- (Subscriber)
        private void HandleStateChanged(object obj, EventArgs args)
        {
            AssignUserRole(); // Don't have to LoadSave() on Awake() because it will be called once after FirebaseAuth instance is created.
        }
        #endregion
    }
}