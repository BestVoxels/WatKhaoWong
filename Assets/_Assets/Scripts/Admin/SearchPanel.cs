using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using System.Threading.Tasks;
using WatKhaoWong.Utils.Localization;

namespace WatKhaoWong.Admin
{
    public class SearchPanel : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        [field: Header("Status Text")]
        [field: SerializeField] public LocalizedString StatusMustBeFilled { get; private set; }
        [field: SerializeField] public Color32 StatusMustBeFilledColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Status Setter Event")]
        [SerializeField] private UnityEvent _onSearchFilterStarted;
        [SerializeField] private UnityEvent _onSearchFilterRemoved;
        [SerializeField] private UnityEvent _onValidateFailed;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnUIUpdated;
        #endregion



        #region --Properties-- (Auto)
        private ESearchPanelLocation CurrentLocation { get; set; }
        #endregion



        #region --Properties-- (With Backing Fields)
        // Doing this way to PREVENT Null Error from accessing Records. This way it will gets value when it needs, no need to initialize on Start().
        private RecordCollection Records
        {
            get
            {
                if (_records == null)
                    _records = new();

                return _records;
            }

            set => _records = value;
        }
        #endregion



        #region --Fields-- (In Class)
        private RecordCollection _records;

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



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void StartSearchFilter(ESearchPanelLocation location, byte criteriaIndex, string searchData)
        {
            CurrentLocation = location;
            Records[CurrentLocation].HasFilter = true;
            Records[CurrentLocation].CriteriaIndex = criteriaIndex;
            Records[CurrentLocation].SearchData = searchData;

            OnUIUpdated?.Invoke();
            _onSearchFilterStarted?.Invoke();
        }

        public void RemoveSearchFilter(ESearchPanelLocation location)
        {
            CurrentLocation = location;
            Records[CurrentLocation].RemoveSearchFilter();
            
            OnUIUpdated?.Invoke();
            _onSearchFilterRemoved?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Filter Methods~
        public void SetLocation(ESearchPanelLocation location) => CurrentLocation = location;

        public bool HasFilter() => Records[CurrentLocation].HasFilter;

        public IUserData FilterRowData(IUserData userData)
        {
            if (IsFilterPassed(userData))
                return userData;
            else
                return null;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool IsFilterPassed(IUserData userData)
        {
            switch (Records[CurrentLocation].CriteriaIndex)
            {
                case 0:
                    string name = userData.GetAllUserNameTextCombined(userData.GetDataNationalIDInfoNoLoad(), userData.GetDataPassportInfoNoLoad());

                    if (IsFullNameMatch(name))
                        return true;
                    break;
                case 1:
                    string number = userData.GetNationalIDAndPassportNumberCombined(userData.GetDataNationalIDInfoNoLoad(), userData.GetDataPassportInfoNoLoad());

                    if (IsNationalIDAndPassportNumberMatch(number))
                        return true;
                    break;
                case 2:
                    int age = userData.GetAge(userData.GetDataNationalIDInfoNoLoad(), userData.GetDataPassportInfoNoLoad(), _serverTime);

                    if (IsAgeMatch(age))
                        return true;
                    break;
                case 3:
                    string plateNumber = userData.GetPlateNumberFromActiveStayEntry();

                    if (IsPlateNumberMatch(plateNumber))
                        return true;
                    break;
                case 4:
                    string buildingName = userData.GetBuildingNameFromActiveStayEntry(_localizer);

                    if (IsBuildingNameMatch(buildingName))
                        return true;
                    break;
                case 5:
                    string roomNumber = userData.GetRoomNumberFromActiveStayEntry();

                    if (IsRoomNumberMatch(roomNumber))
                        return true;
                    break;
                case 6:
                    string accountStatus = userData.GetAccountStatusTextCombined(_localizer);

                    if (IsAccountStatusMatch(accountStatus))
                        return true;
                    break;
            }

            return false;
        }
        private bool IsFullNameMatch(string inputName)
        {
            // print($"InputName : {inputName} / SearchData from User : {Records[CurrentLocation].SearchData}");
            if (string.IsNullOrWhiteSpace(inputName))
                return true;

            return inputName.Contains(Records[CurrentLocation].SearchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsNationalIDAndPassportNumberMatch(string number)
        {
            // print($"number : {number} / SearchData from User : {Records[CurrentLocation].SearchData}");
            if (string.IsNullOrWhiteSpace(number))
                return false;

            return number.Contains(Records[CurrentLocation].SearchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAgeMatch(int age)
        {
            // print($"age : {age} / SearchData from User : {Records[CurrentLocation].SearchData}");
            if (age <= -1)
                return false;

            return age.ToString().Contains(Records[CurrentLocation].SearchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsPlateNumberMatch(string plateNumber)
        {
            // print($"plateNumber : {plateNumber} / SearchData from User : {Records[CurrentLocation].SearchData}");
            if (string.IsNullOrWhiteSpace(plateNumber))
                return false;

            return plateNumber.Contains(Records[CurrentLocation].SearchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsBuildingNameMatch(string buildingName)
        {
            // print($"buildingName : {buildingName} / SearchData from User : {Records[CurrentLocation].SearchData}");
            if (string.IsNullOrWhiteSpace(buildingName))
                return false;

            return buildingName.Contains(Records[CurrentLocation].SearchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsRoomNumberMatch(string roomNumber)
        {
            // print($"roomNumber : {roomNumber} / SearchData from User : {Records[CurrentLocation].SearchData}");
            if (string.IsNullOrWhiteSpace(roomNumber))
                return false;

            return roomNumber.Contains(Records[CurrentLocation].SearchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAccountStatusMatch(string accountStatus)
        {
            // print($"accountStatus : {accountStatus} / SearchData from User : {Records[CurrentLocation].SearchData}");
            if (string.IsNullOrWhiteSpace(accountStatus))
                return false;

            return accountStatus.Contains(Records[CurrentLocation].SearchData, StringComparison.OrdinalIgnoreCase);
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        private class Record
        {
            public bool HasFilter = false;
            public byte CriteriaIndex = 0;
            public string SearchData = null;

            public void RemoveSearchFilter()
            {
                HasFilter = false;
                CriteriaIndex = 0;
                SearchData = null;
            }
        }

        private class RecordCollection
        {
            // Collection
            private readonly Record[] _records = new Record[2];

            // Indexer
            public Record this[ESearchPanelLocation location]
            {
                get => _records[GetInt(location)];
            }

            // Constructor
            public RecordCollection()
            {
                for (byte i = 0; i < _records.Length; i++)
                    _records[i] = new Record();
            }

            // Methods
            private int GetInt(ESearchPanelLocation location)
            {
                return location switch
                {
                    ESearchPanelLocation.SearchBoard => 0,
                    ESearchPanelLocation.ApprovalBoard => 1,
                    _ => -1
                };
            }
        }
        #endregion
    }
}