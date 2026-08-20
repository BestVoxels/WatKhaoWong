using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
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
        [SerializeField] private TMP_Text _cancelButtonText;
        [SerializeField] private TMP_Text _confirmButtonText;
        [Space]
        [SerializeField] private Image _cancelButtonImage;
        [SerializeField] private Image _confirmButtonImage;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private Color32 _cancelButtonColorDefault;
        private Color32 _confirmButtonColorDefault;

        private ConfirmPopup _confirmPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);
        }

        private void Start()
        {
            _cancelButtonColorDefault = _cancelButtonImage.color;
            _confirmButtonColorDefault = _confirmButtonImage.color;
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
            
            // Cancel
            if (!_confirmPopup.CancelText.IsEmpty)
            {
                _cancelButtonText.GetComponent<LocalizeStringEvent>().enabled = false; // Need to Turn Off LocalizeStringEvent component first so our text will works without getting change by this.
                _cancelButtonText.text = _confirmPopup.CancelText.GetLocalizedString();
            }
            else
                _cancelButtonText.GetComponent<LocalizeStringEvent>().enabled = true; // Turn Back On

            if (_confirmPopup.CancelButtonColor.r != 0 || _confirmPopup.CancelButtonColor.g != 0 || _confirmPopup.CancelButtonColor.b != 0 || _confirmPopup.CancelButtonColor.a != 0)
                _cancelButtonImage.color = _confirmPopup.CancelButtonColor;
            else
                _cancelButtonImage.color = _cancelButtonColorDefault;

            // Confirm
            if (!_confirmPopup.ConfirmText.IsEmpty)
            {
                _confirmButtonText.GetComponent<LocalizeStringEvent>().enabled = false; // Need to Turn Off LocalizeStringEvent component first so our text will works without getting change by this.
                _confirmButtonText.text = _confirmPopup.ConfirmText.GetLocalizedString();
            }
            else
                _confirmButtonText.GetComponent<LocalizeStringEvent>().enabled = true; // Turn Back On

            if (_confirmPopup.ConfirmButtonColor.r != 0 || _confirmPopup.ConfirmButtonColor.g != 0 || _confirmPopup.ConfirmButtonColor.b != 0 || _confirmPopup.ConfirmButtonColor.a != 0)
                _confirmButtonImage.color = _confirmPopup.ConfirmButtonColor;
            else
                _confirmButtonImage.color = _confirmButtonColorDefault;
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