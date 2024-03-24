using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.SharePopup;

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
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private AspectRatioFitter _aspectRatioFitter;
        [SerializeField] private RectTransform _iconRect;
        [Space]
        [SerializeField] private Text _userNameText;
        [SerializeField] private Text _userLevelText;
        [SerializeField] private Button _modifyButton;

        [Header("User Stats")]
        [SerializeField] private Text _allTimeText;
        [SerializeField] private Text _todayText;
        [SerializeField] private Text _wonChallengeText;
        [SerializeField] private Text _memberSinceText;

        [Header("User Inventory")]
        [SerializeField] private Text _inventoryHeaderText;
        [SerializeField] private Text _inventoryValueText;
        [SerializeField] private ProfileIconUI[] _profileIconUI;
        #endregion



        #region --Fields-- (In Class)
        private AccountPopup _playerAccountPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerAccountPopup = GameObject.FindWithTag("Player").GetComponentInChildren<AccountPopup>();

            _closeButton.onClick.AddListener(Close);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerAccountPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        #endregion
    }
}