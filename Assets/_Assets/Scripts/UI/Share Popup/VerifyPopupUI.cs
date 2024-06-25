using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WatKhaoWong.SharePopup;
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
        private string _typedCode;

        private VerifyPopup _playerVerifyPopup;
        private InputFieldValidator _inputFieldValidator;
        private InputFieldStatus _codeInputFieldStatus;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerVerifyPopup = GameObject.FindWithTag("Player").GetComponentInChildren<VerifyPopup>();
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _codeInputFieldStatus = _codeInputField.GetComponentInChildren<InputFieldStatus>();

            BindUIWithFunction();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void BindUIWithFunction()
        {
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

        private bool Validate()
        {
            bool status = true;

            if (!IsCodeValidated()) status = false;

            return status;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerVerifyPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private bool IsCodeValidated() => _inputFieldValidator.ValidateCode(
            _codeInputField.text, _codeInputFieldStatus, out _typedCode,
            _playerVerifyPopup.MinimumCodeLength,
            (string.Empty, default),
            (_playerVerifyPopup.StatusCodeTooShort, _playerVerifyPopup.StatusCodeTooShortColor));

        private void InformText(PointerEventData data)
        {
            _playerVerifyPopup.OnInformTextClick();
        }

        private void ResendText(PointerEventData data)
        {
            _playerVerifyPopup.OnResendTextClick();
        }

        private void Confirm()
        {
            if (Validate())
            {
                _playerVerifyPopup.OnValidateSucceeded(_typedCode);
            }
            else
            {
                _playerVerifyPopup.OnValidateFailed();
            }
        }
        #endregion
    }
}