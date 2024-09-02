using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.SharePopup;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Identity;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.UI.SharePopup
{
    public class AccountPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Space]

        [Header("Account Popup UI Stuffs")]
        [Header("User Profile")]
        [SerializeField] private ProfileIconInspector _icon;
        [Space]
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _userLevelText;
        [SerializeField] private Button _modifyButton;

        [Header("User Stats")]
        [SerializeField] private TMP_Text _allTimeTMPointsText;
        [SerializeField] private TMP_Text _todayTMPointsText;
        [SerializeField] private TMP_Text _challengeTMPointsText;
        [SerializeField] private TMP_Text _totalWonTMChallengeText;
        [SerializeField] private TMP_Text _memberSinceText;

        [Header("User Inventory")]
        [SerializeField] private TMP_Text _profilePicHeaderText;
        [SerializeField] private Transform _profileIconUIParent;
        #endregion



        #region --Fields-- (In Class)
        private readonly List<ProfileIconUI> _profileIcons = new List<ProfileIconUI>();

        private AccountPopup _accountPopup;
        private MyUserData _myUserData;
        private StatusText _statusText;
        private SavingWrapper _savingWrapper;
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

            _closeButton.onClick.AddListener(Close);
            _modifyButton.onClick.AddListener(OnModifyButtonClicked);

            UIRefresher.OnPopupRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

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

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            _myUserData.UpdateProfileIcon(_icon, _myUserData.GetProfileIcon(), MultiplierRatioForDecorator);

            _userNameText.text = _myUserData.GetUserNameText();
            _userLevelText.text = _myUserData.GetLevelText();

            _allTimeTMPointsText.text = _myUserData.GetTotalTMPointsText();
            _todayTMPointsText.text = _myUserData.GetTodayTMPointsText();
            _challengeTMPointsText.text = _myUserData.GetChallengeTMPointsText();

            _totalWonTMChallengeText.text = _myUserData.GetTotalWonTMChallengeText();
            _memberSinceText.text = _myUserData.GetMemberSinceText();

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
            ProfileIconItem target = _myUserData.GetProfileIcon();

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



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _accountPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void OnToggleChangedByClick(ProfileIconItem selectedProfileIcon, bool isOn)
        {
            if (isOn)
            {
                _myUserData.UpdateProfileIcon(_icon, selectedProfileIcon, MultiplierRatioForDecorator);
                _savingWrapper.Save(ECategoryNode.Users, EValueNode.ProfileIconID, selectedProfileIcon.ItemID);

                _accountPopup.OnAccountProfileChangedByClick();
            }
        }

        private void OnModifyButtonClicked()
        {
            _statusText.Show(_accountPopup.StatusInformUser, _accountPopup.StatusInformUserColor);
        }
        #endregion
    }
}