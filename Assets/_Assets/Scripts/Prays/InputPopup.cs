using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Prays
{
    public class InputPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Input Popup Settings")]
        [field: SerializeField] public string StatusTextDefault { get; private set; } = "Enter your desired number!";
        [field: SerializeField] public string StatusTextValid { get; private set; } = "Good job!";
        [field: SerializeField] public string StatusTextInvalid { get; private set; } = "Only positive value are allow!";
        [field: SerializeField] public string StatusTextCantParse { get; private set; } = "Error! Wrong number format.";
        [field: SerializeField] public string StatusTextCantClick { get; private set; } = "Please enter a Valid number";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Input Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onOkButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            Debug.LogWarning("Click \"Cancel\" Button! on Popup");

            _onCancelButtonClick?.Invoke();
        }

        public void OnOkButtonClick()
        {
            Debug.LogWarning("Click \"OK\" Button! on Popup");

            _onOkButtonClick?.Invoke();
        }
        #endregion
    }
}