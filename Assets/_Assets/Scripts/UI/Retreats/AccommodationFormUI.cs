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

            UIRefresher.OnMeditationRetreatRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
            UIRefresher.OnLocalizeDynamicString += () => { SetTextWhenEntryExists(); RefreshAccountStatusUI(); }; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
        }

        private void OnEnable()
        {
            _setTimePopup.OnValidated += UpdateTextOnButtonSetTime;
            _accommodationForm.OnUploadedToServer += UploadToServer;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            _setTimePopup.OnValidated -= UpdateTextOnButtonSetTime;
            _accommodationForm.OnUploadedToServer -= UploadToServer;
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
            Enum.TryParse(_myUserData.GetAccountStatus().StatusInfo.Status, true, out EAccountStatus eStatus);

            return eStatus == EAccountStatus.BanTemporary || eStatus == EAccountStatus.BanPermanent;
        }

        private void ShowReasonBannedUI()
        {
            _formPanel.SetActive(false);
            _printPanel.SetActive(false);
            _reasonBannedPanel.SetActive(true);

            AccountStatus accountStatus = _myUserData.GetAccountStatus();
            _reasonText.text = accountStatus.NotesInfo.Text;

            ColorUtility.TryParseHtmlString(accountStatus.NotesInfo.Color, out Color notesColor);
            _reasonText.color = notesColor;
        }

        private async void ShowResultTextsUI()
        {
            if (_stayEntry == null)
                _stayEntry = await _myUserData.GetMyEntryFromStayRequests();

            _formPanel.SetActive(true);
            _printPanel.SetActive(false);
            _reasonBannedPanel.SetActive(false);

            ShowHideUIWhenEntryExists();

            SetTextWhenEntryExists();
        }

        private bool Validate()
        {
            bool status = true;

            if (!IsSetTimeValidated()) status = false;
            if (_hasCarSwitch.isOn && !IsPlateNumberValidated()) status = false;

            return status;
        }

        private void ShowHideUIWhenEntryExists()
        {
            if (_stayEntry == null) return;

            _activityDropdown.gameObject.SetActive(false);
            _setTimeButton.gameObject.SetActive(false);
            _hasCarSwitch.gameObject.SetActive(false);

            _activityResultText.gameObject.SetActive(true);
            _setTimeResultText.gameObject.SetActive(true);
            _hasCarResultText.gameObject.SetActive(true);

            
            if (GetHasCar() == EHasCar.Has)
            {
                _plateNumberInputField.gameObject.SetActive(false);

                _plateNumberResultText.gameObject.SetActive(true);
            }
            else if (GetHasCar() == EHasCar.None)
            {
                ShowHidePlateNumber(false);
            }

            _uploadPanel.SetActive(false);
            _printPanel.SetActive(true);
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

        private void UploadToServer(StayEntry stayEntry)
        {
            _stayEntry = stayEntry;

            ShowResultTextsUI();
        }

        private void Confirm()
        {
            if (Validate())
            {
                EHasCar hasCar = _hasCarSwitch.isOn ? EHasCar.Has : EHasCar.None;

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