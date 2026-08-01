using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Admin
{
    public class ApprovalYesPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Approval Yes Popup UI Settings")]
        [field: SerializeField] public LocalizedString OfferText { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Approval Yes Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onValidateSucceeded;
        [SerializeField] private UnityEvent _onValidateFailed;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<StayEntry, EStayStatus?> OnAccepted;
        #endregion



        #region --Fields-- (In Class)
        private StayEntry _stayEntry;
        private string _keyId;
        private IUserData _userData;
        private byte _buildingIndex;
        private string _roomNumber;

        private AccommodationApproval _accommodationApproval;
        private MyUserData _myUserData;
        private SavingWrapper _savingWrapper;
        private ServerTime _serverTime;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _accommodationApproval = player.GetComponentInChildren<AccommodationApproval>();
            _myUserData = player.GetComponentInChildren<MyUserData>();

            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _serverTime = FindAnyObjectByType<ServerTime>();
            _statusText = FindAnyObjectByType<StatusText>();

            _userData = _myUserData;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            _onCancelButtonClick?.Invoke();
        }

        public void OnValidateSucceeded(StayEntry stayEntry, string keyId, IUserData userData, byte buildingIndex, string roomNumber)
        {
            _stayEntry = stayEntry;
            _keyId = keyId;
            _userData = userData;
            _buildingIndex = buildingIndex;
            _roomNumber = roomNumber;

            _onValidateSucceeded?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool IsAdmin() => _myUserData.GetRole() == EUserRole.Admin;
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public async void UpdateOnServer()
        {
            if (!IsAdmin()) return;

            DateTime nowDate = await _serverTime.Now();
            StayEntry stayEntry = null;
            ActiveStay activeStay = null;

            // DELETE : under StayRequests's Category
            _savingWrapper.DeleteStayRequestsEntry(_keyId);
            
            // Check for Time Period
            ETimePeriod? timePeriod = DateExtension.GetTimePeriod(_stayEntry.StayInfo.StartDate, nowDate);
            switch (timePeriod)
            {
                // --- Past & Active ---
                case ETimePeriod.Past:
                case ETimePeriod.Present:
                    stayEntry = await _accommodationApproval.GetStayEntry(_stayEntry, EStayStatus.Active, _buildingIndex, _roomNumber);
                    
                    // -> ADD : under ActiveStay's Category
                    string keyId = await _savingWrapper.SaveDataWithKey(ECategoryNode.ActiveStay, stayEntry);

                    // -> ADD : under User's ActiveStay
                    activeStay = await _accommodationApproval.GetActiveStay(keyId, EStayStatus.Active);
                    await _userData.SetDataActiveStay(activeStay);

                    OnAccepted?.Invoke(stayEntry, EStayStatus.Active);
                    break;

                // --- Scheduled ---
                case ETimePeriod.Future:
                    stayEntry = await _accommodationApproval.GetStayEntry(_stayEntry, EStayStatus.Scheduled, _buildingIndex, _roomNumber);
                    
                    // -> ADD : under ScheduledStay's Category
                    keyId = await _savingWrapper.SaveDataWithKey(ECategoryNode.ScheduledStay, stayEntry);

                    // -> ADD : under User's ActiveStay
                    activeStay = await _accommodationApproval.GetActiveStay(keyId, EStayStatus.Scheduled);
                    await _userData.SetDataActiveStay(activeStay);

                    OnAccepted?.Invoke(stayEntry, EStayStatus.Scheduled);
                    break;
            }

            _statusText.Show(_accommodationApproval.StatusAccepted.GetLocalizedString(), _accommodationApproval.StatusAcceptedColor);
        }
        #endregion
    }
}