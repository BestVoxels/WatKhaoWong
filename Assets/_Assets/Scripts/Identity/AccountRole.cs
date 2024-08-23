using System;
using UnityEngine;
using Firebase.Auth;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.Identity
{
    public class AccountRole : MonoBehaviour
    {
        #region --Events-- (Delegate as Action)
        public event Action OnRoleChanged;
        #endregion



        #region --Fields-- (In Class)
        [SerializeField] private EAccountRole _role = EAccountRole.Member;

        private SavingWrapper _savingWrapper;
        #endregion



        #region --Properties-- (With Backing Fields)
        public EAccountRole Role
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
                Role = (EAccountRole)Enum.Parse(typeof(EAccountRole), data.Value.ToString());
        }

        private void SetRoleToGuestIfNoAuthen()
        {
            if (!IsAuthenticated())
                Role = EAccountRole.Guest;
        }

        private bool IsAuthenticated() => FirebaseAuth.DefaultInstance.CurrentUser != null;
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