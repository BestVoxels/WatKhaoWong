using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Retreats
{
    public class AccommodationForm : Page
    {
        #region --Fields-- (Inspector)
        [Header("Submit Info Status Text")]
        [SerializeField] private LocalizedString _statusSucceeded;
        [SerializeField] private Color32 _statusSucceededColor;
        #endregion



        //#region --Properties-- (Inspector)
        //[field: Header("Submit Info General Settings")]
        //[field: SerializeField] public byte MinimumPhoneNumberLength { get; private set; } = 9;
        //[field: SerializeField] public byte MaximumPhoneNumberLength { get; private set; } = 10;
        //[field: Space]
        //[field: Header("Submit Info Status Text")]
        //[field: SerializeField] public LocalizedString StatusInvalidPhoneNumber { get; private set; }
        //[field: SerializeField] public Color32 StatusInvalidPhoneNumberColor { get; private set; }
        //[field: Space]
        //[field: SerializeField] public LocalizedString StatusPhoneNumberTooShort { get; private set; }
        //[field: SerializeField] public Color32 StatusPhoneNumberTooShortColor { get; private set; }
        //[field: Space]
        //[field: SerializeField] public LocalizedString StatusPhoneNumberTooLong { get; private set; }
        //[field: SerializeField] public Color32 StatusPhoneNumberTooLongColor { get; private set; }
        //[field: Space]
        //[field: SerializeField] public LocalizedString StatusMustBeFilled { get; private set; }
        //[field: SerializeField] public Color32 StatusMustBeFilledColor { get; private set; }
        //#endregion



        #region --Events-- (UnityEvent)
        [Header("Accommodation Form UI Event")]
        [SerializeField] private UnityEvent _onSetTimeButtonClick;
        [SerializeField] private UnityEvent _onValidateTextSucceeded;
        [SerializeField] private UnityEvent _onValidateTextFailed;
        #endregion



        #region --Fields-- (In Class)
        //private MyUserData _myUserData;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            //_myUserData = player.GetComponentInChildren<MyUserData>();
            _statusText = FindAnyObjectByType<StatusText>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnSetTimeButtonClick()
        {
            _onSetTimeButtonClick?.Invoke();
        }

        public async void OnValidateSucceeded(string phoneNumber, string medical, string urgentPhoneNumber, string relation, string line, string fb, string ig, string tiktok)
        {
            _onValidateTextSucceeded?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateTextFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void UploadToServer()
        {
            //await _myUserData.SetDataGeneralInfo(phoneNumber, medical, urgentPhoneNumber, relation, line, fb, ig, tiktok);

            _statusText.Show(_statusSucceeded.GetLocalizedString(), _statusSucceededColor);
            print("Upload Data to Server!");
        }
        #endregion
    }
}