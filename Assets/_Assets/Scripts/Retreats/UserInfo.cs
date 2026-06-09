using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Identities;

namespace WatKhaoWong.Retreats
{
    public class UserInfo : Page
    {
        #region --Fields-- (Inspector)
        [Header("UserInfo Settings")]
        [SerializeField] private EUserInfoTab _defaultTab;
        [SerializeField] private EViewEditMode _defaultViewEditMode;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("User Info - Status Text")]
        [field: SerializeField] public LocalizedString StatusRecordAdded { get; private set; }
        [field: SerializeField] public Color32 StatusRecordAddedColor { get; private set; }
        [field: SerializeField] public LocalizedString StatusChangesSaved { get; private set; }
        [field: SerializeField] public Color32 StatusChangesSavedColor { get; private set; }
        [field: SerializeField] public LocalizedString StatusRecordDeleted { get; private set; }
        [field: SerializeField] public Color32 StatusRecordDeletedColor { get; private set; }
        [field: SerializeField] public LocalizedString StatusMustBeFilled { get; private set; }
        [field: SerializeField] public Color32 StatusMustBeFilledColor { get; private set; }
        [field: SerializeField] public LocalizedString StatusCantAddCurExists { get; private set; }
        [field: SerializeField] public Color32 StatusCantAddCurExistsColor { get; private set; }
        [field: Space]
        [field: Header("User Info - Default Text to show when no Data")]
        [field: SerializeField] public LocalizedString NoDataText { get; private set; }
        [field: SerializeField] public Color32 DefaultNotesTextColor { get; private set; }
        [field: Space]
        [field: Header("User Info - Phone Number on Personal Info Tab")]
        [field: SerializeField] public byte MinimumPhoneNumberLength { get; private set; } = 9;
        [field: SerializeField] public byte MaximumPhoneNumberLength { get; private set; } = 10;
        [field: Space]
        [field: SerializeField] public LocalizedString StatusInvalidPhoneNumber { get; private set; }
        [field: SerializeField] public Color32 StatusInvalidPhoneNumberColor { get; private set; }
        [field: SerializeField] public LocalizedString StatusPhoneNumberTooShort { get; private set; }
        [field: SerializeField] public Color32 StatusPhoneNumberTooShortColor { get; private set; }
        [field: SerializeField] public LocalizedString StatusPhoneNumberTooLong { get; private set; }
        [field: SerializeField] public Color32 StatusPhoneNumberTooLongColor { get; private set; }

        [field: Header("User Info - Tab Settings")]
        [field: SerializeField] public Color32 SelectedColor { get; private set; }
        [field: SerializeField] public Color32 UnselectedColor { get; private set; }

        [field: Header("User Info - Title Text")]
        [field: SerializeField] public LocalizedString MyProfileTitleText { get; private set; }
        [field: SerializeField] public LocalizedString EditMyProfileTitleText { get; private set; }
        [field: SerializeField] public LocalizedString UserProfileTitleText { get; private set; }
        [field: SerializeField] public LocalizedString EditUserProfileTitleText { get; private set; }

        [field: Header("User Info - Edit/View Button")]
        [field: SerializeField] public LocalizedString EditButtonText { get; private set; }
        [field: SerializeField] public LocalizedString ViewButtonText { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("User Info UI Event")]
        [SerializeField] private UnityEvent _onViewEditButtonClick;
        [Space]
        [SerializeField] private UnityEvent _onUserProfileClickAsMyself;
        [SerializeField] private UnityEvent _onUserProfileClickAsOtherUser;
        [SerializeField] private UnityEvent _onUserIDCardClickAsMyself;
        [SerializeField] private UnityEvent _onUserIDCardClickAsOtherUser;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnTabChanged;
        public event Action<EViewEditMode> OnModeChanged;
        public event Action<EUserInfoView, IUserData> OnViewSetup;
        #endregion



        #region --Properties-- (Auto)
        public static bool IsAsyncRunning { get; private set; } = false;
        #endregion



        #region --Fields-- (In Class)
        private EUserInfoView _currentView;

        private MyUserData _myUserData;
        private SavingWrapper _savingWrapper;
        #endregion



        #region --Properties-- (With Backing Fields)
        public EUserInfoTab Tab
        {
            get => _defaultTab;

            set
            {
                _defaultTab = value;

                OnTabChanged?.Invoke();
            }
        }

        public EViewEditMode ViewEditMode
        {
            get => _defaultViewEditMode;

            set
            {
                _defaultViewEditMode = value;

                OnModeChanged?.Invoke(value);
            }
        }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void Start()
        {
            ShowMyUser();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~History Row~
        public async IAsyncEnumerable<(StayEntry, string)> GetRows(string userID)
        {
            //+Prevent duplicates Rows Bug. Since we are dealing with 'await' so we only allow ONE instance of this method to run at a time.
            //+Prevent some LeaderboardUI GameObject show Empty Data (No Rows), solve by make LeaderboardUI GameObject that comes after wait first then loads when Async is done.
            if (IsAsyncRunning) yield break;

            IsAsyncRunning = true;

            IAsyncEnumerable<(StayEntry, string)> rows = _savingWrapper.LoadPastEntryFromUser(userID);

            if (rows == null)
            {
                Debug.LogError("Error : There is no data on Server. Because 'rows' is null.");
                IsAsyncRunning = false;
                yield break;
            }

            await foreach ((StayEntry, string) eachData in rows)
            {
                yield return eachData;
            }

            IsAsyncRunning = false;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnViewEditButtonClick()
        {
            _onViewEditButtonClick?.Invoke();
        }

        public void OnUserProfileClickAsMySelf()
        {
            _onUserProfileClickAsMyself?.Invoke();
        }

        public void OnUserProfileClickAsOtherUser()
        {
            _onUserProfileClickAsOtherUser?.Invoke();
        }

        public void OnUserIDCardClickAsMyself()
        {
            print("User ID CARD clicked as Myself");

            _onUserIDCardClickAsMyself?.Invoke();
        }

        public void OnUserIDCardClickAsOtherUser()
        {
            print("User ID CARD clicked as Other User");

            _onUserIDCardClickAsOtherUser?.Invoke();
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
            _currentView = EUserInfoView.MyUser;

            Setup(_myUserData);
        }

        public void ShowOtherUser()
        {
            _currentView = EUserInfoView.OtherUser;
        }
        #endregion
    }
}