using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;

namespace WatKhaoWong.Prays
{
    public class InputPopup : Popup
    {
        #region --Fields-- (Inspector)
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Input Popup Status Text")]
        [field: SerializeField] public LocalizedString StatusTextDefault { get; private set; }
        [field: SerializeField] public LocalizedString StatusTextValid { get; private set; }
        [field: SerializeField] public LocalizedString StatusTextNoNegative { get; private set; }
        [field: SerializeField] public LocalizedString StatusTextNoZero { get; private set; }
        [field: SerializeField] public LocalizedString StatusTextTooHigh { get; private set; }
        [field: SerializeField] public LocalizedString StatusTextCantParse { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Input Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onOkButtonClick;
        [SerializeField] private UnityEvent _onOkButtonCantClick;
        #endregion



        #region --Fields-- (In Class)
        private MyUserData _myUserData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public bool Validate(int inputNumber, out string validateStatus)
        {
            if (inputNumber < 0)
            {
                validateStatus = StatusTextNoNegative.GetLocalizedString();
                return false;
            }
            else if (inputNumber == 0)
            {
                validateStatus = StatusTextNoZero.GetLocalizedString();
                return false;
            }
            else if (inputNumber > _myUserData.GetTMPointCap())
            {
                validateStatus = StatusTextTooHigh.GetLocalizedString();
                return false;
            }

            validateStatus = StatusTextValid.GetLocalizedString();
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