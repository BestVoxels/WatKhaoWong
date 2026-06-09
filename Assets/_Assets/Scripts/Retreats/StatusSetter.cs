using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Retreats
{
    public class StatusSetter : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Status Text")]
        [SerializeField] private LocalizedString _statusSucceeded;
        [SerializeField] private Color32 _statusSucceededColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Format on Button")]
        [field: SerializeField] public LocalizedString DateEndsOnText { get; private set; }
        [field: SerializeField] public string DayFormat { get; private set; } = "d/M/yyyy";

        [field: Header("Settings")]
        [field: SerializeField] public string BanTemporayNameOnDropdown { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Status Setter Event")]
        [SerializeField] private UnityEvent _onSetTimeButtonClick;
        [SerializeField] private UnityEvent _onValidateSucceeded;
        [SerializeField] private UnityEvent _onValidateFailed;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnUploadedToServer;
        #endregion



        #region --Fields-- (In Class)
        private byte _accountStatusIndex;
        private DateTime _dateEndsOn;
        private string _notes;

        private MyUserData _myUserData;
        private IUserData _userData;
        private Localizer _localizer;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _localizer = FindAnyObjectByType<Localizer>();
            _statusText = FindAnyObjectByType<StatusText>();

            _userData = _myUserData;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnSetTimeButtonClick()
        {
            _onSetTimeButtonClick?.Invoke();
        }

        public void OnValidateSucceeded(IUserData userData, byte accountStatusIndex, DateTime dateEndsOn, string notes)
        {
            _userData = userData;

            _accountStatusIndex = accountStatusIndex;
            _dateEndsOn = dateEndsOn;
            _notes = notes;

            _onValidateSucceeded?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public async void UploadToServer()
        {
            EAccountStatus accountStatus = (EAccountStatus)_accountStatusIndex;
            DateTime dateTime = accountStatus == EAccountStatus.BanTemporary ? _dateEndsOn : default;
            string notes = _notes;
            string notesColor = string.IsNullOrWhiteSpace(_notes) ? "" : "#" + ColorUtility.ToHtmlStringRGB(_localizer.ColorizeAccountStatus(accountStatus.ToString()));

            await _userData.SetDataAccountStatus(
                updateCheckinAt: false,
                accountStatus,
                dateTime,
                notes,
                notesColor
                );

            //// Code that makes it only updates the relevant one, but problem is we can't delete the one that is already written
            //DateTime? dateTime = (accountStatus == EAccountStatus.BanTemporary && _dateEndsOn != default) ? _dateEndsOn : null;
            //string notes = string.IsNullOrWhiteSpace(_notes) ? null : _notes;
            //string notesColor = notes == null ? null : "#" + ColorUtility.ToHtmlStringRGB(_localizer.ColorizeAccountStatus(accountStatus.ToString()));

            OnUploadedToServer?.Invoke();

            _statusText.Show(_statusSucceeded.GetLocalizedString(), _statusSucceededColor);
        }
        #endregion
    }
}