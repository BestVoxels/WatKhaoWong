using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Prays;

namespace WatKhaoWong.UI.Prays
{
    public class ConfirmPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Confirm Popup UI Stuffs")]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private ConfirmPopup _playerConfirmPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerConfirmPopup = GameObject.FindWithTag("Player").GetComponentInChildren<ConfirmPopup>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerConfirmPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Cancel()
        {
            _playerConfirmPopup.OnCancelButtonClick();
        }

        private void Confirm()
        {
            _playerConfirmPopup.OnConfirmButtonClick();
        }
        #endregion
    }
}