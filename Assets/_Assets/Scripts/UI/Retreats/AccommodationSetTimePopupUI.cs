using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;
using Bitsplash.DatePicker;
using WatKhaoWong.Attributes;
using Firebase.Auth;
using Michsky.MUIP;

namespace WatKhaoWong.UI.Retreats
{
    public class AccommodationSetTimePopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Accommodation Set Time Popup UI Stuffs")]
        [SerializeField] private SwitchManager _notStayingOvernightSwitch;
        [Space]
        [SerializeField] private DatePickerSettings _datePickerStart;
        [SerializeField] private DatePickerSettings _datePickerEnd;
        [Space]
        [SerializeField] private TMP_Text _startDateText;
        [SerializeField] private TMP_Text _endDateText;
        [SerializeField] private TMP_Text _durationText;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        [Space]
        [SerializeField] private GameObject[] _gameObjectsToShowHide;
        #endregion



        #region --Fields-- (In Class)
        private EIsStaying _isStaying;
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _duration;

        private AccommodationSetTimePopup _setTimePopup;
        private ServerTime _serverTime;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _setTimePopup = GameObject.FindWithTag("Player").GetComponentInChildren<AccommodationSetTimePopup>();
            _serverTime = FindAnyObjectByType<ServerTime>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);

            _notStayingOvernightSwitch.onValueChanged.AddListener(ToggleOvernightSwitch);

            _datePickerStart.Content.OnSelectionChanged.AddListener(SelectedDateOnStartCalendar);
            //_datePickerStart.Content.OnDisplayChanged.AddListener(() => print("StartCalendar: Calls when Click Change Month or Year"));

            _datePickerEnd.Content.OnSelectionChanged.AddListener(SelectedDateOnEndCalendar);
            //_datePickerEnd.Content.OnDisplayChanged.AddListener(() => print("EndCalendar: Calls when Click Change Month or Year"));

            UIRefresher.OnLocalizeDynamicString += RefreshUI;
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
        }

        private void Start()
        {
            RefreshUI();

            _notStayingOvernightSwitch.SetOff();
            ToggleOvernightSwitch(_notStayingOvernightSwitch.isOn);
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            _startDateText.text = _setTimePopup.StartDateText.GetLocalizedString(_setTimePopup.FormatDateString(_startDate, _setTimePopup.DateStringFormat));

            _endDateText.text = _setTimePopup.EndDateText.GetLocalizedString(_setTimePopup.FormatDateString(_endDate, _setTimePopup.DateStringFormat));

            _durationText.text = _setTimePopup.DurationText.GetLocalizedString(_setTimePopup.FormatDurationString(_duration));
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _setTimePopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void ToggleOvernightSwitch(bool isNotStaying)
        {
            _isStaying = isNotStaying ? EIsStaying.NotStaying : EIsStaying.Staying;

            _setTimePopup.SetIsStayingOvernight(_isStaying);

            // Update UI accordingly
            foreach (var each in _gameObjectsToShowHide)
            {
                each.SetActive(!isNotStaying);
            }

            if (isNotStaying)
                _endDate = default;
        }

        private void Cancel()
        {
            _setTimePopup.OnCancelButtonClick();
        }

        private void Confirm()
        {
            if (_setTimePopup.ValidateSetTimePopup(_startDate, _endDate))
            {
                _setTimePopup.SetDataReadyForUse(_isStaying, _startDate, _endDate, _duration);

                _setTimePopup.OnConfirmButtonClick();
            }
            else
            {
                _setTimePopup.OnConfirmButtonCantClick();
            }
        }

        private void SelectedDateOnStartCalendar()
        {
            _startDate = _datePickerStart.Content.Selection.GetItem(0); // To get multiple Date, check 'SelectionTutorial.cs' line 60
            _duration = _setTimePopup.GetDuration(_startDate, _endDate);
            _setTimePopup.ValidateStartDate(_startDate, _endDate);

            RefreshUI();
        }

        private void SelectedDateOnEndCalendar()
        {
            _endDate = _datePickerEnd.Content.Selection.GetItem(0); // To get multiple Date, check 'SelectionTutorial.cs' line 60
            _duration = _setTimePopup.GetDuration(_startDate, _endDate);
            _setTimePopup.ValidateEndDate(_startDate, _endDate);

            RefreshUI();
        }

        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        /// </summary>
        private async void HandleStateChanged(object obj, EventArgs args)
        {
            DateTime nowDate = await _serverTime.Now();

            _datePickerStart.Content.SetMarkerColor(nowDate);
            _datePickerEnd.Content.SetMarkerColor(nowDate);
        }
        #endregion
    }
}