using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Prays
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
        private void Start()
        {
            _undoPopup = GameObject.FindWithTag("Player").GetComponentInChildren<UndoPopup>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SaveToTempPlace(int result)
        {
            if (result <= 0) return;

            Debug.LogWarning($"Save Points ({result}) to _tempTMPoints field under ConfirmPopup.cs");

            _tempTMPoints = result;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            Debug.LogWarning("Click \"Cancel\" Button! on Popup");

            _tempTMPoints = 0;

            _onCancelButtonClick?.Invoke();
        }

        public void OnConfirmButtonClick()
        {
            Debug.LogWarning("Click \"Confirm\" Button! on Popup");

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