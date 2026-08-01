using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.Localization;

namespace WatKhaoWong.Admin
{
    public class AccommodationApproval : Page
    {
        #region --Properties-- (Inspector)
        [field: Header("User Info - Status Text")]
        [field: SerializeField] public LocalizedString StatusAccepted { get; private set; }
        [field: SerializeField] public Color32 StatusAcceptedColor { get; private set; }
        [field: SerializeField] public LocalizedString StatusRejected { get; private set; }
        [field: SerializeField] public Color32 StatusRejectedColor { get; private set; }

        [field: Header("Reject Settings")]
        [field: SerializeField] public string PreTextForRejectedNotes { get; private set; } = "REJECTED ";

        [field: Header("Accept Settings")]
        [field: Range(0, 100)]
        [field: SerializeField] public byte TargetToOffer { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        //[Header("AccommodationApproval UI Event")]
        #endregion



        #region --Fields-- (Inspector)
        private ServerTime _serverTime;
        private Localizer _localizer;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _serverTime = FindAnyObjectByType<ServerTime>();
            _localizer = FindAnyObjectByType<Localizer>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public async Task<StayEntry> GetStayEntry(StayEntry stayEntry, EStayStatus eStayStatus, string notes)
        {
            stayEntry.NotesInfo = new NotesInfo()
            {
                Text = notes,
                Color = "#" + ColorUtility.ToHtmlStringRGB(_localizer.ColorizeReputation(EReputation.Normal.ToString()))
            };

            return await GetStayEntry(stayEntry, eStayStatus);
        }
        
        public async Task<StayEntry> GetStayEntry(StayEntry stayEntry, EStayStatus eStayStatus, byte buildingIndex, string roomNumber)
        {
            // IF roomNumber is Null that means no need to create RoomInfo.
            if (roomNumber != null)
            {
                stayEntry.RoomInfo = new RoomInfo()
                {
                    BuildingName = ((EBuildingName)buildingIndex).ToString(),
                    RoomNumber = roomNumber
                };
            }

            return await GetStayEntry(stayEntry, eStayStatus);
        }

        public async Task<StayEntry> GetStayEntry(StayEntry stayEntry, EStayStatus eStayStatus)
        {
            DateTime nowDate = await _serverTime.Now();

            stayEntry.StatusInfo = new StatusInfo()
            {
                Status = eStayStatus.ToString(),
                StatusUpdatedAt = nowDate.ToGregorianString()
            };
            stayEntry.Reputation = EReputation.Normal.ToString(); // Set Default Reputation to Normal

            return stayEntry;
        }

        public async Task<ActiveStay> GetActiveStay(string keyId, EStayStatus eStayStatus)
        {
            DateTime nowDate = await _serverTime.Now();

            ActiveStay activeStay = new ActiveStay()
            {
                KeyId = keyId,
                StatusInfo = new StatusInfo()
                {
                    Status = eStayStatus.ToString(),
                    StatusUpdatedAt = nowDate.ToGregorianString()
                }
            };

            return activeStay;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        #endregion
    }
}