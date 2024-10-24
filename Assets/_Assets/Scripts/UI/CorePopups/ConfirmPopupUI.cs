using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.CorePopups;

namespace WatKhaoWong.UI.CorePopups
{
    public class ConfirmPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Confirm Popup UI Stuffs")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _infoText;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private ConfirmPopup _confirmPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void Setup(ConfirmPopup confirmPopup)
        {
            if (!_confirmButton)
            {
                Debug.LogError($"Custom Error: No 'ConfirmPopup' component assigned to the UI. Please assign on {gameObject.name}");
                return;
            }

            _confirmPopup = confirmPopup;

            _titleText.text = _confirmPopup.TitleText.GetLocalizedString();

            _infoText.text = _confirmPopup.InfoText.GetLocalizedString();
            _infoText.color = _confirmPopup.InfoTextColor;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _confirmPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Cancel()
        {
            _confirmPopup.OnCancelButtonClick();
        }

        private void Confirm()
        {
            _confirmPopup.OnConfirmButtonClick();
        }
        #endregion
    }
}