using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.UI.Identities
{
    public class AccountPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private Button _closeButton;

        [Space]

        [Header("Account Popup UI Stuffs")]
        [Header("User Profile")]
        [SerializeField] private ProfileIconInspector _icon;
        [Space]
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _userTitleText;
        [SerializeField] private TMP_Text _userLevelText;
        [SerializeField] private Button _modifyButton;

        [Header("User Stats")]
        [SerializeField] private TMP_Text _allTimeTMPointsText;
        [SerializeField] private TMP_Text _todayTMPointsText;
        [SerializeField] private TMP_Text _challengeTMPointsText;
        [SerializeField] private TMP_Text _totalChallengeTMWonText;
        [SerializeField] private TMP_Text _memberSinceText;

        [Header("User Inventory")]
        [SerializeField] private TMP_Text _profilePicHeaderText;
        [SerializeField] private Transform _profileIconUIParent;
        #endregion



        #region --Fields-- (In Class)
        private readonly List<ProfileIconUI> _profileIcons = new List<ProfileIconUI>();
        private EAccountPopupView _currentView = EAccountPopupView.MyUser;

        private AccountPopup _accountPopup;
        private MyUserData _myUserData;
        private IUserData _userData;
        private StatusText _statusText;
        private SavingWrapper _savingWrapper;
        private Localizer _localizer;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 175f / 135f;  // Formula : [CHANGE THIS] AccountPopupUI Profile's Size  %  [FIX] Inventory Profile's Size (original looks)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _accountPopup = GameObject.FindWithTag("Player").GetComponentInChildren<AccountPopup>();
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
            _statusText = FindAnyObjectByType<StatusText>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _localizer = FindAnyObjectByType<Localizer>();

            _userData = _myUserData;

            _closeButton.onClick.AddListener(Close);
            _modifyButton.onClick.AddListener(OnModifyButtonClicked);

            UIRefresher.OnPopupRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
            UIRefresher.OnLocalizeDynamicString += () => _userTitleText.text = _localizer.LocalizeUserTitle(_userData.GetTitleText());

            _accountPopup.OnViewSetup += SetupNewView; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

            PopulateProfileIconList();
        }

        private void Start()
        {
            RefreshUI();

            SubscribeEachProfileIconWithOnToggleChanged(); // Must Run after RefreshToggleStatusUI() (only in the beginning) to avoid overriding save file of ProfileIconUI, overrided by its default state.
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            RefreshToggleStatusUI();

            RefreshHeaderUI();

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            _userData.UpdateProfileIcon(_icon, _userData.GetProfileIcon(), MultiplierRatioForDecorator);

            _userNameText.text = _userData.GetUserNameText();
            _userTitleText.text = _localizer.LocalizeUserTitle(_userData.GetTitleText());
            _userLevelText.text = _userData.GetLevelText();

            _allTimeTMPointsText.text = _userData.GetTotalTMPointsText();
            _todayTMPointsText.text = _userData.GetTodayTMPointsText();
            _challengeTMPointsText.text = _userData.GetChallengeTMPointsText();

            _totalChallengeTMWonText.text = _userData.GetTotalChallengeTMWonText();
            _memberSinceText.text = _userData.GetMemberSinceText();

            _profilePicHeaderText.text = $"<#f8913f>{_profileIcons.Count.ToString("#,0", nfi)}</color>";
        }

        private void PopulateProfileIconList()
        {
            foreach (Transform child in _profileIconUIParent)
            {
                if (child.TryGetComponent(out ProfileIconUI profileIcon))
                    _profileIcons.Add(profileIcon);
            }
        }

        /// <summary>
        /// Turn on Toggle that matches with Player's selected ProfileIcon.
        /// </summary>
        private void RefreshToggleStatusUI()
        {
            ProfileIconItem target = _userData.GetProfileIcon();

            _profileIcons.ForEach(eachIconUI =>
            {
                if (eachIconUI.Icon.ItemID.Equals(target.ItemID))
                    eachIconUI.Toggle.isOn = true;
            });
        }

        private void SubscribeEachProfileIconWithOnToggleChanged()
        {
            _profileIcons.ForEach(eachIconUI => eachIconUI.OnToggleChanged += OnToggleChangedByClick);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Tab~
        private void RefreshHeaderUI()
        {
            _headerText.text = _currentView switch
            {
                EAccountPopupView.MyUser => _accountPopup.MyInfoTitleText.GetLocalizedString(),
                EAccountPopupView.OtherUser => _accountPopup.UserInfoTitleText.GetLocalizedString(),
                _ => ""
            };
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _accountPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void OnToggleChangedByClick(ProfileIconItem selectedProfileIcon, bool isOn)
        {
            if (isOn)
            {
                _userData.UpdateProfileIcon(_icon, selectedProfileIcon, MultiplierRatioForDecorator);
                _userData.SaveProfileIcon(selectedProfileIcon);

                _accountPopup.OnAccountProfileChangedByClick();
            }
        }

        private void OnModifyButtonClicked()
        {
            _statusText.Show(_accountPopup.StatusInformUser.GetLocalizedString(), _accountPopup.StatusInformUserColor);
        }

        private void SetupNewView(EAccountPopupView newView, IUserData userData)
        {
            _currentView = newView;
            _userData = userData;

            RefreshUI();
        }
        #endregion
    }
}