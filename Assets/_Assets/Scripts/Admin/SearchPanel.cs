using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.SceneManagement;
using Firebase.Database;
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
        public bool HasFilter { get; private set; } = false;
        #endregion



        #region --Fields-- (In Class)
        private byte _criteriaIndex;
        private string _searchData;

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
        public void StartSearchFilter(byte criteriaIndex, string searchData)
        {
            HasFilter = true;
            _criteriaIndex = criteriaIndex;
            _searchData = searchData;

            OnUIUpdated?.Invoke();
            _onSearchFilterStarted?.Invoke();
        }

        public void RemoveSearchFilter()
        {
            HasFilter = false;
            _criteriaIndex = 0;
            _searchData = null;
            
            OnUIUpdated?.Invoke();
            _onSearchFilterRemoved?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateFailed?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Filter Methods~
        public async Task<(StayEntry, string, DataSnapshot)> FilterApprovalRowData((StayEntry stayEntry, string key, DataSnapshot dataSnapshot) input)
        {
            IUserData userData = new OtherUserData(input.dataSnapshot);
            switch (_criteriaIndex)
            {
                case 0:
                    string name = userData.GetAllUserNameTextCombined(await userData.GetDataNationalIDInfo(), await userData.GetDataPassportInfo());

                    if (IsFullNameMatch(name))
                        return input;
                    break;
                case 1:
                    string number = userData.GetNationalIDAndPassportNumberCombined(await userData.GetDataNationalIDInfo(), await userData.GetDataPassportInfo());

                    if (IsNationalIDAndPassportNumberMatch(number))
                        return input;
                    break;
                case 2:
                    int age = await userData.GetAge(await userData.GetDataNationalIDInfo(), await userData.GetDataPassportInfo(), _serverTime);

                    if (IsAgeMatch(age))
                        return input;
                    break;
                case 3:
                    string plateNumber = await userData.GetPlateNumberFromActiveStayEntry();

                    if (IsPlateNumberMatch(plateNumber))
                        return input;
                    break;
                case 4:
                    string buildingName = await userData.GetBuildingNameFromActiveStayEntry(_localizer);

                    if (IsBuildingNameMatch(buildingName))
                        return input;
                    break;
                case 5:
                    string roomNumber = await userData.GetRoomNumberFromActiveStayEntry();

                    if (IsRoomNumberMatch(roomNumber))
                        return input;
                    break;
                case 6:
                    string accountStatus = userData.GetAccountStatusTextCombined(_localizer);

                    if (IsAccountStatusMatch(accountStatus))
                        return input;
                    break;
            }

            return (null, null, null);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool IsFullNameMatch(string inputName)
        {
            print($"InputName : {inputName} / SearchData from User : {_searchData}");
            if (string.IsNullOrWhiteSpace(inputName))
                return true;

            return inputName.Contains(_searchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsNationalIDAndPassportNumberMatch(string number)
        {
            print($"number : {number} / SearchData from User : {_searchData}");
            if (string.IsNullOrWhiteSpace(number))
                return false;

            return number.Contains(_searchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAgeMatch(int age)
        {
            print($"age : {age} / SearchData from User : {_searchData}");
            if (age <= -1)
                return false;

            return age.ToString().Contains(_searchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsPlateNumberMatch(string plateNumber)
        {
            print($"plateNumber : {plateNumber} / SearchData from User : {_searchData}");
            if (string.IsNullOrWhiteSpace(plateNumber))
                return false;

            return plateNumber.Contains(_searchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsBuildingNameMatch(string buildingName)
        {
            print($"buildingName : {buildingName} / SearchData from User : {_searchData}");
            if (string.IsNullOrWhiteSpace(buildingName))
                return false;

            return buildingName.Contains(_searchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsRoomNumberMatch(string roomNumber)
        {
            print($"roomNumber : {roomNumber} / SearchData from User : {_searchData}");
            if (string.IsNullOrWhiteSpace(roomNumber))
                return false;

            return roomNumber.Contains(_searchData, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAccountStatusMatch(string accountStatus)
        {
            print($"accountStatus : {accountStatus} / SearchData from User : {_searchData}");
            if (string.IsNullOrWhiteSpace(accountStatus))
                return false;

            return accountStatus.Contains(_searchData, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}