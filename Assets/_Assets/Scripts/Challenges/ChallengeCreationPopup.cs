using System;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
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
        [field: SerializeField] public LocalizedString StatusCreateFailed { get; private set; }
        [field: SerializeField] public Color32 StatusCreateFailedColor { get; private set; }
        [Space]
        [SerializeField] private LocalizedString _statusCreateSucceeded;
        [SerializeField] private Color32 _statusCreateSucceededColor;
        [Space]
        [Space]
        [Space]
        [SerializeField] private LocalizedString _statusStartDateIsNull;
        [SerializeField] private Color32 _statusStartDateIsNullColor;
        [Space]
        [SerializeField] private LocalizedString _statusStartDateIsInPast;
        [SerializeField] private Color32 _statusStartDateIsInPastColor;
        [Space]
        [SerializeField] private LocalizedString _statusStartDateIsAfterEndDate;
        [SerializeField] private Color32 _statusStartDateIsAfterEndDateColor;
        [Space]
        [Space]
        [Space]
        [SerializeField] private LocalizedString _statusEndDateIsNull;
        [SerializeField] private Color32 _statusEndDateIsNullColor;
        [Space]
        [SerializeField] private LocalizedString _statusEndDateIsInPast;
        [SerializeField] private Color32 _statusEndDateIsInPastColor;
        [Space]
        [SerializeField] private LocalizedString _statusEndDateIsBeforeStartDate;
        [SerializeField] private Color32 _statusEndDateIsBeforeStartDateColor;
        [Space]
        [Space]
        [Space]
        [SerializeField] private LocalizedString _statusBothDateIsSameDate;
        [SerializeField] private Color32 _statusBothDateIsSameDateColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Challenge Creation Popup - Settings")]
        [field: SerializeField] public string DateStringFormat { get; private set; } = "dddd, MMMM d, yyyy\nHH:mm";
        [field: Space]
        [field: SerializeField] public LocalizedString StartDateFormat { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString EndDateFormat { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString DurationFormat { get; private set; }
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
        private DateTime _serverTimeNow;

        private StatusText _statusText;
        private Challenge _challenge;
        private ServerTime _serverTime;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _challenge = GameObject.FindWithTag("Player").GetComponentInChildren<Challenge>();
            _statusText = FindAnyObjectByType<StatusText>();
            _serverTime = FindAnyObjectByType<ServerTime>();
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
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
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusStartDateIsNull.GetLocalizedString(), _statusStartDateIsNullColor);
                return false;
            }
            if (startDate.Date < _serverTimeNow.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusStartDateIsInPast.GetLocalizedString(), _statusStartDateIsInPastColor);
                return false;
            }
            if (endDate != default && startDate.Date > endDate.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusStartDateIsAfterEndDate.GetLocalizedString(), _statusStartDateIsAfterEndDateColor);
                return false;
            }
            if (startDate.Date == endDate.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusBothDateIsSameDate.GetLocalizedString(), _statusBothDateIsSameDateColor);
                return false;
            }

            return true;
        }

        public bool ValidateEndDate(DateTime startDate, DateTime endDate, ShowStatus displayStatus = ShowStatus.Show)
        {
            if (endDate == default)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusEndDateIsNull.GetLocalizedString(), _statusEndDateIsNullColor);
                return false;
            }
            if (endDate.Date < _serverTimeNow.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusEndDateIsInPast.GetLocalizedString(), _statusEndDateIsInPastColor);
                return false;
            }
            if (startDate != default && endDate.Date < startDate.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusEndDateIsBeforeStartDate.GetLocalizedString(), _statusEndDateIsBeforeStartDateColor);
                return false;
            }
            if (startDate.Date == endDate.Date)
            {
                if (displayStatus == ShowStatus.Show) _statusText.Show(_statusBothDateIsSameDate.GetLocalizedString(), _statusBothDateIsSameDateColor);
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
            _statusText.Show(_statusCreateSucceeded.GetLocalizedString(), _statusCreateSucceededColor);

            _challenge.CreatePendingChallenge(_startDate, _endDate, _duration);
        }
        #endregion



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        /// </summary>
        private async void HandleStateChanged(object obj, EventArgs args)
        {
            _serverTimeNow = await _serverTime.Now();
        }
        #endregion
    }
}