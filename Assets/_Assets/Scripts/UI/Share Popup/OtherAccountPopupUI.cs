using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Identity;
using WatKhaoWong.SharePopup;

namespace WatKhaoWong.UI.SharePopup
{
    public class OtherAccountPopupUI : MonoBehaviour
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

        [Header("User Stats")]
        [SerializeField] private TMP_Text _allTimeTMPointsText;
        [SerializeField] private TMP_Text _todayTMPointsText;
        [SerializeField] private TMP_Text _totalWonTMChallengeText;
        [SerializeField] private TMP_Text _memberSinceText;
        #endregion



        #region --Fields-- (In Class)
        private OtherAccountPopup _otherAccountPopup;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 175f / 135f;  // Formula : Main Profile's Size (BIG) % Inventory Profile's Size (SMALL)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _otherAccountPopup = GameObject.FindWithTag("Player").GetComponentInChildren<OtherAccountPopup>();

            _closeButton.onClick.AddListener(Close);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(AccountData account)
        {
            RefreshUI(account);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI(AccountData account)
        {
            account.UpdateProfileIcon(_icon, account.GetProfileIcon(), MultiplierRatioForDecorator);

            _userNameText.text = account.GetUserNameText();
            _userLevelText.text = account.GetLevelText();

            _allTimeTMPointsText.text = account.GetTotalTMPointsText();
            _todayTMPointsText.text = account.GetTodayTMPointsText();

            _totalWonTMChallengeText.text = account.GetTotalWonTMChallengeText();
            _memberSinceText.text = account.GetMemberSinceText();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _otherAccountPopup.OnCloseButtonClick();
        #endregion
    }
}