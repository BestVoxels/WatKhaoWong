using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.SharePopup;
using WatKhaoWong.UI.System;
using WatKhaoWong.Attributes;

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

        private AccountPopup _playerAccountPopup;
        private Account _playerAccount;
        private StatusText _statusText;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 175f / 135f;  // Formula : Main Profile's Size (BIG) % Inventory Profile's Size (SMALL)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerAccountPopup = GameObject.FindWithTag("Player").GetComponentInChildren<AccountPopup>();
            _playerAccount = GameObject.FindWithTag("Player").GetComponentInChildren<Account>();
            _statusText = FindAnyObjectByType<StatusText>();

            _closeButton.onClick.AddListener(Close);
            _modifyButton.onClick.AddListener(OnModifyButtonClicked);

            foreach (Transform child in _profileIconUIParent)
            {
                if (child.TryGetComponent(out ProfileIconUI profileIconUI))
                {
                    profileIconUI.OnToggleChanged += OnToggleChanged;

                    _totalProfileIcon++;
                }
            }
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            _userNameText.text = _playerAccount.GetUserNameText();
            _userLevelText.text = _playerAccount.GetUserLevelText();

            _allTimeTMPointsText.text = _playerAccount.GetAllTimeTMPoints().ToString("#,0", nfi);
            _todayTMPointsText.text = _playerAccount.GetTodayTMPoints().ToString("#,0", nfi);

            _totalWonTMChallengeText.text = _playerAccount.GetTotalWonTMChallenge().ToString("#,0", nfi);
            _memberSinceText.text = _playerAccount.GetMemberSinceText();

            _profilePicHeaderText.text = $"<#f8913f>{_totalProfileIcon.ToString("#,0", nfi)}</color>";
        }

        private void UpdateProfile(ProfileIconUI profileIconUI)
        {
            // Clear Spawned Decorators (no error if there are not)
            foreach (Transform each in _icon.decoratorSpawnParent)
                Destroy(each.gameObject);

            // Replicate Toggle Profile to Main Profile
            _icon.backgroundImage.color = profileIconUI.BackgroundColor;
            _icon.iconImage.overrideSprite = profileIconUI.Icon;
            _icon.aspectRatioFitter.aspectRatio = profileIconUI.AspectRatio;
            _icon.iconRect.pivot = profileIconUI.IconPivotY;

            foreach (GameObject each in profileIconUI.Decorators)
            {
                GameObject result = Instantiate(each, _icon.decoratorSpawnParent, false);

                RectTransform rt = result.GetComponent<RectTransform>();
                rt.localPosition = new Vector2(rt.localPosition.x * MultiplierRatioForDecorator, rt.localPosition.y * MultiplierRatioForDecorator);
                rt.sizeDelta = new Vector2(rt.sizeDelta.x * MultiplierRatioForDecorator, rt.sizeDelta.y * MultiplierRatioForDecorator);
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerAccountPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void OnToggleChanged(ProfileIconUI profileIconUI, bool isOn)
        {
            if (isOn)
            {
                UpdateProfile(profileIconUI);

                _playerAccountPopup.OnAccountProfileChanged();

            }
        }

        private void OnModifyButtonClicked()
        {
            _statusText.Show(_playerAccountPopup.StatusInformUser, _playerAccountPopup.StatusInformUserColor);
        }
        #endregion
    }
}