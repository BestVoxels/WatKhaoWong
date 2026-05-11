using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using WatKhaoWong.Retreats;

namespace WatKhaoWong.UI.Retreats
{
    public class StatusSetterUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Settings")]
        [SerializeField] private StatusSetterInspector _ui;
        #endregion



        #region --Fields-- (In Class)
        private byte _accountStatusIndex;
        private DateTime _dateEndsOn = default;

        private StatusSetter _statusSetter;
        private SetADatePopup _setADatePopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _statusSetter = player.GetComponentInChildren<StatusSetter>();
            _setADatePopup = player.GetComponentInChildren<SetADatePopup>();

            _ui.statusDropdown.onValueChanged.AddListener(StatusSetterDropdownValue);
            _ui.setTimeButton.onClick.AddListener(StatusSetterSetTime);
            _ui.confirmButton.onClick.AddListener(StatusSetterConfirm);

            LocalizationSettings.SelectedLocaleChanged += (obj) => { UpdateInfoText(_dateEndsOn); };
        }

        private void OnEnable()
        {
            _setADatePopup.OnValidated += UpdateInfoText;
        }

        private void Start()
        {
            _accountStatusIndex = (byte)_ui.statusDropdown.index;

            UpdateInfoText(default);

            ToShowGameObjects();
        }

        private void OnDisable()
        {
            _setADatePopup.OnValidated -= UpdateInfoText;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool Validate()
        {
            bool status = true;

            if (IsBanTemporaySelected() && !IsSetADateValidated()) status = false;

            return status;
        }

        private bool IsSetADateValidated() => _setADatePopup.ValidateSetADatePopup();

        private void ToShowGameObjects()
        {
            foreach (GameObject each in _ui.gameOjectsToShowHide)
            {
                each.SetActive(IsBanTemporaySelected());
            }
        }

        private bool IsBanTemporaySelected() => _ui.statusDropdown.selectedText.text == _statusSetter.BanTemporayNameOnDropdown;
        #endregion



        #region --Methods-- (Subscriber)
        private void StatusSetterDropdownValue(int index)
        {
            _accountStatusIndex = (byte)index;

            ToShowGameObjects();
        }

        private void StatusSetterSetTime()
        {
            _statusSetter.OnSetTimeButtonClick();
        }

        private void StatusSetterConfirm()
        {
            if (Validate())
            {
                string notes = _ui.notesInputField.text;

                _statusSetter.OnValidateSucceeded(_accountStatusIndex, _dateEndsOn, notes);
            }
            else
            {
                _statusSetter.OnValidateFailed();
            }
        }

        private void UpdateInfoText(DateTime date)
        {
            _dateEndsOn = date;

            _ui.infoText.text = $"{_statusSetter.DateEndsOnText.GetLocalizedString()}: {_setADatePopup.FormatInfoString(date, _statusSetter.DayFormat)}";
        }
        #endregion
    }
}