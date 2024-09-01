using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Prays
{
    public class InputPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Input Popup Validation Settings")]
        [Range(1, 9999)]
        [SerializeField] private int _maximumPointInput;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Input Popup Status Text")]
        [field: SerializeField] public string StatusTextDefault { get; private set; } = "Enter your desired number!";
        [field: SerializeField] public string StatusTextValid { get; private set; } = "Good job!";
        [field: SerializeField] public string StatusTextNoNegative { get; private set; } = "Negative value is not allow!";
        [field: SerializeField] public string StatusTextNoZero { get; private set; } = "Zero is not allow!";
        [field: SerializeField] public string StatusTextTooHigh { get; private set; } = "Too High! Try lower the number down...";
        [field: SerializeField] public string StatusTextCantParse { get; private set; } = "Error! Wrong number format.";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Input Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onOkButtonClick;
        [SerializeField] private UnityEvent _onOkButtonCantClick;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public bool Validate(int inputNumber, out string validateStatus)
        {
            if (inputNumber < 0)
            {
                validateStatus = StatusTextNoNegative;
                return false;
            }
            else if (inputNumber == 0)
            {
                validateStatus = StatusTextNoZero;
                return false;
            }
            else if (inputNumber > _maximumPointInput)
            {
                validateStatus = StatusTextTooHigh;
                return false;
            }

            validateStatus = StatusTextValid;
            return true;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            _onCancelButtonClick?.Invoke();
        }

        public void OnOkButtonClick()
        {
            _onOkButtonClick?.Invoke();
        }

        public void OnOkButtonCantClick()
        {
            _onOkButtonCantClick?.Invoke();
        }
        #endregion
    }
}