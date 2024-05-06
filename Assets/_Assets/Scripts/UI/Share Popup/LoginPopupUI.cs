using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WatKhaoWong.SharePopup;

namespace WatKhaoWong.UI.SharePopup
{
    public class LoginPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Login Popup UI Stuffs")]
        [SerializeField] private TMP_InputField _userNameInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [Space]
        [SerializeField] private EventTrigger _signupTextEventTrigger;
        [SerializeField] private EventTrigger _forgotTextEventTrigger;
        [Space]
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private LoginPopup _playerLoginPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerLoginPopup = GameObject.FindWithTag("Player").GetComponentInChildren<LoginPopup>();

            _closeButton.onClick.AddListener(Close);

            _userNameInputField.onEndEdit.AddListener(UpdateUserNameInputField);
            _passwordInputField.onEndEdit.AddListener(UpdatePasswordInputField);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => SignupText((PointerEventData)data));
            _signupTextEventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => ForgotText((PointerEventData)data));
            _forgotTextEventTrigger.triggers.Add(entry);

            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerLoginPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateUserNameInputField(string text)
        {

        }

        private void UpdatePasswordInputField(string text)
        {

        }

        private void SignupText(PointerEventData data) => _playerLoginPopup.OnSignupTextClick();

        private void ForgotText(PointerEventData data) => _playerLoginPopup.OnForgotTextClick();

        private void Confirm()
        {
            _playerLoginPopup.OnConfirmButtonClick();
        }
        #endregion
    }
}