using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WatKhaoWong.Identity;
using WatKhaoWong.Leaderboards;
using WatKhaoWong.UI.SharePopup;

namespace WatKhaoWong.UI.Leaderboards
{
    public class RowUI : MonoBehaviour
    {
        public enum IsInLeaderboard
        {
            Yes,
            No
        }

        private enum RowType
        {
            Myself,
            OtherUser
        }



        #region --Fields-- (Inspector)
        [Header("Row UI Stuffs")]
        [SerializeField] private RowType _rowType = RowType.OtherUser;
        [Space]
        [SerializeField] private Button _rowButton;

        [Space]

        [Header("Rank")]
        [SerializeField] private GameObject _firstRankGameObject;
        [SerializeField] private GameObject _secondRankGameObject;
        [SerializeField] private GameObject _thirdRankGameObject;
        [SerializeField] private TMP_Text _rankText;

        [Header("Profile Icon")]
        [SerializeField] private ProfileIconInspector _icon;

        [Header("Profile Name")]
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _levelText;

        [Header("Stats")]
        [SerializeField] private TMP_Text _scoreText;
        #endregion



        #region --Fields-- (In Class)
        private Row _row;
        private IUserData _userData;
        private OtherAccountPopupUI _otherAccountPopupUI;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 112f / 135f;  // Formula : [CHANGE THIS] RowUI Profile's Size  %  [FIX] Inventory Profile's Size (original looks)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _row = GameObject.FindWithTag("Player").GetComponentInChildren<Row>();
            _otherAccountPopupUI = FindAnyObjectByType<OtherAccountPopupUI>(FindObjectsInactive.Include);

            _rowButton.onClick.AddListener(RowClick);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(IUserData userdata, ushort rankNumber, ELeaderboardCategory category, IsInLeaderboard isInLeaderboard)
        {
            _userData = userdata;

            RefreshUI(category);

            UpdateRankUI(rankNumber, isInLeaderboard);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI(ELeaderboardCategory category)
        {
            if (_userData == default)
            {
                Debug.LogError("CUSTOM Error : RowUI.cs is created BUT havn't Setup() yet! Must call Setup() method first!");
                return;
            }

            _userData.UpdateProfileIcon(_icon, _userData.GetProfileIcon(), MultiplierRatioForDecorator);

            _userNameText.text = _userData.GetUserNameText();
            _levelText.text = _userData.GetLevelText();

            _scoreText.text = category switch
            {
                ELeaderboardCategory.AllTime => _userData.GetTotalTMPointsText(),
                ELeaderboardCategory.Today => _userData.GetTodayTMPointsText(),
                ELeaderboardCategory.Challenge => "temp challenge score",
                _ => _row.DefaultNullScoreText
            };
        }

        private void UpdateRankUI(ushort rankNumber, IsInLeaderboard isInLeaderboard)
        {
            if (rankNumber == default)
            {
                Debug.LogError("CUSTOM Error : RowUI.cs is created BUT havn't Setup() yet! Must call Setup() method first!");
                return;
            }

            // CloseAllRankUI
            _firstRankGameObject.SetActive(false);
            _secondRankGameObject.SetActive(false);
            _thirdRankGameObject.SetActive(false);
            _rankText.gameObject.SetActive(false);

            // Open Accordingly
            if (rankNumber == 1 && isInLeaderboard == IsInLeaderboard.Yes)
            {
                _firstRankGameObject.SetActive(true);
            }
            else if (rankNumber == 2 && isInLeaderboard == IsInLeaderboard.Yes)
            {
                _secondRankGameObject.SetActive(true);
            }
            else if (rankNumber == 3 && isInLeaderboard == IsInLeaderboard.Yes)
            {
                _thirdRankGameObject.SetActive(true);
            }
            else
            {
                _rankText.gameObject.SetActive(true);

                string rank = rankNumber.ToString();

                if (_rowType == RowType.Myself && isInLeaderboard == IsInLeaderboard.No)
                    rank = $">{rankNumber}";

                _rankText.text = rank;
            }
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RowClick()
        {
            switch (_rowType)
            {
                case RowType.Myself:
                    _row.OnClickMyselfRow();
                    break;

                case RowType.OtherUser:
                    _otherAccountPopupUI.Setup(_userData);
                    _row.OnClickOtherUserRow();
                    break;
            }
        }
        #endregion
    }
}