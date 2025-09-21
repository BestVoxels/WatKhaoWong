using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;
using Bitsplash.DatePicker;
using WatKhaoWong.Attributes;
using Firebase.Auth;

namespace WatKhaoWong.UI.Retreats
{
    public class SetADatePopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Set A Date Popup UI Stuffs")]
        [SerializeField] private DatePickerSettings _datePicker;
        [Space]
        [SerializeField] private TMP_Text _dateText;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private DateTime _date;

        private SetADatePopup _setADatePopup;
        private ServerTime _serverTime;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _setADatePopup = GameObject.FindWithTag("Player").GetComponentInChildren<SetADatePopup>();
            _serverTime = FindAnyObjectByType<ServerTime>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);

            _datePicker.Content.OnSelectionChanged.AddListener(SelectedDateOnCalendar);
            //_datePicker.Content.OnDisplayChanged.AddListener(() => print("Calendar: Calls when Click Change Month or Year"));

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
            _dateText.text = _setADatePopup.DateText.GetLocalizedString(_setADatePopup.FormatDateString(_date, _setADatePopup.DateStringFormat));
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _setADatePopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Cancel()
        {
            _setADatePopup.OnCancelButtonClick();
        }

        private void Confirm()
        {
            if (_setADatePopup.ValidateSetADatePopup(_date))
            {
                _setADatePopup.SetDataReadyForUse(_date);

                _setADatePopup.OnConfirmButtonClick();
            }
            else
            {
                _setADatePopup.OnConfirmButtonCantClick();
            }
        }

        private void SelectedDateOnCalendar()
        {
            _date = _datePicker.Content.Selection.GetItem(0); // To get multiple Date, check 'SelectionTutorial.cs' line 60
            _setADatePopup.ValidateDate(_date);

            RefreshUI();
        }

        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        /// </summary>
        private async void HandleStateChanged(object obj, EventArgs args)
        {
            DateTime nowDate = await _serverTime.Now();

            _datePicker.Content.SetMarkerColor(nowDate);
        }
        #endregion
    }
}