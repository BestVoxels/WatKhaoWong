using System;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Utils.Conditions;

namespace WatKhaoWong.Challenges
{
    public class Challenge : MonoBehaviour, IConditionEvaluator
    {
        #region --Fields-- (Inspector)
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Challenge Event")]
        [SerializeField] private UnityEvent _onChallengeCreationButtonClick;
        [SerializeField] private UnityEvent _onChallengePendingButtonClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public static event Action OnStatusChanged;
        #endregion



        #region --Fields-- (In Class)
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _duration;

        private static EChallengeStatus _status;
        #endregion



        #region --Properties-- (With Backing Fields)
        public DateTime StartDate
        {
            get
            {
                // TODO has to load from server.

                return _startDate;
            }

            private set
            {
                _startDate = value;

                // TODO upload data to server.
            }
        }

        public DateTime EndDate
        {
            get
            {
                // TODO has to load from server.

                return _endDate;
            }

            private set
            {
                _endDate = value;

                // TODO upload data to server.
            }
        }

        public TimeSpan Duration
        {
            get
            {
                // TODO has to load from server.

                return _duration;
            }

            private set
            {
                _duration = value;

                // TODO upload data to server.
            }
        }


        public static EChallengeStatus Status
        {
            get => _status;

            private set
            {
                _status = value;

                OnStatusChanged?.Invoke();
            }
        }
        #endregion



        #region --Methods-- (Built In)
        // ------
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                Status = EChallengeStatus.None;

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Status = EChallengeStatus.Pending;

            if (Input.GetKeyDown(KeyCode.Alpha3))
                Status = EChallengeStatus.Live;
        }
        // ------
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void CreatePendingChallenge(DateTime startDate, DateTime endDate, TimeSpan duration)
        {
            if (Status == EChallengeStatus.Pending) return;

            StartDate = startDate;
            EndDate = endDate;
            Duration = duration;

            Status = EChallengeStatus.Pending; // Put this line last after all info got assigned so UI can update properly.
        }

        public void DeletePendingChallenge()
        {
            if (Status != EChallengeStatus.Pending) return;

            StartDate = default;
            EndDate = default;
            Duration = default;

            Status = EChallengeStatus.None; // Put this line last after all info got assigned so UI can update properly.
        }

        public void LiveChallenge()
        {
            if (Status == EChallengeStatus.Live) return;

            // TODO live ...coding...

            Status = EChallengeStatus.Live; // Put this line last after all info got assigned so UI can update properly.
        }

        public int GetChallengeEndDaysLeft()
        {
            if (Status == EChallengeStatus.None || DateTime.Today < StartDate.Date) return -1; // Challenge is not yet started

            TimeSpan daysLeft = EndDate.Date - DateTime.Today;

            return (int)Math.Round(daysLeft.TotalDays, MidpointRounding.AwayFromZero);
        }

        public int GetChallengeStartDaysLeft()
        {
            if (Status == EChallengeStatus.Live || DateTime.Today >= StartDate.Date) return -1; // Challenge is already started

            TimeSpan daysLeft = StartDate.Date - DateTime.Today;

            return (int)Math.Round(daysLeft.TotalDays, MidpointRounding.AwayFromZero);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Text Formatter~
        public string FormatDateString(DateTime date, string format) => (date == default) ? "-" : $"<u>{date.ToString(format)}</u>";

        public string FormatDurationString(TimeSpan duration)
        {
            if (duration == default)
                return "-";

            int totalDays = (int)Math.Round(duration.TotalDays, MidpointRounding.AwayFromZero);

            return $"<u>{totalDays} day{S(totalDays)}</u>";
        }

        public string S(int input) => input > 1 ? "s" : "";
        #endregion



        #region --Methods-- (Custom PUBLIC) ~UI Buttons~
        public void OnChallengeCreationButtonClick()
        {
            _onChallengeCreationButtonClick?.Invoke();
        }

        public void OnChallengePendingButtonClick()
        {
            _onChallengePendingButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Interface)
        bool? IConditionEvaluator.Evaluate(EConditionType conditionType, EConditionValue[] conditionValues)
        {
            switch (conditionType)
            {
                case EConditionType.IsChallengeStatusEquals:
                    byte stringStartIndex = (byte)EConditionType.IsChallengeStatusEquals;
                    string enumString = conditionValues[0].ToString()[stringStartIndex..];

                    if (!Enum.TryParse(enumString, true, out EChallengeStatus result))
                        return false;

                    return Status == result;
            }

            return null;
        }
        #endregion
    }
}