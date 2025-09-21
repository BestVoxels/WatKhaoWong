using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.Localization;

namespace WatKhaoWong.Retreats
{
    public class UserInfo : Page
    {
        #region --Fields-- (Inspector)
        [Header("Accommodation Form Status Text")]
        [SerializeField] private LocalizedString _statusSucceeded;
        [SerializeField] private Color32 _statusSucceededColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Accommodation Form - Status Text")]
        [field: SerializeField] public LocalizedString StatusMustBeFilled { get; private set; }
        [field: SerializeField] public Color32 StatusMustBeFilledColor { get; private set; }

        [field: Header("Accommodation Form - Day Format on Button")]
        [field: SerializeField] public string DayFormat { get; private set; } = "d/M/yyyy";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Accommodation Form UI Event")]
        [SerializeField] private UnityEvent _onSetTimeButtonClick;
        [SerializeField] private UnityEvent _onValidateTextSucceeded;
        [SerializeField] private UnityEvent _onValidateTextFailed;
        [Space]
        [SerializeField] private UnityEvent _onPrintButtonClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<StayEntry> OnUploadedToServer;
        #endregion



        #region --Fields-- (In Class)
        private byte _activityIndex;
        private DateTime _dataTime;
        private EHasCar _hasCar;
        private string _plateNumber;

        private MyUserData _myUserData;
        private SavingWrapper _savingWrapper;
        private ServerTime _serverTime;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();

            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _serverTime = FindAnyObjectByType<ServerTime>();
            _statusText = FindAnyObjectByType<StatusText>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnSetTimeButtonClick()
        {
            _onSetTimeButtonClick?.Invoke();
        }

        public void OnValidateSucceeded(byte activityIndex, DateTime dateTime, EHasCar hasCar, string plateNumber)
        {
            _activityIndex = activityIndex;
            _dataTime = dateTime;
            _hasCar = hasCar;
            _plateNumber = plateNumber;

            _onValidateTextSucceeded?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateTextFailed?.Invoke();
        }

        public void OnPrintButtonClick()
        {
            _onPrintButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public async void UploadToServer()
        {
            DateTime nowDate = await _serverTime.Now();

            StayEntry stayEntry = new StayEntry()
            {
                UserId = FirebaseUtils.CurrentUserID,
                Activity = ((EActivityType)_activityIndex).ToString(),
                //StayInfo = new StayInfo()
                //{
                //    IsStaying = _dataTime.isStayingOvernight.ToString(),
                //    StartDate = _dataTime.startDate.ToGregorianString(),
                //    EndDate = _dataTime.endDate.ToGregorianString()
                //},
                Transportation = new Transportation()
                {
                    HasCar = _hasCar.ToString(),
                    CarPlateNumber = _plateNumber
                },
                StatusInfo = new StatusInfo()
                {
                    Status = EStayStatus.Pending.ToString(),
                    StatusUpdatedAt = nowDate.ToGregorianString()
                }
            };

            // Upload to Server -> 'Stay Requests' Category
            string keyId = await _savingWrapper.SaveDataWithKey(ECategoryNode.StayRequests, stayEntry);

            // Upload to Server -> 'User Themselves' Active Stay
            await _myUserData.SetDataActiveStay(keyId, EStayStatus.Pending);

            // Let Subscriber class use 'stayEntry' data to update UI
            OnUploadedToServer?.Invoke(stayEntry);

            _statusText.Show(_statusSucceeded.GetLocalizedString(), _statusSucceededColor);
        }

        // TODO Create PUblic method for other Page to set (2 methods) for 2 different animation styles. For them to set Previous Page.
        #endregion
    }
}