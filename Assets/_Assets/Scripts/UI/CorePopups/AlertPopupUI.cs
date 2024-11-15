using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.CorePopups;

namespace WatKhaoWong.UI.CorePopups
{
    public class AlertPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Alert Popup UI Stuffs")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _infoText;
        [Space]
        [SerializeField] private Button _alertButton;
        #endregion



        #region --Fields-- (In Class)
        private AlertPopup _alertPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _closeButton.onClick.AddListener(Close);

            _alertButton.onClick.AddListener(Alert);
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void Setup(AlertPopup AlertPopup)
        {
            if (!_alertButton)
            {
                Debug.LogError($"Custom Error: No 'AlertPopup' component assigned to the UI. Please assign on {gameObject.name}");
                return;
            }

            _alertPopup = AlertPopup;

            _titleText.text = _alertPopup.TitleText.GetLocalizedString();

            _infoText.text = _alertPopup.InfoText.GetLocalizedString();
            _infoText.color = _alertPopup.InfoTextColor;
        }

        public void SetupTitleSmartString(string input)
        {
            _titleText.text = _alertPopup.TitleText.GetLocalizedString(input);
        }

        public void SetupInfoSmartString(string input)
        {
            _infoText.text = _alertPopup.InfoText.GetLocalizedString(input);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _alertPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Alert()
        {
            _alertPopup.OnAlertButtonClick();
        }
        #endregion
    }
}