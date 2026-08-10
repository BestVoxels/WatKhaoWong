using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Admin;
using WatKhaoWong.Retreats;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.UI.Admin
{
    public class ApprovalNoPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Approval No Popup UI Stuffs")]
        [SerializeField] private TMP_InputField _notesInputField;
        [SerializeField] private InputFieldStatus _notesInputFieldStatus;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private StayEntry _stayEntry;
        private string _keyId;
        private IUserData _userData;
        private string _notes;

        private UserInfo _userInfo;
        private ApprovalNoPopup _approvalNoPopup;
        private InputFieldValidator _inputFieldValidator;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _userInfo = player.GetComponentInChildren<UserInfo>();
            _approvalNoPopup = player.GetComponentInChildren<ApprovalNoPopup>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool Validate()
        {
            bool status = true;

            if (!IsNotesValidated()) status = false;
            return status;
        }

        private bool IsNotesValidated() => _inputFieldValidator.ValidateNotNull(
            _notesInputField.text, _notesInputFieldStatus, out _notes,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public async void Setup(StayEntry stayEntry, string keyId, IUserData userData)
        {
            if (!await MyUserData.IsAdmin()) return;

            _stayEntry = stayEntry;
            _keyId = keyId;
            _userData = userData;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _approvalNoPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Cancel() => _approvalNoPopup.OnCancelButtonClick();

        private void Confirm()
        {
            if (Validate())
                _approvalNoPopup.OnValidateSucceeded(_stayEntry, _keyId, _userData, _notes);
            else
                _approvalNoPopup.OnValidateFailed();
        }
        #endregion
    }
}