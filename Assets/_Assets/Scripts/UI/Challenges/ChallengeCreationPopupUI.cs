using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Challenges;
using Bitsplash.DatePicker;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Attributes;
using Firebase.Auth;

namespace WatKhaoWong.UI.Challenges
{
    public class ChallengeCreationPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Challenge Creation Popup UI Stuffs")]
        [SerializeField] private DatePickerSettings _datePickerStart;
        [SerializeField] private DatePickerSettings _datePickerEnd;
        [Space]
        [SerializeField] private TMP_Text _startDateText;
        [SerializeField] private TMP_Text _endDateText;
        [SerializeField] private TMP_Text _durationText;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _duration;

        private StatusText _statusText;
        private ChallengeCreationPopup _challengeCreation;
        private Challenge _challenge;
        private ServerTime _serverTime;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _challengeCreation = GameObject.FindWithTag("Player").GetComponentInChildren<ChallengeCreationPopup>();
            _challenge = GameObject.FindWithTag("Player").GetComponentInChildren<Challenge>();
            _statusText = FindAnyObjectByType<StatusText>();
            _serverTime = FindAnyObjectByType<ServerTime>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);

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
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void RefreshUI()
        {
            _startDateText.text = _challengeCreation.StartDateFormat.GetLocalizedString(_challenge.FormatDateString(_startDate, _challengeCreation.DateStringFormat));

            _endDateText.text = _challengeCreation.EndDateFormat.GetLocalizedString(_challenge.FormatDateString(_endDate, _challengeCreation.DateStringFormat));

            _durationText.text = _challengeCreation.DurationFormat.GetLocalizedString(_challenge.FormatDurationString(_duration));
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _challengeCreation.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Cancel()
        {
            _challengeCreation.OnCancelButtonClick();
        }

        private void Confirm()
        {
            if (_challengeCreation.ValidateChallengePopup(_startDate, _endDate))
            {
                _challengeCreation.SetDataAwaitConfirmation(_startDate, _endDate, _duration);

                _challengeCreation.OnConfirmButtonClick();
            }
            else
            {
                _challengeCreation.OnConfirmButtonCantClick();

                _statusText.Show(_challengeCreation.StatusCreateFailed.GetLocalizedString(), _challengeCreation.StatusCreateFailedColor);
            }
        }

        private void SelectedDateOnStartCalendar()
        {
            _startDate = _datePickerStart.Content.Selection.GetItem(0); // To get multiple Date, check 'SelectionTutorial.cs' line 60
            _duration = _challengeCreation.GetChallengeDuration(_startDate, _endDate);
            _challengeCreation.ValidateStartDate(_startDate, _endDate);

            RefreshUI();
        }

        private void SelectedDateOnEndCalendar()
        {
            _endDate = _datePickerEnd.Content.Selection.GetItem(0); // To get multiple Date, check 'SelectionTutorial.cs' line 60
            _duration = _challengeCreation.GetChallengeDuration(_startDate, _endDate);
            _challengeCreation.ValidateEndDate(_startDate, _endDate);

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