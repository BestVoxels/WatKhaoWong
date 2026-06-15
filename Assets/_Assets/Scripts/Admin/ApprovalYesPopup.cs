using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;

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



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            _onCancelButtonClick?.Invoke();
        }

        public void OnValidateSucceeded()
        {
            _onValidateSucceeded?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void UploadToServer()
        {

        }
        #endregion
    }
}