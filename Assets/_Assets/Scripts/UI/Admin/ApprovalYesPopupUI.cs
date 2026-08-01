using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Admin;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Retreats;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Localization;

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
        [SerializeField] private GameObject[] _toShowHideWhenIsStaying;
        [SerializeField] private GameObject[] _toShowHideWhenNotStaying;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private StayEntry _stayEntry;
        private string _keyId;
        private IUserData _userData;
        private byte _buildingIndex;
        private string _roomNumber;

        private AccommodationApproval _accommodationApproval;
        private UserInfo _userInfo;
        private ApprovalYesPopup _approvalYesPopup;
        private InputFieldValidator _inputFieldValidator;
        private SavingWrapper _savingWrapper;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _accommodationApproval = player.GetComponentInChildren<AccommodationApproval>();
            _userInfo = player.GetComponentInChildren<UserInfo>();
            _approvalYesPopup = player.GetComponentInChildren<ApprovalYesPopup>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);

            _buildingDropdown.onValueChanged.AddListener(BuildingDropdown);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            ToShowOfferText();

            ToShowHideIfIsStaying();
        }

        private bool Validate()
        {
            bool status = true;

            if (IsStayingOvernight() && !IsRoomNumberValidated()) status = false;
            return status;
        }

        private bool IsRoomNumberValidated() => _inputFieldValidator.ValidateNotNull(
            _roomNumberInputField.text, _roomNumberInputFieldStatus, out _roomNumber,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private void SetupIndexResultFromDropdown()
        {
            _buildingIndex = (byte)_buildingDropdown.index;
        }

        private async void ToShowOfferText()
        {
            _offerText.gameObject.SetActive(false);

            byte goodReputationAmount = await GetGoodReputationAmount();
            if (goodReputationAmount >= _accommodationApproval.TargetToOffer)
            {
                _offerText.gameObject.SetActive(true);
                _offerText.text = _approvalYesPopup.OfferText.GetLocalizedString(goodReputationAmount);
            }
        }

        private void ToShowHideIfIsStaying()
        {
            if (IsStayingOvernight())
            {
                foreach (GameObject each in _toShowHideWhenIsStaying)
                    each.SetActive(true);

                foreach (GameObject each in _toShowHideWhenNotStaying)
                    each.SetActive(false);
                return;
            }

            foreach (GameObject each in _toShowHideWhenIsStaying)
                    each.SetActive(false);

            foreach (GameObject each in _toShowHideWhenNotStaying)
                each.SetActive(true);
        }

        private async Task<byte> GetGoodReputationAmount()
        {            
            IAsyncEnumerable<(StayEntry, string)> rows = _savingWrapper.LoadPastEntryFromUser(_userData.GetUserKeyID());

            if (rows == null)
            {
                Debug.LogError("Error : There is no data on Server. Because 'rows' is null.");
                return 0;
            }

            byte goodReputationAmount = 0;
            await foreach ((StayEntry stayEntry, string keyId) eachData in rows)
            {
                Enum.TryParse(eachData.stayEntry.Reputation, true, out EReputation eReputation);

                if (eReputation == EReputation.Good)
                    goodReputationAmount++;
            }

            return goodReputationAmount;
        }

        private bool IsStayingOvernight()
        {
            if (_stayEntry == null)
                return false;

            return ((EIsStaying)Enum.Parse(typeof(EIsStaying), _stayEntry.StayInfo.IsStaying)) == EIsStaying.Staying;
        }

        private void FilterIsNotStayingData()
        {
            if (IsStayingOvernight()) return;

            _roomNumber = null; // 'AccommodationApproval.cs' check if Null
            _buildingIndex = 0;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(StayEntry stayEntry, string keyId, IUserData userData)
        {
            _stayEntry = stayEntry;
            _keyId = keyId;
            _userData = userData;

            RefreshUI();
            SetupIndexResultFromDropdown();
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
            {
                FilterIsNotStayingData();
                _approvalYesPopup.OnValidateSucceeded(_stayEntry, _keyId, _userData, _buildingIndex, _roomNumber);
            }
            else
                _approvalYesPopup.OnValidateFailed();
        }
        #endregion
    }
}