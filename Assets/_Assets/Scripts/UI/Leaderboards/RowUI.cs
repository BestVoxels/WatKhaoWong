using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using WatKhaoWong.Identity;
using WatKhaoWong.Leaderboards;

namespace WatKhaoWong.UI.Leaderboards
{
    public class RowUI : MonoBehaviour
    {
        private enum RowType
        {
            Myself,
            OtherUser
        }



        #region --Fields-- (Inspector)
        [Header("Row UI Stuffs")]
        [SerializeField] private RowType _rowType = RowType.OtherUser;
        [Space]
        [SerializeField] private EventTrigger _rowEventTrigger;

        [Space]

        [Header("Rank")]
        [SerializeField] private GameObject _firstRankGameObject;
        [SerializeField] private GameObject _secondRankGameObject;
        [SerializeField] private GameObject _thirdRankGameObject;
        [SerializeField] private TMP_Text _rankText;

        [Header("Profile Icon")]
        [SerializeField] private AccountData.IconUI _icon;

        [Header("Profile Name")]
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _levelText;

        [Header("Stats")]
        [SerializeField] private TMP_Text _scoreText;
        #endregion



        #region --Fields-- (In Class)
        private Row _row;
        #endregion



        #region --Fields-- (Constant)
        private const float MultiplierRatioForDecorator = 165f / 135f;  // Formula : Main Profile's Size (BIG) % Inventory Profile's Size (SMALL)
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _row = GameObject.FindWithTag("Player").GetComponentInChildren<Row>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => RowClick((PointerEventData)data));
            _rowEventTrigger.triggers.Add(entry);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(string name, string score) // AccountData account, ushort rankNumber
        {
            // TODO temp Setup()
            _userNameText.text = name;
            _scoreText.text = score;

            //UpdateRankUI(rankNumber);

            //RefreshUI(account);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UpdateRankUI(ushort rankNumber)
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
            if (rankNumber == 1)
            {
                _firstRankGameObject.SetActive(true);
            }
            else if (rankNumber == 2)
            {
                _secondRankGameObject.SetActive(true);
            }
            else if (rankNumber == 3)
            {
                _thirdRankGameObject.SetActive(true);
            }
            else
            {
                _rankText.gameObject.SetActive(true);
                _rankText.text = rankNumber.ToString();
            }
        }

        private void RefreshUI(AccountData account)
        {
            if (account == default)
            {
                Debug.LogError("CUSTOM Error : RowUI.cs is created BUT havn't Setup() yet! Must call Setup() method first!");
                return;
            }

            account.UpdateProfileIcon(_icon, account.GetProfileIcon(), MultiplierRatioForDecorator);

            _userNameText.text = account.GetUserNameText();
            _levelText.text = account.GetLevelText();

            _scoreText.text = account.GetTotalTMPointsText();
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void RowClick(PointerEventData data)
        {
            switch (_rowType)
            {
                case RowType.Myself:
                    _row.OnClickMyselfRow();
                    break;

                case RowType.OtherUser:
                    _row.OnClickOtherUserRow();
                    break;
            }
        }
        #endregion
    }
}