using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;
using Bitsplash.DatePicker;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Attributes;
using Firebase.Auth;

namespace WatKhaoWong.UI.Retreats
{
    public class AccommodationSetTimePopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        //[Header("Challenge Creation Popup UI Stuffs")]
        //[SerializeField] private DatePickerSettings _datePickerStart;
        //[SerializeField] private DatePickerSettings _datePickerEnd;
        //[Space]
        //[SerializeField] private TMP_Text _startDateText;
        //[SerializeField] private TMP_Text _endDateText;
        //[SerializeField] private TMP_Text _durationText;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        //private DateTime _startDate;
        //private DateTime _endDate;
        //private TimeSpan _duration;

        //private StatusText _statusText;
        private AccommodationSetTimePopup _setTimePopup;
        //private Challenge _challenge;
        //private ServerTime _serverTime;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _setTimePopup = GameObject.FindWithTag("Player").GetComponentInChildren<AccommodationSetTimePopup>();
            //_challenge = GameObject.FindWithTag("Player").GetComponentInChildren<Challenge>();
            //_statusText = FindAnyObjectByType<StatusText>();
            //_serverTime = FindAnyObjectByType<ServerTime>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);

            //_datePickerStart.Content.OnSelectionChanged.AddListener(SelectedDateOnStartCalendar);
            ////_datePickerStart.Content.OnDisplayChanged.AddListener(() => print("StartCalendar: Calls when Click Change Month or Year"));
            
            //_datePickerEnd.Content.OnSelectionChanged.AddListener(SelectedDateOnEndCalendar);
            ////_datePickerEnd.Content.OnDisplayChanged.AddListener(() => print("EndCalendar: Calls when Click Change Month or Year"));

            //UIRefresher.OnLocalizeDynamicString += RefreshUI;
        }

        //private void OnEnable()
        //{
        //    FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
        //}

        //private void Start()
        //{
        //    RefreshUI();
        //}

        //private void OnDisable()
        //{
        //    FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        //}
        #endregion



        //#region --Methods-- (Custom PRIVATE)
        //private void RefreshUI()
        //{
        //    _startDateText.text = _setTimePopup.StartDateFormat.GetLocalizedString(_challenge.FormatDateString(_startDate, _setTimePopup.DateStringFormat));

        //    _endDateText.text = _setTimePopup.EndDateFormat.GetLocalizedString(_challenge.FormatDateString(_endDate, _setTimePopup.DateStringFormat));

        //    _durationText.text = _setTimePopup.DurationFormat.GetLocalizedString(_challenge.FormatDurationString(_duration));
        //}
        //#endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _setTimePopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Cancel()
        {
            _setTimePopup.OnCancelButtonClick();
        }

        private void Confirm()
        {
            //if (_setTimePopup.ValidateChallengePopup(_startDate, _endDate))
            if (true)
            {
                //_setTimePopup.SetDataAwaitConfirmation(_startDate, _endDate, _duration);

                _setTimePopup.OnConfirmButtonClick();
            }
            else
            {
                _setTimePopup.OnConfirmButtonCantClick();

                //_statusText.Show(_setTimePopup.StatusCreateFailed.GetLocalizedString(), _setTimePopup.StatusCreateFailedColor);
            }
        }

        //private void SelectedDateOnStartCalendar()
        //{
        //    _startDate = _datePickerStart.Content.Selection.GetItem(0); // To get multiple Date, check 'SelectionTutorial.cs' line 60
        //    _duration = _setTimePopup.GetChallengeDuration(_startDate, _endDate);
        //    _setTimePopup.ValidateStartDate(_startDate, _endDate);

        //    RefreshUI();
        //}

        //private void SelectedDateOnEndCalendar()
        //{
        //    _endDate = _datePickerEnd.Content.Selection.GetItem(0); // To get multiple Date, check 'SelectionTutorial.cs' line 60
        //    _duration = _setTimePopup.GetChallengeDuration(_startDate, _endDate);
        //    _setTimePopup.ValidateEndDate(_startDate, _endDate);

        //    RefreshUI();
        //}

        ///// <summary>
        ///// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        ///// </summary>
        //private async void HandleStateChanged(object obj, EventArgs args)
        //{
        //    DateTime nowDate = await _serverTime.Now();

        //    _datePickerStart.Content.SetMarkerColor(nowDate);
        //    _datePickerEnd.Content.SetMarkerColor(nowDate);
        //}
        #endregion
    }
}