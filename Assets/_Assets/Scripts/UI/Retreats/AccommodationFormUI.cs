using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;
using WatKhaoWong.Utils.UI;
using Michsky.MUIP;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.Localization;
using System;

namespace WatKhaoWong.UI.Retreats
{
    public class AccommodationFormUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        [Header("AccommodationForm UI Stuffs")]
        [SerializeField] private AccountStatusInspector _accountStatusUI;
        [Space]
        [SerializeField] private CustomDropdown _activityDropdown;
        [SerializeField] private Button _setTimeButton;
        [SerializeField] private SwitchManager _hasCarSwitch;
        [SerializeField] private TMP_InputField _plateNumberInputField;
        [Space]
        [SerializeField] private TMP_Text _activityResultText;
        [SerializeField] private TMP_Text _setTimeResultText;
        [SerializeField] private TMP_Text _hasCarResultText;
        [SerializeField] private TMP_Text _plateNumberResultText;
        [Space]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _printButton;
        [Space]
        [SerializeField] private GameObject _rowMenuPlateNumber;
        [SerializeField] private GameObject _uploadPanel;
        [SerializeField] private GameObject _printPanel;
        [SerializeField] private GameObject _formPanel;
        [SerializeField] private GameObject _reasonBannedPanel;
        [Space]
        [SerializeField] private TMP_Text _reasonText;
        #endregion



        #region --Fields-- (In Class)
        private string _plateNumber;
        private StayEntry _stayEntry;

        private MyUserData _myUserData;
        private Localizer _localizer;
        private AccommodationSetTimePopup _setTimePopup;
        private AccommodationForm _accommodationForm;
        private StayEntryRow _stayEntryRow;
        private InputFieldValidator _inputFieldValidator;
        private InputFieldStatus _plateNumberInputFieldStatus;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _setTimePopup = player.GetComponentInChildren<AccommodationSetTimePopup>();
            _accommodationForm = player.GetComponentInChildren<AccommodationForm>();
            _stayEntryRow = player.GetComponentInChildren<StayEntryRow>();

            _localizer = FindAnyObjectByType<Localizer>();

            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _plateNumberInputFieldStatus = _plateNumberInputField.GetComponent<InputFieldStatus>();
            
            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            // No need to subscribe to dropdown since we can just get its value when user click 'Confirm'
            _setTimeButton.onClick.AddListener(SetTime);
            _hasCarSwitch.onValueChanged.AddListener(ShowHidePlateNumber);
            _plateNumberInputField.onEndEdit.AddListener(inputText => IsPlateNumberValidated());

            _confirmButton.onClick.AddListener(Confirm);
            _printButton.onClick.AddListener(Print);

