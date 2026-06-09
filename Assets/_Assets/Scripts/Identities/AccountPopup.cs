using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Identities
{
    public class AccountPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Account Popup Status Text")]
        [field: SerializeField] public LocalizedString StatusInformUser { get; private set; }
        [field: SerializeField] public Color32 StatusInformUserColor { get; private set; }

        [field: Header("Account Popup - Title Text")]
        [field: SerializeField] public LocalizedString MyInfoTitleText { get; private set; }
        [field: SerializeField] public LocalizedString UserInfoTitleText { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Account Popup UI Event")]
        [SerializeField] private UnityEvent _onAccountProfileChangedByClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<EAccountPopupView, IUserData> OnViewSetup;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnProfileIconChangedByClick;
        #endregion



        #region --Fields-- (In Class)
        private EAccountPopupView _currentView;

        private MyUserData _myUserData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
        }

        private void Start()
        {
            ShowMyUser();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnAccountProfileChangedByClick()
        {
            OnProfileIconChangedByClick?.Invoke();
            _onAccountProfileChangedByClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page Setup~
        public void Setup(IUserData userData)
        {
            OnViewSetup?.Invoke(_currentView, userData);
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void ShowMyUser()
        {
            _currentView = EAccountPopupView.MyUser;

            Setup(_myUserData);
        }

        public void ShowOtherUser()
        {
            _currentView = EAccountPopupView.OtherUser;
        }
        #endregion
    }
}