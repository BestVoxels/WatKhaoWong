using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Prays;
using Bitsplash.DatePicker;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.Prays
{
    public class ChallengePopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Challenge Popup UI Stuffs")]
        [SerializeField] private DatePickerSettings _datePickerStart;
        [SerializeField] private DatePickerSettings _datePickerEnd;
        [Space]
        [SerializeField] private TMP_Text _startDateText;
        [SerializeField] private TMP_Text _endDateText;
        [SerializeField] private TMP_Text _durationText;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _duration;

        private StatusText _statusText;
        private ChallengePopup _challengePopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _challengePopup = GameObject.FindWithTag("Player").GetComponentInChildren<ChallengePopup>();
            _statusText = FindAnyObjectByType<StatusText>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);

            _datePickerStart.Content.OnSelectionChanged.AddListener(SelectedDateOnStartCalendar);
            //_datePickerStart.Content.OnDisplayChanged.AddListener(() => print("StartCalendar: Calls when Click Change Month or Year"));
            
            _datePickerEnd.Content.OnSelectionChanged.AddListener(SelectedDateOnEndCalendar);
            //_datePickerEnd.Content.OnDisplayChanged.AddListener(() => print("EndCalendar: Calls when Click Change Month or Year"));
        }

        private void Start()
        {
            _datePickerStart.Content.SetMarkerColor(DateTime.Now);
            _datePickerEnd.Content.SetMarkerColor(DateTime.Now);

            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            _startDateText.text = $"{_challengePopup.StartDateFormatBegin}{GetStartDateString()}{_challengePopup.StartDateFormatEnd}";

            _endDateText.text = $"{_challengePopup.EndDateFormatBegin}{GetEndDateString()}{_challengePopup.EndDateFormatEnd}";

            _durationText.text = $"{_challengePopup.DurationFormatBegin}{GetDurationString()}{_challengePopup.DurationFormatEnd}";
        }

        private string GetStartDateString() => (_startDate == default) ? "-" : $"<u>{_startDate:dddd, MMMM d, yyyy\nHH:mm}</u>";

        private string GetEndDateString() => (_endDate == default) ? "-" : $"<u>{_endDate:dddd, MMMM d, yyyy\nHH:mm}</u>";

        private string GetDurationString()
        {
            if (_duration == default)
                return "-";

            int totalDays = (int)Math.Round(_duration.TotalDays, MidpointRounding.AwayFromZero);

            return $"<u>{totalDays} day{S(totalDays)}</u>";
        }

        private string S(int input) => input > 1 ? "s" : "";
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _challengePopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Cancel()
        {
            _challengePopup.OnCancelButtonClick();
        }

        private void Confirm()
        {
            if (_challengePopup.ValidateChallengePopup(_startDate, _endDate))
            {
                _challengePopup.CreateChallenge(_startDate, _endDate, _duration);

                _challengePopup.OnConfirmButtonClick();

                _statusText.Show(_challengePopup.StatusCreateSucceeded, _challengePopup.StatusCreateSucceededColor);
            }
            else
            {
                _challengePopup.OnConfirmButtonCantClick();

                _statusText.Show(_challengePopup.StatusCreateFailed, _challengePopup.StatusCreateFailedColor);
            }
        }

        private void SelectedDateOnStartCalendar()
        {
            _startDate = _datePickerStart.Content.Selection.GetItem(0); // To get multiple Date, check 'SelectionTutorial.cs' line 60
            _duration = _challengePopup.GetChallengeDuration(_startDate, _endDate);
            _challengePopup.ValidateStartDate(_startDate, _endDate);

            RefreshUI();
        }

        private void SelectedDateOnEndCalendar()
        {
            _endDate = _datePickerEnd.Content.Selection.GetItem(0); // To get multiple Date, check 'SelectionTutorial.cs' line 60
            _duration = _challengePopup.GetChallengeDuration(_startDate, _endDate);
            _challengePopup.ValidateEndDate(_startDate, _endDate);

            RefreshUI();
        }
        #endregion
    }
}