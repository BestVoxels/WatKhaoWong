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
        [SerializeField] private AccountData.IconUI _icon;
        [Space]
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _userLevelText;
        [SerializeField] private Button _modifyButton;

        [Header("User Stats")]
        [SerializeField] private TMP_Text _allTimeTMPointsText;
        [SerializeField] private TMP_Text _todayTMPointsText;
        [SerializeField] private TMP_Text _totalWonTMChallengeText;
        [SerializeField] private TMP_Text _memberSinceText;

        [Header("User Inventory")]
        [SerializeField] private TMP_Text _profilePicHeaderText;
        [SerializeField] private Transform _profileIconUIParent;
        #endregion



        #region --Fields-- (In Class)
        private readonly List<ProfileIconUI> _profileIcons = new List<ProfileIconUI>();

        private AccountPopup _playerAccountPopup;
        private AccountData _account;
        private StatusText _statusText;
        private SavingWrapper _savingWrapper;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 175f / 135f;  // Formula : Main Profile's Size (BIG) % Inventory Profile's Size (SMALL)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerAccountPopup = GameObject.FindWithTag("Player").GetComponentInChildren<AccountPopup>();
            _account = GameObject.FindWithTag("Player").GetComponentInChildren<AccountData>();
            _statusText = FindAnyObjectByType<StatusText>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();

            _closeButton.onClick.AddListener(Close);
            _modifyButton.onClick.AddListener(OnModifyButtonClicked);

            UIRefresher.OnPopupRefreshed += RefreshUI; // Can't use OnDisable() to unsubscribe Since the attached GameObject will be closed / also can't use OnEnable() cuz without OnDisable() it will keep adding more and more

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

            _account.UpdateProfileIcon(_icon, _account.GetProfileIcon(), MultiplierRatioForDecorator);

            _userNameText.text = _account.GetUserNameText();
            _userLevelText.text = _account.GetLevelText();

            _allTimeTMPointsText.text = _account.GetTotalTMPointsText();
            _todayTMPointsText.text = _account.GetTodayTMPointsText();

            _totalWonTMChallengeText.text = _account.GetTotalWonTMChallengeText();
            _memberSinceText.text = _account.GetMemberSinceText();

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
            ProfileIcon target = _account.GetProfileIcon();

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
        private void Close() => _playerAccountPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void OnToggleChangedByClick(ProfileIcon selectedProfileIcon, bool isOn)
        {
            if (isOn)
            {
                _account.UpdateProfileIcon(_icon, selectedProfileIcon, MultiplierRatioForDecorator);
                _savingWrapper.Save(ESaveName.ProfileIconID, selectedProfileIcon.ItemID);

                _playerAccountPopup.OnAccountProfileChangedByClick();
            }
        }

        private void OnModifyButtonClicked()
        {
            _statusText.Show(_playerAccountPopup.StatusInformUser, _playerAccountPopup.StatusInformUserColor);
        }
        #endregion
    }
}