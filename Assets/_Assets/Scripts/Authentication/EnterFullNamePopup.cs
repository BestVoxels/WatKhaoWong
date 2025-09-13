using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Utils.Localization;

namespace WatKhaoWong.Authentication
{
    public class EnterFullNamePopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Signup Popup Status Text")]
        [SerializeField] private LocalizedString _statusSucceeded;
        [SerializeField] private Color32 _statusSucceededColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Signup Popup General Settings")]
        [field: SerializeField] public byte MinimumFirstNameLength { get; private set; } = 5;
        [field: SerializeField] public byte MinimumLastNameLength { get; private set; } = 5;
        [field: Space]
        [field: Header("Signup Popup Status Text")]
        [field: SerializeField] public LocalizedString StatusFirstNameTooShort { get; private set; }
        [field: SerializeField] public Color32 StatusFirstNameTooShortColor { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString StatusLastNameTooShort { get; private set; }
        [field: SerializeField] public Color32 StatusLastNameTooShortColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Signup Popup UI Event")]
        [SerializeField] private UnityEvent _onEnterSucceeded;
        [SerializeField] private UnityEvent _onValidateTextFailed;
        #endregion



        #region --Fields-- (In Class)
        private MyUserData _myUserData;
        private StatusText _statusText;
        private ServerTime _serverTime;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _myUserData = player.GetComponentInChildren<MyUserData>();
            _statusText = FindAnyObjectByType<StatusText>();
            _serverTime = FindAnyObjectByType<ServerTime>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public async void OnEnterSucceeded(string firstName, string lastName)
        {
            _myUserData.ForceSetFirstName(firstName);
            _myUserData.ForceSetLastName(lastName);
            _myUserData.ForceSetMemberSinceText(await _serverTime.Now());
            _myUserData.ForceSetRole(EUserRole.LayPeople);
            _myUserData.ForceSetTitle(EUserTitle.LayPeople.ToString());

            _statusText.Show(_statusSucceeded.GetLocalizedString(), _statusSucceededColor);
            _onEnterSucceeded?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateTextFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        #endregion
    }
}