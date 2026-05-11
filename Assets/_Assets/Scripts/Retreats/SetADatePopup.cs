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
    public class SetADatePopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Set A Date - Status Text")]
        [SerializeField] private LocalizedString _statusDateIsNull;
        [SerializeField] private Color32 _statusDateIsNullColor;
        [Space]
        [SerializeField] private LocalizedString _statusDateMustBeInFuture;
        [SerializeField] private Color32 _statusDateMustBeInFutureColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Set A Date - Settings")]
        [field: SerializeField] public string DateStringFormat { get; private set; } = "dddd, MMMM d, yyyy\nHH:mm";
        [field: Space]
        [field: SerializeField] public LocalizedString DateText { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Set A Date UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonCantClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<DateTime> OnValidated;
        #endregion



        #region --Fields-- (In Class)
        private DateTime _date;
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
        public bool ValidateSetADatePopup()
        {
            if (!ValidateDate(_date)) return false;

            OnValidated?.Invoke(_date);
            return true;
        }

        public bool ValidateSetADatePopup(DateTime date)
        {
            if (!ValidateDate(date)) return false;

            OnValidated?.Invoke(date);
            return true;
        }

        public bool ValidateDate(DateTime date)
        {
            if (date == default)
            {
                _statusText.Show(_statusDateIsNull.GetLocalizedString(), _statusDateIsNullColor);
                return false;
            }
            if (date.Date <= _serverTimeNow.Date)
            {
                _statusText.Show(_statusDateMustBeInFuture.GetLocalizedString(), _statusDateMustBeInFutureColor);
                return false;
            }

            return true;
        }

        public void SetDataReadyForUse(DateTime date)
        {
            _date = date;
        }

        public DateTime GetData()
        {
            return _date;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Displaying~
        public string FormatInfoString(DateTime date, string format) => (date == default) ? "???" : $"({date.ToGregorianString(format)})";

        public string FormatDateString(DateTime date, string format) => (date == default) ? "-" : $"<u>{date.ToGregorianString(format)}</u>";
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
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        /// </summary>
        private async void HandleStateChanged(object obj, EventArgs args)
        {
            _serverTimeNow = await _serverTime.Now();
        }
        #endregion
    }

    
}