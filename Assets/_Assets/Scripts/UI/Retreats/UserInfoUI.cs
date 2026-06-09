using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.SceneManagement;
using UnityEngine.EventSystems;
using System;
using WatKhaoWong.Utils.Core;
using Firebase.Auth;
using WatKhaoWong.UI.Leaderboards;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.UI.Retreats
{
    [RequireComponent(typeof(StayEntryRowUIPool))]
    public class UserInfoUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        [Header("UserInfo UI Stuffs - Main")]
        [SerializeField] private Transform _tabsTransform;
        [SerializeField] private Button _viewEditButton;
        [Space]
        [SerializeField] private GameObject _generalInfoGameObject;
        [SerializeField] private GameObject _personalInfoGameObject;
        [SerializeField] private GameObject[] _toShowHideByMode;
        [SerializeField] private GameObject[] _toShowHideByModeOnlyAdmin;

        [Header("UserInfo UI Stuffs - General Info")]
        [SerializeField] private ProfileIconInspector _icon;
        // TODO create one for National ID Card Image
        [SerializeField] private EventTrigger _userProfileEventTrigger;
        [SerializeField] private EventTrigger _userIDCardEventTrigger;
        [Space]
        [SerializeField] private AccountStatusInspector _accountStatusUI;
        [SerializeField] private MiniInfoInspector _miniInfoInspectorUI;
        [SerializeField] private StayEntryRowUI _adderEntryUI; // TODO maybe useful for something in the future.
        [SerializeField] private StayEntryRowUI _pendingEntryUI;
        [SerializeField] private StayEntryRowUI _currentEntryUI;
        [Space]
        [SerializeField] private GameObject[] _pendingApprovalGameObjects;
        [SerializeField] private GameObject[] _currentStayGameObjects;
        [SerializeField] private GameObject _historyNoDataGameObject;

        [Header("UserInfo UI Stuffs - Personal Info")]
        [SerializeField] private PersonalRowUI _personalRowUI; // TODO maybe useful for something in the future.
        #endregion



        #region --Fields-- (In Class)
        private List<StayEntryRowUI> _activeRowUIs = new List<StayEntryRowUI>();
        private bool _refreshedOnce = false;
        private EUserInfoView _currentView = EUserInfoView.MyUser;

        private MyUserData _myUserData;
        private IUserData _userData;
        private UserInfo _userInfo;
        private AccountPopup _accountPopup;
        private StayEntryRow _stayEntryRow;
        private AccommodationForm _accommodationForm;
        private StatusSetter _statusSetter;
        private Localizer _localizer;
        private ServerTime _serverTime;
        private StayEntryRowUIPool _rowUIPool;
        #endregion



        #region --Fields-- (Constant)
        private const float WaitAsyncTimeOut = 10f;
        private const float MultiplierRatioForDecorator = 160f / 135f;  // Formula : [CHANGE THIS] UserUI Profile's Size  %  [FIX] Inventory Profile's Size (original looks)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _userInfo = player.GetComponentInChildren<UserInfo>();
            _accountPopup = player.GetComponentInChildren<AccountPopup>();
            _stayEntryRow = player.GetComponentInChildren<StayEntryRow>();
            _accommodationForm = player.GetComponentInChildren<AccommodationForm>();
            _statusSetter = player.GetComponentInChildren<StatusSetter>();
            _localizer = FindAnyObjectByType<Localizer>();
            _serverTime = FindAnyObjectByType<ServerTime>();
            _rowUIPool = GetComponent<StayEntryRowUIPool>();

            _userData = _myUserData;

            // Main
            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            _viewEditButton.onClick.AddListener(ViewEdit);

            // General Info
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => UserProfile((PointerEventData)data));
            _userProfileEventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => UserIDCard((PointerEventData)data));
            _userIDCardEventTrigger.triggers.Add(entry);

            // Others
            _stayEntryRow.OnAddedToServer += (stayEntry, stayStatus) => { BuildAllStayEntryRowUIAgain(); RefreshViewEditMode(); };
            _stayEntryRow.OnDeletedFromServer += (nullEntry) => BuildAllStayEntryRowUIAgain();
            _accommodationForm.OnUploadedToServer += (stayEntry, stayStatus) => BuildAllStayEntryRowUIAgain();

            UIRefresher.OnUserInfoRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
            UIRefresher.OnLocalizeDynamicString += RefreshUI;// Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

            _userInfo.OnViewSetup += SetupNewView; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
        }
        
        private void OnEnable()
        {
            _statusSetter.OnUploadedToServer += StatusSetterUploadToServer;
        }

        private void Start()
        {
            RefreshUI();

            SetupTabsUI();
        }

        private void OnDestroy()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged; // We need to Unsubscribe because this is 'FirebaseAuth' not out Event. So we can't know for sure when it will be reset.
        }

        private void OnDisable()
        {
            _statusSetter.OnUploadedToServer -= StatusSetterUploadToServer;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            RefreshGroupsToShowHide();

            RefreshHeaderUI();

            RefreshTabsUI();

            RefreshViewEditMode();

            RefreshProfileIconUI();
            RefreshAccountStatusUI();
            RefreshMiniInfoUI();

            BuildAllStayEntryRowUI();
        }

        private void RefreshProfileIconUI() => _userData.UpdateProfileIcon(_icon, _userData.GetProfileIcon(), MultiplierRatioForDecorator);

        private void RefreshAccountStatusUI() => _userData.UpdateAccountStatus(_accountStatusUI, _userData.GetAccountStatus(), _localizer);

        private async void RefreshMiniInfoUI() => _userData.UpdateMiniInfo(_miniInfoInspectorUI, await _userData.GetDataNationalIDInfo(), await _userData.GetDataPassportInfo(), _localizer, _serverTime);
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Rows~
        private void BuildAllStayEntryRowUIAgain()
        {
            _refreshedOnce = false;
            BuildAllStayEntryRowUI();
        }

        private async void BuildAllStayEntryRowUI()
        {
            if (_refreshedOnce) return;

            await BuildHistoryRows();

            BuildPendingOrActiveRows();

            _refreshedOnce = true;
        }

        private async Task BuildHistoryRows()
        {
            ClearRows();

            //+Prevent some LeaderboardUI GameObject show Empty Data (No Rows), solve by make LeaderboardUI GameObject that comes after wait first then loads when Async is done.
            float timer = 0f;
            while (UserInfo.IsAsyncRunning == true)
            {
                timer += Time.deltaTime;

                if (timer >= WaitAsyncTimeOut) return;

                await Task.Delay(100);
            }

            ClearRows(); //+Prevent duplicates Rows Bug.

            short rowCounter = 1;
            await foreach ((StayEntry StayEntry, string KeyId) each in _userInfo.GetRows(_userData.GetUserKeyID()))
            {
                StayEntryRowUI createdPrefab = _rowUIPool.Pool.Get();

                createdPrefab.transform.SetSiblingIndex(rowCounter - 1); // -1 bcuz Index starts at 0.
                createdPrefab.Setup(each.StayEntry, each.KeyId,  rowCounter);

                _activeRowUIs.Add(createdPrefab);

                ++rowCounter;
            }

            if (_activeRowUIs.Count == 0)
                ShowHideHistoryNoDataText(true);
            else
                ShowHideHistoryNoDataText(false);
        }

        private async void BuildPendingOrActiveRows()
        {
            ShowHidePendingEntryUI(false);
            ShowHideCurrentEntryUI(false);

            StayEntry stayEntry = await _userData.GetActiveStayEntry();
            if (stayEntry == null) return;

            ActiveStay activeStay = await _userData.GetDataActiveStay();
            if (activeStay == null) return;

            Enum.TryParse(stayEntry.StatusInfo.Status, true, out EStayStatus eStatus);
            switch (eStatus)
            {
                case EStayStatus.Pending:
                    ShowHidePendingEntryUI(true);
                    _pendingEntryUI.Setup(stayEntry, activeStay.KeyId);
                    break;

                case EStayStatus.Scheduled:
                case EStayStatus.Active:
                    ShowHideCurrentEntryUI(true);
                    _currentEntryUI.Setup(stayEntry, activeStay.KeyId);
                    break;
            }
        }

        private void ClearRows()
        {
            foreach (StayEntryRowUI eachRow in _activeRowUIs)
                _rowUIPool.Pool.Release(eachRow);

            _activeRowUIs.Clear();
        }

        private void ShowHidePendingEntryUI(bool toShow)
        {
            foreach (GameObject each in _pendingApprovalGameObjects)
                each.SetActive(toShow);
        }

        private void ShowHideCurrentEntryUI(bool toShow)
        {
            foreach (GameObject each in _currentStayGameObjects)
                each.SetActive(toShow);
        }

        private void ShowHideHistoryNoDataText(bool toShow)
        {
            _historyNoDataGameObject.SetActive(toShow);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Tab~
        private void RefreshHeaderUI()
        {
            if (_currentView == EUserInfoView.MyUser)
            {
                _headerText.text = _userInfo.ViewEditMode switch
                {
                    EViewEditMode.View => _userInfo.MyProfileTitleText.GetLocalizedString(),
                    EViewEditMode.Edit => _userInfo.EditMyProfileTitleText.GetLocalizedString(),
                    _ => ""
                };
            }
            else if (_currentView == EUserInfoView.OtherUser)
            {
                _headerText.text = _userInfo.ViewEditMode switch
                {
                    EViewEditMode.View => _userInfo.UserProfileTitleText.GetLocalizedString(),
                    EViewEditMode.Edit => _userInfo.EditUserProfileTitleText.GetLocalizedString(),
                    _ => ""
                };
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Tab~
        private void RefreshGroupsToShowHide()
        {
            switch (_userInfo.Tab)
            {
                case EUserInfoTab.GeneralInfo:
                    _generalInfoGameObject.SetActive(true);
                    _personalInfoGameObject.SetActive(false);
                    break;

                case EUserInfoTab.PersonalInfo:
                    _generalInfoGameObject.SetActive(false);
                    _personalInfoGameObject.SetActive(true);
                    break;
            }
        }

        private void SetupTabsUI()
        {
            foreach (UserInfoTabUI tab in _tabsTransform.GetComponentsInChildren<UserInfoTabUI>())
            {
                tab.Setup(_userInfo);
            }
        }

        private void RefreshTabsUI()
        {
            foreach (UserInfoTabUI tab in _tabsTransform.GetComponentsInChildren<UserInfoTabUI>())
            {
                tab.UpdateColor();
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~View Edit Button~
        private void RefreshViewEditMode()
        {
            RefreshViewEditButtonText();

            ShowHideUIByViewEditMode();
        }

        private void RefreshViewEditButtonText()
        {
            TMP_Text buttonText = _viewEditButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = (_userInfo.ViewEditMode == EViewEditMode.View) ? _userInfo.EditButtonText.GetLocalizedString() : _userInfo.ViewButtonText.GetLocalizedString();
        }

        private void ShowHideUIByViewEditMode()
        {
            bool showOnEditMode = _userInfo.ViewEditMode == EViewEditMode.Edit;

            foreach (GameObject each in _toShowHideByMode)
                each.SetActive(showOnEditMode);

            foreach (GameObject each in _toShowHideByModeOnlyAdmin)
            {
                if (_myUserData.GetRole() == EUserRole.Admin)
                    each.SetActive(showOnEditMode);
                else
                    each.SetActive(false);
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _userInfo.OnBackButtonClick();
        private void ChangeLang() => _userInfo.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void ViewEdit()
        {
            _userInfo.ViewEditMode = (_userInfo.ViewEditMode == EViewEditMode.View) ? EViewEditMode.Edit : EViewEditMode.View;

            RefreshViewEditButtonText();

            _userInfo.OnViewEditButtonClick();
        }

        private void UserProfile(PointerEventData data)
        {
            // IF Mode is View Mode then Enter edit Mode
            if (_userInfo.ViewEditMode == EViewEditMode.View)
            {
                ViewEdit();
                return;
            }

            // Open Popup
            if (_currentView == EUserInfoView.MyUser)
                _userInfo.OnUserProfileClickAsMySelf();
            else if (_currentView == EUserInfoView.OtherUser)
            {
                _userInfo.OnUserProfileClickAsOtherUser();
                _accountPopup.Setup(_userData);
            }
        }

        private void UserIDCard(PointerEventData data)
        {
            // IF Mode is View Mode then Enter edit Mode
            if (_userInfo.ViewEditMode == EViewEditMode.View)
            {
                ViewEdit();
                return;
            }

            // Open Popup
            if (_currentView == EUserInfoView.MyUser)
                _userInfo.OnUserIDCardClickAsMyself();
            else if (_currentView == EUserInfoView.OtherUser)
                _userInfo.OnUserIDCardClickAsOtherUser();
        }

        private void StatusSetterUploadToServer()
        {
            // When StatusSetter upload to server...
            RefreshAccountStatusUI();
        }

        private void SetupNewView(EUserInfoView newView, IUserData userData)
        {
            _currentView = newView;
            _userData = userData;

            RefreshUI();

            BuildAllStayEntryRowUIAgain();
        }

        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake(). And at time of assiging to 'FirebaseAuth.DefaultInstance.StateChanged'
        /// </summary>
        private void HandleStateChanged(object obj, EventArgs args)
        {
            _refreshedOnce = false;
        }
        #endregion
    }
}