            _stayEntryRow.OnAddedToServer += RefreshUIWithServerData;
            _stayEntryRow.OnDeletedFromServer += (nullEntry) => RefreshUIWithServerData(nullEntry, null);
            UIRefresher.OnMeditationRetreatRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
            UIRefresher.OnLocalizeDynamicString += () => { SetTextWhenEntryExists(); RefreshAccountStatusUI(); }; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
        }

        private void OnEnable()
        {
            _setTimePopup.OnValidated += UpdateTextOnButtonSetTime;
            _accommodationForm.OnUploadedToServer += RefreshUIWithServerData;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            _setTimePopup.OnValidated -= UpdateTextOnButtonSetTime;
            _accommodationForm.OnUploadedToServer -= RefreshUIWithServerData;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            RefreshAccountStatusUI();

            if (IsStatusBanned())
            {
                ShowReasonBannedUI();
            }
            else
            {
                ShowResultTextsUI();
            }
        }

        private void RefreshAccountStatusUI() => _myUserData.UpdateAccountStatus(_accountStatusUI, _myUserData.GetAccountStatus(), _localizer);

        private bool IsStatusBanned()
        {
            AccountStatus accountStatus = _myUserData.GetAccountStatus();
            if (accountStatus == null) return false; // Default is not Ban
            if (accountStatus.StatusInfo == null) return false; // Default is not Ban

            Enum.TryParse(accountStatus.StatusInfo.Status, true, out EAccountStatus eStatus);

            return eStatus == EAccountStatus.BanTemporary || eStatus == EAccountStatus.BanPermanent;
        }

        private void ShowReasonBannedUI()
        {
            _formPanel.SetActive(false);
            _printPanel.SetActive(false);
            _reasonBannedPanel.SetActive(true);

            AccountStatus accountStatus = _myUserData.GetAccountStatus();
            if (accountStatus == null) return;
            if (accountStatus.StatusInfo == null) return;

            _reasonText.text = accountStatus.NotesInfo.Text;

            ColorUtility.TryParseHtmlString(accountStatus.NotesInfo.Color, out Color notesColor);
            _reasonText.color = notesColor;
        }

        private async void ShowResultTextsUI()
        {
            if (_stayEntry == null)
                _stayEntry = await _myUserData.GetActiveStayEntry();

            _formPanel.SetActive(true);
            _printPanel.SetActive(false);
            _reasonBannedPanel.SetActive(false);

            ShowHideFormUIBaseOnStayEntryExists();

            SetTextWhenEntryExists();
        }

        private bool Validate()
        {
            bool status = true;

            if (!IsSetTimeValidated()) status = false;
            if (_hasCarSwitch.isOn && !IsPlateNumberValidated()) status = false;

            return status;
        }

        private void ShowHideFormUIBaseOnStayEntryExists()
        {
            bool isEntryNull = _stayEntry == null;

            _activityDropdown.gameObject.SetActive(isEntryNull);
            _setTimeButton.gameObject.SetActive(isEntryNull);
            _hasCarSwitch.gameObject.SetActive(isEntryNull);

            _activityResultText.gameObject.SetActive(!isEntryNull);
            _setTimeResultText.gameObject.SetActive(!isEntryNull);
            _hasCarResultText.gameObject.SetActive(!isEntryNull);
            
            if (isEntryNull || GetHasCar() == EHasCar.Has)
            {
                _hasCarSwitch.isOn = true;
                ShowHidePlateNumber(true);

                _plateNumberInputField.gameObject.SetActive(isEntryNull);

                _plateNumberResultText.gameObject.SetActive(!isEntryNull);
            }
            else if (GetHasCar() == EHasCar.None)
            {
                ShowHidePlateNumber(false);
            }

            _uploadPanel.SetActive(isEntryNull);
            _printPanel.SetActive(!isEntryNull);
        }

        private void SetTextWhenEntryExists()
        {
            if (_stayEntry == null) return;

            _activityResultText.text = _localizer.LocalizeActivityType(_stayEntry.Activity);

            _stayEntry.StayInfo.StartDate.TryParseGregorian(out DateTime startDate);
            _stayEntry.StayInfo.EndDate.TryParseGregorian(out DateTime endDate);
            _setTimeResultText.text = _setTimePopup.FormatButtonString(startDate, endDate, _accommodationForm.DayFormat);

            _hasCarResultText.text = _localizer.LocalizeHasCar(_stayEntry.Transportation.HasCar);

            if (GetHasCar() == EHasCar.Has)
                _plateNumberResultText.text = _stayEntry.Transportation.CarPlateNumber;
            else
                _plateNumberResultText.text = _accommodationForm.NoDataText.GetLocalizedString();
        }

        private EHasCar GetHasCar() => (EHasCar)Enum.Parse(typeof(EHasCar), _stayEntry.Transportation.HasCar);
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _accommodationForm.OnBackButtonClick();
        private void ChangeLang() => _accommodationForm.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void SetTime() => _accommodationForm.OnSetTimeButtonClick();
        private void UpdateTextOnButtonSetTime(DateTime startDate, DateTime endDate)
        {
            TMP_Text buttonText = _setTimeButton.GetComponentInChildren<TMP_Text>();

            buttonText.text = _setTimePopup.FormatButtonString(startDate, endDate, _accommodationForm.DayFormat);
        }

        private void ShowHidePlateNumber(bool hasCar) => _rowMenuPlateNumber.SetActive(hasCar);

        private bool IsSetTimeValidated() => _setTimePopup.ValidateSetTimePopup();

        private bool IsPlateNumberValidated() => _inputFieldValidator.ValidateNotNull(
            _plateNumberInputField.text, _plateNumberInputFieldStatus, out _plateNumber,
            (_accommodationForm.StatusMustBeFilled.GetLocalizedString(), _accommodationForm.StatusMustBeFilledColor));

        public void RefreshUIWithServerData(StayEntry stayEntry, EStayStatus? stayStatus)
        {
            if (stayStatus == EStayStatus.Completed || stayStatus == EStayStatus.Rejected)
                return;

            _stayEntry = stayEntry;

            ShowResultTextsUI();
        }

        private void Confirm()
        {
            if (Validate())
            {
                EHasCar hasCar = _hasCarSwitch.isOn ? EHasCar.Has : EHasCar.None;
                if (hasCar == EHasCar.None)
                    _plateNumber = null;

                _accommodationForm.OnValidateSucceeded((byte)_activityDropdown.index, _setTimePopup.GetData(), hasCar, _plateNumber);
            }
            else
            {
                _accommodationForm.OnValidateFailed();
            }
        }

        private void Print()
        {
            _accommodationForm.OnPrintButtonClick();
        }
        #endregion
    }
}