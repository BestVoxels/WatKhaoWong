using System;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Retreats
{
    public class AccommodationSetTimePopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Accommodation Set Time - Settings")]
        [Range(1, 30)]
        [SerializeField] private byte _maxDaysAllowed = 7;

        [Header("Accommodation Set Time - Status Text")]
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
        [Space]
        [SerializeField] private LocalizedString _statusExceedsMaxDays;
        [SerializeField] private Color32 _statusExceedsMaxDaysColor;

        [Header("Accommodation Set Time - For Displaying Text")]
        [SerializeField] private LocalizedString _dayText;
        [SerializeField] private LocalizedString _sText;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Accommodation Set Time - Settings")]
        [field: SerializeField] public string DateStringFormat { get; private set; } = "dddd, MMMM d, yyyy\nHH:mm";
        [field: Space]
        [field: SerializeField] public LocalizedString StartDateText { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString EndDateText { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString DurationText { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Accommodation Set Time UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonCantClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<DateTime, DateTime> OnValidated;
        #endregion



        #region --Fields-- (In Class)
        private bool _allowPastDate = false;
        private EIsStaying _isStayingOvernight;
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _duration;
        private DateTime _serverTimeNow;

        private StatusText _statusText;
        private ServerTime _serverTime;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
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
        public TimeSpan GetDuration(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default) return default;

            TimeSpan duration = endDate - startDate;

            // Don't return Negative TimeSpan
            if (duration.TotalDays < 0) return default;

            return duration;
        }

        public void SetAllowPastDate(bool allowPastDate)
        {
            _allowPastDate = allowPastDate;
        }

        public bool ValidateSetTimePopup(bool allowPastDate = false)
        {
            _allowPastDate = allowPastDate;

            if (!ValidateStartDate(_startDate, _endDate)) return false;
            if (!ValidateEndDate(_startDate, _endDate)) return false;

            OnValidated?.Invoke(_startDate, _endDate);
            return true;
        }

        public bool ValidateSetTimePopup(DateTime startDate, DateTime endDate)
        {
            if (!ValidateStartDate(startDate, endDate)) return false;
            if (!ValidateEndDate(startDate, endDate)) return false;

            OnValidated?.Invoke(startDate, endDate);
            return true;
        }

        public bool ValidateStartDate(DateTime startDate, DateTime endDate)
        {
            if (startDate == default)
            {
                _statusText.Show(_statusStartDateIsNull.GetLocalizedString(), _statusStartDateIsNullColor);
                return false;
            }
            if (!_allowPastDate && startDate.Date < _serverTimeNow.Date)
            {
                _statusText.Show(_statusStartDateIsInPast.GetLocalizedString(), _statusStartDateIsInPastColor);
                return false;
            }

            if (_isStayingOvernight == EIsStaying.NotStaying) return true;
            // 3 IF clause below don't need to runs if 'NotStaying'

            if (endDate != default && startDate.Date > endDate.Date)
            {
                _statusText.Show(_statusStartDateIsAfterEndDate.GetLocalizedString(), _statusStartDateIsAfterEndDateColor);
                return false;
            }
            if (startDate.Date == endDate.Date)
            {
                _statusText.Show(_statusBothDateIsSameDate.GetLocalizedString(), _statusBothDateIsSameDateColor);
                return false;
            }
            if (GetDuration(startDate, endDate).TotalDays > _maxDaysAllowed)
            {
                _statusText.Show(_statusExceedsMaxDays.GetLocalizedString(DaysString(_maxDaysAllowed)), _statusExceedsMaxDaysColor);
                return false;
            }

            return true;
        }

        public bool ValidateEndDate(DateTime startDate, DateTime endDate)
        {
            if (_isStayingOvernight == EIsStaying.NotStaying) return true;
            // 5 IF clause below don't need to runs if 'NotStaying'
            
            if (endDate == default)
            {
                _statusText.Show(_statusEndDateIsNull.GetLocalizedString(), _statusEndDateIsNullColor);
                return false;
            }
            if (!_allowPastDate && endDate.Date < _serverTimeNow.Date)
            {
                _statusText.Show(_statusEndDateIsInPast.GetLocalizedString(), _statusEndDateIsInPastColor);
                return false;
            }
            if (startDate != default && endDate.Date < startDate.Date)
            {
                _statusText.Show(_statusEndDateIsBeforeStartDate.GetLocalizedString(), _statusEndDateIsBeforeStartDateColor);
                return false;
            }
            if (startDate.Date == endDate.Date)
            {
                _statusText.Show(_statusBothDateIsSameDate.GetLocalizedString(), _statusBothDateIsSameDateColor);
                return false;
            }
            if (GetDuration(startDate, endDate).TotalDays > _maxDaysAllowed)
            {
                _statusText.Show(_statusExceedsMaxDays.GetLocalizedString(DaysString(_maxDaysAllowed)), _statusExceedsMaxDaysColor);
                return false;
            }

            return true;
        }

        public EIsStaying GetIsStayingOvernight() => _isStayingOvernight;

        public void SetIsStayingOvernight(EIsStaying isStayingOvernight)
        {
            _isStayingOvernight = isStayingOvernight;
        }

        public void SetDataReadyForUse(EIsStaying isStayingOvernight, DateTime startDate, DateTime endDate, TimeSpan duration)
        {
            _isStayingOvernight = isStayingOvernight;
            _startDate = startDate;
            _endDate = endDate;
            _duration = duration;
        }

        public SetTimeData GetData()
        {
            SetTimeData data = new SetTimeData()
            {
                isStayingOvernight = _isStayingOvernight,
                startDate = _startDate,
                endDate = _endDate,
                duration = _duration
            };

            return data;
        }

        public void ClearOnValidatedSubscribers()
        {
            OnValidated = null;
        }

        public void ClearDateData()
        {
            // No Need to reset '_isStayingOvernight = default' just make it same like what is showing on UI
            _startDate = default;
            _endDate = default;
            _duration = default;

            _allowPastDate = false;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Displaying~
        public string FormatButtonString(DateTime startDate, DateTime endDate, string format)
        {
            int totalDays = 1;

            if (endDate != default)
            {
                TimeSpan duration = endDate - startDate;

                totalDays = (int)Math.Round(duration.TotalDays, MidpointRounding.AwayFromZero);

                return $"{DaysString(totalDays)}\n({startDate.ToGregorianString(format)} - {endDate.ToGregorianString(format)})";
            }

            return $"{DaysString(totalDays)}\n({startDate.ToGregorianString(format)})";
        }

        public string FormatDateString(DateTime date, string format) => (date == default) ? "-" : $"<u>{date.ToGregorianString(format)}</u>";

        public string FormatDurationString(TimeSpan duration)
        {
            if (duration == default)
                return "-";

            int totalDays = (int)Math.Round(duration.TotalDays, MidpointRounding.AwayFromZero);

            return $"<u>{totalDays} {_dayText.GetLocalizedString()}{S(totalDays)}</u>";
        }

        public string DaysString(int days)
        {
            if (days < 0)
                return $"??? {_dayText.GetLocalizedString()}";

            return $"{days} {_dayText.GetLocalizedString()}{S(days)}";
        }

        public string S(int input) => input > 1 ? _sText.GetLocalizedString() : "";
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



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake(). And at time of assiging to 'FirebaseAuth.DefaultInstance.StateChanged'
        /// </summary>
        private async void HandleStateChanged(object obj, EventArgs args)
        {
            _serverTimeNow = await _serverTime.Now();
        }
        #endregion
    }

    
}