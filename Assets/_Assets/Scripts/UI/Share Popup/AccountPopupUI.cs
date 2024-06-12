using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.SharePopup;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Identity;

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
        [SerializeField] private Account.IconUI _icon;
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
        private byte _totalProfileIcon;
        private readonly List<ProfileIcon> _profileIcons = new List<ProfileIcon>();

        private AccountPopup _playerAccountPopup;
        private Account _account;
        private StatusText _statusText;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 175f / 135f;  // Formula : Main Profile's Size (BIG) % Inventory Profile's Size (SMALL)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerAccountPopup = GameObject.FindWithTag("Player").GetComponentInChildren<AccountPopup>();
            _account = GameObject.FindWithTag("Player").GetComponentInChildren<Account>();
            _statusText = FindAnyObjectByType<StatusText>();

            _closeButton.onClick.AddListener(Close);
            _modifyButton.onClick.AddListener(OnModifyButtonClicked);

            foreach (Transform child in _profileIconUIParent)
            {
                if (child.TryGetComponent(out ProfileIcon profileIcon))
                {
                    profileIcon.OnToggleChanged += OnToggleChanged;

                    _profileIcons.Add(profileIcon);
                    _totalProfileIcon++;
                }
            }
        }

        private void Start()
        {
            RefreshToggleStatusOnStart();

            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        /// <summary>
        /// Turn on Toggle to matches with Player's selected ProfileIcon.
        /// </summary>
        private void RefreshToggleStatusOnStart()
        {
            Account.IconData iconData = _account.GetIconData();
            List<ProfileIcon> temp = _profileIcons.Where(
                (ProfileIcon p) =>
                {
                    bool result = p.UI.Icon.Equals(iconData.icon);
                    if (result)
                        p.Toggle.isOn = true;

                    return result;
                }
            ).ToList();
        }

        private void RefreshUI()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            _account.UpdateProfileIcon(_icon, _account.GetIconData(), MultiplierRatioForDecorator);

            _userNameText.text = _account.GetUserNameText();
            _userLevelText.text = _account.GetUserLevelText();

            _allTimeTMPointsText.text = _account.GetAllTimeTMPoints().ToString("#,0", nfi);
            _todayTMPointsText.text = _account.GetTodayTMPoints().ToString("#,0", nfi);

            _totalWonTMChallengeText.text = _account.GetTotalWonTMChallenge().ToString("#,0", nfi);
            _memberSinceText.text = _account.GetMemberSinceText();

            _profilePicHeaderText.text = $"<#f8913f>{_totalProfileIcon.ToString("#,0", nfi)}</color>";
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerAccountPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void OnToggleChanged(Account.IconUI selectedIconUI, bool isOn)
        {
            if (isOn)
            {
                _account.UpdateProfileIcon(_icon, selectedIconUI, MultiplierRatioForDecorator);

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