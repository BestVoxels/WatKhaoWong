using TMPro;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Admin;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Retreats;

namespace WatKhaoWong.UI.Admin
{
    public class ApprovalYesPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Approval Yes Popup UI Stuffs")]
        [SerializeField] private TMP_Text _offerText;
        [SerializeField] private CustomDropdown _buildingDropdown;
        [SerializeField] private TMP_InputField _roomNumberInputField;
        [SerializeField] private InputFieldStatus _roomNumberInputFieldStatus;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private byte _buildingIndex;
        private string _roomNumber;

        private UserInfo _userInfo;
        private InputFieldValidator _inputFieldValidator;
        private ApprovalYesPopup _approvalYesPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _userInfo = player.GetComponentInChildren<UserInfo>();
            _approvalYesPopup = player.GetComponentInChildren<ApprovalYesPopup>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);

            _buildingDropdown.onValueChanged.AddListener(BuildingDropdown);
        }

        private void OnEnable()
        {
            RefreshUI();

            SetupIndexResultFromDropdown();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            // TODO Find Logic to ShowOfferText if needed. & Update accordingly
            byte goodReputationAmount = 0;
            _offerText.text = _approvalYesPopup.OfferText.GetLocalizedString(goodReputationAmount);
        }

        private bool Validate()
        {
            bool status = true;

            if (!IsRoomNumberValidated()) status = false;
            return status;
        }

        private bool IsRoomNumberValidated() => _inputFieldValidator.ValidateNotNull(
            _roomNumberInputField.text, _roomNumberInputFieldStatus, out _roomNumber,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private void SetupIndexResultFromDropdown()
        {
            _buildingIndex = (byte)_buildingDropdown.index;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _approvalYesPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void BuildingDropdown(int index)
        {
            _buildingIndex = (byte)index;
        }

        private void Cancel() => _approvalYesPopup.OnCancelButtonClick();

        private void Confirm()
        {
            if (Validate())
                _approvalYesPopup.OnValidateSucceeded();
            else
                _approvalYesPopup.OnValidateFailed();
        }
        #endregion
    }
}