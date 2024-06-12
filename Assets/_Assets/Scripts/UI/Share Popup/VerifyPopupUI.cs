using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WatKhaoWong.SharePopup;
using WatKhaoWong.UI.InputFields;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.SharePopup
{
    public class VerifyPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Verify Popup UI Stuffs")]
        [SerializeField] private TMP_InputField _codeInputField;
        [Space]
        [SerializeField] private EventTrigger _informTextEventTrigger;
        [SerializeField] private EventTrigger _resendTextEventTrigger;
        [Space]
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        // TODO Temp Maybe? Have to check with firebase again on how to implement this.
        private string _generatedCode = "123456";

        private VerifyPopup _playerVerifyPopup;
        private StatusText _statusText;
        private InputFieldValidator _inputFieldValidator;
        private InputFieldStatus _codeInputFieldStatus;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerVerifyPopup = GameObject.FindWithTag("Player").GetComponentInChildren<VerifyPopup>();
            _statusText = FindAnyObjectByType<StatusText>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _codeInputFieldStatus = _codeInputField.GetComponentInChildren<InputFieldStatus>();

            _closeButton.onClick.AddListener(Close);

            _codeInputField.onEndEdit.AddListener(inputText => IsCodeValidated());

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => InformText((PointerEventData)data));
            _informTextEventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => ResendText((PointerEventData)data));
            _resendTextEventTrigger.triggers.Add(entry);

            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool Validate()
        {
            bool status = true;

            if (!IsCodeValidated()) status = false;

            return status;
        }

        private void SendStatusText()
        {
            _statusText.Show(_playerVerifyPopup.StatusResendCode, _playerVerifyPopup.StatusResendCodeColor);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerVerifyPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private bool IsCodeValidated() => _inputFieldValidator.ValidateCode(
            _codeInputField.text, _codeInputFieldStatus, out _,
            _playerVerifyPopup.MinimumCodeLength, _generatedCode,
            (string.Empty, default),
            (_playerVerifyPopup.StatusCodeTooShort, _playerVerifyPopup.StatusCodeTooShortColor),
            (_playerVerifyPopup.StatusCodeNotMatch, _playerVerifyPopup.StatusCodeNotMatchColor));

        private void InformText(PointerEventData data)
        {
            SendStatusText();

            _playerVerifyPopup.OnInformTextClick();
        }

        private void ResendText(PointerEventData data)
        {
            SendStatusText();

            _playerVerifyPopup.OnResendTextClick();
        }

        private void Confirm()
        {
            if (Validate())
            {
                // TODO Do something with server maybe?

                _playerVerifyPopup.OnConfirmButtonClick();
            }
            else
            {
                _playerVerifyPopup.OnConfirmButtonCantClick();
            }
        }
        #endregion
    }
}