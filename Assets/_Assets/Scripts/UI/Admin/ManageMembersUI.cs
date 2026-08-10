using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WatKhaoWong.Admin;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.UI.Admin
{
    public class ManageMembersUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        [Header("ManageMembers UI Stuffs")]
        [SerializeField] private EventTrigger _userProfileEventTrigger;
        [SerializeField] private EventTrigger _userStatsEventTrigger;
        [Space]
        [SerializeField] private ProfileIconInspector _icon;
        [Space]
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _totalUsersText;
        [SerializeField] private TMP_Text _activeStayText;
        [Space]
        [SerializeField] private Button _searchEditMemberButton;
        [SerializeField] private Button _stayApprovalButton;
        [SerializeField] private Button _activityManagementButton;
        [SerializeField] private Button _registerMemberButton;
        #endregion


        // TODO ---REMOVE when don't have to Test Upload/Download/SaveImage Features!!!---
        [SerializeField] Button _uploadImage;
        [SerializeField] Button _downloadImage;
        [SerializeField] Button _saveToGallery;
        // TODO ---REMOVE when don't have to Test Upload/Download/SaveImage Features!!!---


        #region --Fields-- (In Class)
        private int _totalUsersNumber = 0;
        private int _activeUsersNumber = 0;

        private ManageMembers _manageMembers;
        private MyUserData _myUserData;
        private SavingWrapper _savingWrapper;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 160f / 135f;  // Formula : [CHANGE THIS] ManageMembersUI Profile's Size  %  [FIX] Inventory Profile's Size (original looks)
        private const float WaitUIToTurnOffOnStartTime = 3.5f;
        #endregion

        

        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject _player = GameObject.FindWithTag("Player");
            _manageMembers = _player.GetComponentInChildren<ManageMembers>();
            _myUserData = _player.GetComponentInChildren<MyUserData>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => UserProfile((PointerEventData)data));
            _userProfileEventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => UserStats((PointerEventData)data));
            _userStatsEventTrigger.triggers.Add(entry);

            _searchEditMemberButton.onClick.AddListener(SearchEditMember);
            _stayApprovalButton.onClick.AddListener(StayApproval);
            _activityManagementButton.onClick.AddListener(ActivityManagement);
            _registerMemberButton.onClick.AddListener(RegisterMember);

            // TODO ---REMOVE when don't have to Test Upload/Download/SaveImage Features!!!---
            _uploadImage.onClick.AddListener(() => _manageMembers.TestUploadImage());
            _downloadImage.onClick.AddListener(() => _manageMembers.TestDownloadImageNAssign());
            _saveToGallery.onClick.AddListener(() => _manageMembers.TestSaveImageToGallery());
            // TODO ---REMOVE when don't have to Test Upload/Download/SaveImage Features!!!---

            UIRefresher.OnManageMembersRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

            UIRefresher.OnLocalizeDynamicString += RefreshStatUI;
        }

        private async void Start()
        {
            if (!await MyUserData.IsAdmin()) return;

            RefreshUI();
            UpdateTotalNumbers();
        }

        private async void OnEnable()
        {
            if (Time.time < WaitUIToTurnOffOnStartTime) return; // Prevent OnEnable() on first Start when UI are seting itself which then it will hide itself. We only want OnEnable() when user open UI.
            if (!await MyUserData.IsAdmin()) return;

            RefreshUI();
            UpdateTotalNumbers();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async void UpdateTotalNumbers()
        {
            if (!await MyUserData.IsAdmin()) return;

            int count = 0;
            await foreach (var snapshot in _savingWrapper.LoadAllUsers())
                count++;

            _totalUsersNumber = count;

            count = 0;
            await foreach (var snapshot in _savingWrapper.LoadEntryFromCategory(ECategoryNode.ActiveStay))
                count++;

            _activeUsersNumber = count;
            RefreshStatUI();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _manageMembers.OnBackButtonClick();
        private void ChangeLang() => _manageMembers.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void UserProfile(PointerEventData data) => _manageMembers.OnUserProfileClick();

        private void UserStats(PointerEventData data) => _manageMembers.OnUserStatsClick();

        private void SearchEditMember()
        {
            _manageMembers.OnSearchEditMemberButtonClick();
        }

        private void StayApproval()
        {
            _manageMembers.OnStayApprovalButtonClick();
        }

        private void ActivityManagement()
        {
            _manageMembers.OnActivityManagementButtonClick();
        }

        private void RegisterMember()
        {
            _manageMembers.OnRegisterMemberButtonClick();
        }

        private void RefreshUI()
        {
            _myUserData.UpdateProfileIcon(_icon, _myUserData.GetProfileIcon(), MultiplierRatioForDecorator);

            RefreshStatUI();
        }

        private void RefreshStatUI()
        {
            _userNameText.text = _myUserData.GetUserNameText();

            _totalUsersText.text = _manageMembers.TotalUsersText.GetLocalizedString($"{_manageMembers.ValueTextFormatBegin}{(_totalUsersNumber == 0 ? "-" : _totalUsersNumber.ToString())}{_manageMembers.ValueTextFormatEnd}");
            _activeStayText.text = _manageMembers.ActiveStayText.GetLocalizedString($"{_manageMembers.ValueTextFormatBegin}{(_activeUsersNumber == 0 ? "-" : _activeUsersNumber.ToString())}{_manageMembers.ValueTextFormatEnd}");
        }
        #endregion
    }
}