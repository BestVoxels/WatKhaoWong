using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Challenges;

namespace WatKhaoWong.UI.Challenges
{
    public class ChallengePendingPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Challenge Pending Popup UI Stuffs")]
        [SerializeField] private TMP_Text _startDateText;
        [SerializeField] private TMP_Text _endDateText;
        [SerializeField] private TMP_Text _durationText;
        [Space]
        [SerializeField] private Button _deleteButton;
        #endregion



        #region --Fields-- (In Class)
        private ChallengePendingPopup _challengePending;
        private Challenge _challenge;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _challengePending = GameObject.FindWithTag("Player").GetComponentInChildren<ChallengePendingPopup>();
            _challenge = GameObject.FindWithTag("Player").GetComponentInChildren<Challenge>();

            _closeButton.onClick.AddListener(Close);

            _deleteButton.onClick.AddListener(Delete);

            UIRefresher.OnPopupRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            _startDateText.text = $"{_challengePending.StartDateFormatBegin}{_challenge.FormatDateString(_challenge.StartDate, _challengePending.DateStringFormat)}{_challengePending.StartDateFormatEnd}";

            _endDateText.text = $"{_challengePending.EndDateFormatBegin}{_challenge.FormatDateString(_challenge.EndDate, _challengePending.DateStringFormat)}{_challengePending.EndDateFormatEnd}";

            _durationText.text = $"{_challengePending.DurationFormatBegin}{_challenge.FormatDurationString(_challenge.Duration)}{_challengePending.DurationFormatEnd}";
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _challengePending.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Delete()
        {
            _challengePending.OnDeleteButtonClick();
        }
        #endregion
    }
}