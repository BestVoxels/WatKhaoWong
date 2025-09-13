using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.Localization;

namespace WatKhaoWong.UI.Identities
{
    public class OtherAccountPopupUI : MonoBehaviour
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
        [SerializeField] private TMP_Text _userTitleText;
        [SerializeField] private TMP_Text _userLevelText;

        [Header("User Stats")]
        [SerializeField] private TMP_Text _allTimeTMPointsText;
        [SerializeField] private TMP_Text _todayTMPointsText;
        [SerializeField] private TMP_Text _challengeTMPointsText;
        [SerializeField] private TMP_Text _totalChallengeTMWonText;
        [SerializeField] private TMP_Text _memberSinceText;
        #endregion



        #region --Fields-- (In Class)
        private IUserData _userData;
        private OtherAccountPopup _otherAccountPopup;
        private Localizer _localizer;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 175f / 135f;  // Formula : [CHANGE THIS] OtherAccountPopupUI Profile's Size  %  [FIX] Inventory Profile's Size (original looks)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _otherAccountPopup = GameObject.FindWithTag("Player").GetComponentInChildren<OtherAccountPopup>();
            _localizer = FindAnyObjectByType<Localizer>();

            _closeButton.onClick.AddListener(Close);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(IUserData userData)
        {
            _userData = userData;

            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            _userData.UpdateProfileIcon(_icon, _userData.GetProfileIcon(), MultiplierRatioForDecorator);

            _userNameText.text = _userData.GetUserNameText();
            _userTitleText.text = _localizer.LocalizeUserTitle(_userData.GetTitleText());
            _userLevelText.text = _userData.GetLevelText();

            _allTimeTMPointsText.text = _userData.GetTotalTMPointsText();
            _todayTMPointsText.text = _userData.GetTodayTMPointsText();
            _challengeTMPointsText.text = _userData.GetChallengeTMPointsText();

            _totalChallengeTMWonText.text = _userData.GetTotalChallengeTMWonText();
            _memberSinceText.text = _userData.GetMemberSinceText();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _otherAccountPopup.OnCloseButtonClick();
        #endregion
    }
}