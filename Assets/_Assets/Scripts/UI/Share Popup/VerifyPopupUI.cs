using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WatKhaoWong.SharePopup;

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
        private VerifyPopup _playerVerifyPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerVerifyPopup = GameObject.FindWithTag("Player").GetComponentInChildren<VerifyPopup>();

            _closeButton.onClick.AddListener(Close);

            _codeInputField.onEndEdit.AddListener(UpdateCodeInputField);

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



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerVerifyPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateCodeInputField(string text)
        {

        }

        private void InformText(PointerEventData data) => _playerVerifyPopup.OnInformTextClick();

        private void ResendText(PointerEventData data) => _playerVerifyPopup.OnResendTextClick();

        private void Confirm()
        {
            _playerVerifyPopup.OnConfirmButtonClick();
        }
        #endregion
    }
}