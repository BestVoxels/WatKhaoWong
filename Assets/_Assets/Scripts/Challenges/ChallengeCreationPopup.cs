using System;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Challenges
{
    public class ChallengeCreationPopup : Popup
    {
        public enum ShowStatus
        {
            Show,
            Hide
        }



        #region --Fields-- (Inspector)
        [field: Header("Challenge Creation Popup - Status Text")]
        [field: SerializeField] public string StatusCreateFailed { get; private set; } = "Creation failed.";
        [field: SerializeField] public Color32 StatusCreateFailedColor { get; private set; }
        [Space]
        [SerializeField] private string _statusCreateSucceeded = "The challenge has been created successfully!";
        [SerializeField] private Color32 _statusCreateSucceededColor;
        [Space]
        [Space]
        [Space]
        [SerializeField] private string _statusStartDateIsNull = "Please select a Start Date on the first calendar.";
        [SerializeField] private Color32 _statusStartDateIsNullColor;
        [Space]
        [SerializeField] private string _statusStartDateIsInPast = "The Start Date cannot be in the past. Please select today’s date or a date in the future.";
        [SerializeField] private Color32 _statusStartDateIsInPastColor;
        [Space]
        [SerializeField] private string _statusStartDateIsAfterEndDate = "The Start Date cannot be after the End Date. Please select a date earlier than the End Date.";
        [SerializeField] private Color32 _statusStartDateIsAfterEndDateColor;
        [Space]
        [Space]
        [Space]
        [SerializeField] private string _statusEndDateIsNull = "Please select an End Date on the second calendar.";
        [SerializeField] private Color32 _statusEndDateIsNullColor;
        [Space]
        [SerializeField] private string _statusEndDateIsInPast = "The End Date cannot be in the past. Please select a date in the future.";
        [SerializeField] private Color32 _statusEndDateIsInPastColor;
        [Space]
        [SerializeField] private string _statusEndDateIsBeforeStartDate = "The End Date cannot be before the Start Date. Please select a date after the Start Date.";
        [SerializeField] private Color32 _statusEndDateIsBeforeStartDateColor;
        [Space]
        [Space]
        [Space]
        [SerializeField] private string _statusBothDateIsSameDate = "The Start Date and End Date cannot be the same date!";
        [SerializeField] private Color32 _statusBothDateIsSameDateColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Challenge Creation Popup - Settings")]
        [field: SerializeField] public string DateStringFormat { get; private set; } = "dddd, MMMM d, yyyy\nHH:mm";
        [field: Space]
        [field: SerializeField] public string StartDateFormatBegin { get; private set; } = "Start Date: ";
        [field: SerializeField] public string StartDateFormatEnd { get; private set; } = "";
        [field: Space]
        [field: SerializeField] public string EndDateFormatBegin { get; private set; } = "End Date: ";
        [field: SerializeField] public string EndDateFormatEnd { get; private set; } = "";
        [field: Space]
        [field: SerializeField] public string DurationFormatBegin { get; private set; } = "Challenge Duration: ";
        [field: SerializeField] public string DurationFormatEnd { get; private set; } = "";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Challenge Creation Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonCantClick;
        #endregion



        #region --Fields-- (In Class)
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _duration;

        private StatusText _statusText;
        private Challenge _challenge;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _challenge = GameObject.FindWithTag("Player").GetComponentInChildren<Challenge>();
            _statusText = FindAnyObjectByType<StatusText>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public TimeSpan GetChallengeDuration(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default) return default;
            if (!ValidateChallengePopup(startDate, endDate, ShowStatus.Hide)) return default;

            TimeSpan duration = endDate - startDate;

            return duration;
        }

        public bool ValidateChallengePopup(DateTime startDate, DateTime endDate, ShowStatus displayStatus = ShowStatus.Show)
        {
            if (!ValidateStartDate(startDate, endDate, displayStatus)) return false;
            if (!ValidateEndDate(startDate, endDate, displayStatus)) return false;

            return true;
        }

        public bool ValidateStartDate(DateTime startDate, DateTime endDate, ShowStatus displayStatus = ShowStatus.Show)
        {
            if (startDate == default)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusStartDateIsNull, _statusStartDateIsNullColor);
                return false;
            }
            if (startDate.Date < DateTime.Today)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusStartDateIsInPast, _statusStartDateIsInPastColor);
                return false;
            }
            if (endDate != default && startDate.Date > endDate.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusStartDateIsAfterEndDate, _statusStartDateIsAfterEndDateColor);
                return false;
            }
            if (startDate.Date == endDate.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusBothDateIsSameDate, _statusBothDateIsSameDateColor);
                return false;
            }    

            return true;
        }

        public bool ValidateEndDate(DateTime startDate, DateTime endDate, ShowStatus displayStatus = ShowStatus.Show)
        {
            if (endDate == default)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusEndDateIsNull, _statusEndDateIsNullColor);
                return false;
            }
            if (endDate.Date < DateTime.Today)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusEndDateIsInPast, _statusEndDateIsInPastColor);
                return false;
            }
            if (startDate != default && endDate.Date < startDate.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusEndDateIsBeforeStartDate, _statusEndDateIsBeforeStartDateColor);
                return false;
            }
            if (startDate.Date == endDate.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusBothDateIsSameDate, _statusBothDateIsSameDateColor);
                return false;
            }

            return true;
        }

        public void SetDataAwaitConfirmation(DateTime startDate, DateTime endDate, TimeSpan duration)
        {
            _startDate = startDate;
            _endDate = endDate;
            _duration = duration;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            _onCancelButtonClick?.Invoke();
        }

        public void OnConfirmButtonClick()
        {
            _onConfirmButtonClick?.Invoke();
        }

        public void OnConfirmButtonCantClick()
        {
            _onConfirmButtonCantClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void CreateChallenge()
        {
            _statusText.Show(_statusCreateSucceeded, _statusCreateSucceededColor);

            _challenge.CreatePendingChallenge(_startDate, _endDate, _duration);
        }
        #endregion
    }
}