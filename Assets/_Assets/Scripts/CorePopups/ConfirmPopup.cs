using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Prays;

namespace WatKhaoWong.CorePopups
{
    public class ConfirmPopup : Popup
    {
        #region --Events-- (UnityEvent)
        [Header("Confirm Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonClick;
        #endregion



        #region --Fields-- (In Class)
        private int _tempTMPoints;

        private UndoPopup _undoPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _undoPopup = GameObject.FindWithTag("Player").GetComponentInChildren<UndoPopup>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SaveToTempPlace(int result)
        {
            if (result <= 0) return;

            _tempTMPoints = result;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            _tempTMPoints = 0;

            _onCancelButtonClick?.Invoke();
        }

        public void OnConfirmButtonClick()
        {
            _undoPopup.StartUploadToServer(_tempTMPoints);

            _onConfirmButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Override)
        public override void OnCloseButtonClick()
        {
            _tempTMPoints = 0;

            base.OnCloseButtonClick();
        }
        #endregion
    }
}