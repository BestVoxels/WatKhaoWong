using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Prays
{
    public class ChallengePopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Challenge Popup Status Text")]
        [field: SerializeField] public string StatusMissingLengthTG { get; private set; } = "Please choose any option from 'How Long?' section";
        [field: SerializeField] public Color32 StatusMissingLengthTGColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusMissingNowOrLaterTG { get; private set; } = "Please choose any option from 'Start Challenge Now or Later?' section";
        [field: SerializeField] public Color32 StatusMissingNowOrLaterTGColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusMissingDelayDurationTG { get; private set; } = "Please choose any option from 'Delay Duration' section";
        [field: SerializeField] public Color32 StatusMissingDelayDurationTGColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusUploadSucceed { get; private set; } = "Uploaded! Challenge is now set!";
        [field: SerializeField] public Color32 StatusUploadSucceedColor { get; private set; }
        [field: Space]
        [field: SerializeField] public string StatusUploadFail { get; private set; } = "Failed! Couldn't Upload to Server somehow!";
        [field: SerializeField] public Color32 StatusUploadFailColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Challenge Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonCantClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            Debug.LogWarning("Click \"Cancel\" Button! on Popup");

            _onCancelButtonClick?.Invoke();
        }

        public void OnConfirmButtonClick()
        {
            Debug.LogWarning("Click \"Confirm\" Button! on Popup");

            _onConfirmButtonClick?.Invoke();
        }

        public void OnConfirmButtonCantClick()
        {
            Debug.LogWarning("CANT Click \"Confirm\" Button! on Popup");

            _onConfirmButtonCantClick?.Invoke();
        }
        #endregion
    }
}