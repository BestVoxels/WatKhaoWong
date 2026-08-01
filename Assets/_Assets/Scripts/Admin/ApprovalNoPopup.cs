using System;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Admin
{
    public class ApprovalNoPopup : Popup
    {
        #region --Events-- (UnityEvent)
        [Header("Approval Yes Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onValidateSucceeded;
        [SerializeField] private UnityEvent _onValidateFailed;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<StayEntry, EStayStatus?> OnRejected;
        #endregion



        #region --Fields-- (In Class)
        private StayEntry _stayEntry;
        private string _keyId;
        private IUserData _userData;
        private string _notes;

        private AccommodationApproval _accommodationApproval;
        private MyUserData _myUserData;
        private SavingWrapper _savingWrapper;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _accommodationApproval = player.GetComponentInChildren<AccommodationApproval>();
            _myUserData = player.GetComponentInChildren<MyUserData>();

            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _statusText = FindAnyObjectByType<StatusText>();

            _userData = _myUserData;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            _onCancelButtonClick?.Invoke();
        }

        public void OnValidateSucceeded(StayEntry stayEntry, string keyId, IUserData userData, string notes)
        {
            _stayEntry = stayEntry;
            _keyId = keyId;
            _userData = userData;
            _notes = _accommodationApproval.PreTextForRejectedNotes + notes;

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
        public async void UploadOnServer()
        {
            if (!IsAdmin()) return;
            
            // DELETE : under StayRequests's Category
            _savingWrapper.DeleteStayRequestsEntry(_keyId);

            // -> DELETE : under ActiveStay's Category
            _savingWrapper.DeleteActiveStayEntry(_keyId);

            // -> DELETE : under User's ActiveStay
            _savingWrapper.DeleteFromUser(_userData.GetUserKeyID(), EParentNode.ActiveStay);

            // Reset Data so it UI updates accordingly
            _userData.DeleteActiveStay();
            _userData.DeleteStayEntry();


            // -> ADD : under User's PastStay
            StayEntry stayEntry = await _accommodationApproval.GetStayEntry(_stayEntry, EStayStatus.Rejected, _notes);
            await _savingWrapper.SaveDataWithKeyToUser(_userData.GetUserKeyID(), EParentNode.PastStay, stayEntry);


            OnRejected?.Invoke(stayEntry, EStayStatus.Rejected);

            _statusText.Show(_accommodationApproval.StatusRejected.GetLocalizedString(), _accommodationApproval.StatusRejectedColor);
        }
        #endregion
    }
}