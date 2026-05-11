using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Retreats
{
    public class SystemStayEntryData
    {
        public EStayStatus? status;
        public string keyId;
        public short rowIndex;
        public StayEntryRow.RowType rowType; // unlike 'status' from EStayStatus, useful when creating new Entry where we don't have 'status' data yet. Ex. Adder RowType
    }

    public class UserInputStayEntryData
    {
        public byte activityIndex;
        public SetTimeData setTimeData;
        public byte? buildingIndex;
        public string roomNumber;
        public EHasCar hasCar;
        public string plateNumber;
        public string notes;
        public byte? reputationIndex;
    }

    public class StayEntryRow : MonoBehaviour
    {
        public enum RowType
        {
            Adder,
            Pending,
            Current,
            Past
        }



        #region --Properties-- (Inspector)
        [field: Header("Stay Entry Form - Day Format on Button")]
        [field: SerializeField] public string DayFormat { get; private set; } = "d/M/yyyy";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Status Setter Event")]
        [SerializeField] private UnityEvent _onSetTimeButtonClick;
        [SerializeField] private UnityEvent _onConfirmAdded;
        [SerializeField] private UnityEvent _onConfirmEdited;
        [SerializeField] private UnityEvent _onValidateFailed;
        [Space]
        [SerializeField] private UnityEvent _onDeleteButtonClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<StayEntry, EStayStatus?> OnAddedToServer;
        public event Action<StayEntry, EStayStatus?> OnUpdatedOnServer;
        public event Action<StayEntry> OnDeletedFromServer;
        #endregion



        #region --Fields-- (In Class)
        private SystemStayEntryData _systemData;
        private UserInputStayEntryData _userInputData;

        private MyUserData _myUserData;
        private UserInfo _userInfo;
        private AccommodationSetTimePopup _setTimePopup;
        private SavingWrapper _savingWrapper;
        private ServerTime _serverTime;
        private Localizer _localizer;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _userInfo = player.GetComponentInChildren<UserInfo>();
            _setTimePopup = player.GetComponentInChildren<AccommodationSetTimePopup>();

            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _serverTime = FindAnyObjectByType<ServerTime>();
            _localizer = FindAnyObjectByType<Localizer>();
            _statusText = FindAnyObjectByType<StatusText>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// ONLY need for OnUpdated event is because StayEntryRowUI.cs needs to subscribe and resubcribe. Unlike OnAdded or OnDeleted events that UserInfoUI.cs can just subscribe one time.
        /// </summary>
        public void ClearOnUpdatedToServerSubscribers()
        {
            OnUpdatedOnServer = null;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnSetTimeButtonClick(bool allowPastDate)
        {
            _setTimePopup.SetAllowPastDate(allowPastDate);

            _onSetTimeButtonClick?.Invoke();
        }

        public void OnConfirmAdded(SystemStayEntryData systemData, UserInputStayEntryData userInputData)
        {
            _systemData = systemData;
            _userInputData = FilterUserInputStayEntryData(userInputData);

            _onConfirmAdded?.Invoke();
        }

        public void OnConfirmEdited(SystemStayEntryData systemData, UserInputStayEntryData userInputData)
        {
            _systemData = systemData;
            _userInputData = FilterUserInputStayEntryData(userInputData);

            _onConfirmEdited?.Invoke();
        }

        public void OnValidateFailed()
        {
            _onValidateFailed?.Invoke();
        }

        public void OnDeleteButtonClick(SystemStayEntryData systemData)
        {
            _systemData = systemData;

            _onDeleteButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool IsAdderRow() => _systemData.rowType == RowType.Adder;
        private bool IsPendingRow() => _systemData.rowType == RowType.Pending;
        private bool IsCurrentRow() => _systemData.rowType == RowType.Current;
        private bool IsPastRow() => _systemData.rowType == RowType.Past;

        private bool IsAdmin() => _myUserData.GetRole() == EUserRole.Admin;

        private UserInputStayEntryData FilterUserInputStayEntryData(UserInputStayEntryData inputData)
        {
            // BuildingName & RoomNumber
            byte? buildingIndex = inputData.buildingIndex;
            string roomNumber = inputData.roomNumber;
            if (IsPendingRow() || inputData.setTimeData.isStayingOvernight == EIsStaying.NotStaying)
            {
                buildingIndex = null;
                roomNumber = null;
            }

            // CarPlateNumber
            string plateNumber = inputData.plateNumber;
            if (inputData.hasCar == EHasCar.None)
            {
                plateNumber = null;
            }

            // NotesInfo & Reputation
            string notes = inputData.notes;
            byte? reputationIndex = inputData.reputationIndex;
            if (IsPendingRow())
            {
                notes = null;
                reputationIndex = null;
            }

            UserInputStayEntryData filteredData = new UserInputStayEntryData()
            {
                activityIndex = inputData.activityIndex,
                setTimeData = inputData.setTimeData,
                buildingIndex = buildingIndex,
                roomNumber = roomNumber,
                hasCar = inputData.hasCar,
                plateNumber = plateNumber,
                notes = notes,
                reputationIndex = reputationIndex
            };

            return filteredData;
        }

        private async Task<StayEntry> GetStayEntryFromUserInputData(EStayStatus eStayStatus)
        {
            DateTime nowDate = await _serverTime.Now();

            // BuildingName & RoomNumber
            RoomInfo roomInfo = null;
            if (_userInputData.buildingIndex != null)
            {
                roomInfo = new RoomInfo()
                {
                    BuildingName = ((EBuildingName)_userInputData.buildingIndex).ToString(),
                    RoomNumber = _userInputData.roomNumber
                };
            }

            // CarPlateNumber
            string carPlateNumber = null;
            if (_userInputData.plateNumber != null)
            {
                carPlateNumber = _userInputData.plateNumber;
            }

            // NotesInfo & Reputation
            NotesInfo notesInfo = null;
            string reputation = null;
            if (_userInputData.notes != null)
            {
                EReputation eReputation = (EReputation)_userInputData.reputationIndex;

                notesInfo = new NotesInfo()
                {
                    Text = _userInputData.notes,
                    Color = string.IsNullOrWhiteSpace(_userInputData.notes) ? "" : "#" + ColorUtility.ToHtmlStringRGB(_localizer.ColorizeReputation(eReputation.ToString()))
                };
                reputation = eReputation.ToString();
            }

            StayEntry stayEntry = new StayEntry()
            {
                UserId = FirebaseUtils.CurrentUserID, // TODO my or other userID 
                Activity = ((EActivityType)_userInputData.activityIndex).ToString(),
                StayInfo = new StayInfo()
                {
                    IsStaying = _userInputData.setTimeData.isStayingOvernight.ToString(),
                    StartDate = _userInputData.setTimeData.startDate.ToGregorianString(),
                    EndDate = _userInputData.setTimeData.endDate.ToGregorianString()
                },
                RoomInfo = roomInfo,
                Transportation = new Transportation()
                {
                    HasCar = _userInputData.hasCar.ToString(),
                    CarPlateNumber = carPlateNumber
                },
                StatusInfo = new StatusInfo()
                {
                    Status = eStayStatus.ToString(),
                    StatusUpdatedAt = nowDate.ToGregorianString() // Just update this right away because we can know either the whole thing got updated not just 'Status' is changed
                },
                NotesInfo = notesInfo,
                Reputation = reputation
            };

            return stayEntry;
        }

        private async Task<ActiveStay> GetActiveStay(string keyId, EStayStatus eStayStatus)
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


        
        #region --Methods-- (Subscriber) ~UnityEvent~
        /// <summary>
        /// No need to add 'Pending' entry.
        /// Allows Admin to add 'Scheduled' 'Active' 'Completed' entries.
        /// </summary>
        public async void AddToServer()
        {
            if (!IsAdmin()) return;

            DateTime nowDate = await _serverTime.Now();
            StayEntry stayEntry = null;
            ActiveStay activeStay = null;

            // Check for Time Period
            ETimePeriod? timePeriod = _userInputData.setTimeData.GetTimePeriod(nowDate);
            switch (timePeriod)
            {
                // --- Past ---
                case ETimePeriod.Past:

                    // -> ADD : under User's PastStay
                    stayEntry = await GetStayEntryFromUserInputData(EStayStatus.Completed);
                    await _savingWrapper.SaveDataWithKeyToMyUser(EParentNode.PastStay, stayEntry);

                    OnAddedToServer?.Invoke(stayEntry, EStayStatus.Completed);
                    break;

                // --- Active ---
                case ETimePeriod.Present:
                    // IF there is Current Entry can't add!
                    if (_myUserData.IsActiveStayExists())
                    {
                        _statusText.Show(_userInfo.StatusCantAddCurExists.GetLocalizedString(), _userInfo.StatusCantAddCurExistsColor);
                        return;
                    }

                    // IF no Current Entry
                    stayEntry = await GetStayEntryFromUserInputData(EStayStatus.Active);

                    // -> ADD : under ActiveStay's Category
                    string keyId = await _savingWrapper.SaveDataWithKey(ECategoryNode.ActiveStay, stayEntry);

                    // -> ADD : under User's ActiveStay
                    activeStay = await GetActiveStay(keyId, EStayStatus.Active);
                    await _myUserData.SetDataActiveStay(activeStay);

                    OnAddedToServer?.Invoke(stayEntry, EStayStatus.Active);
                    break;

                // --- Scheduled ---
                case ETimePeriod.Future:
                    // IF there is Current Entry can't add!
                    if (_myUserData.IsActiveStayExists())
                    {
                        _statusText.Show(_userInfo.StatusCantAddCurExists.GetLocalizedString(), _userInfo.StatusCantAddCurExistsColor);
                        return;
                    }

                    // IF no Current Entry
                    stayEntry = await GetStayEntryFromUserInputData(EStayStatus.Scheduled);

                    // -> ADD : under ScheduledStay's Category
                    keyId = await _savingWrapper.SaveDataWithKey(ECategoryNode.ScheduledStay, stayEntry);

                    // -> ADD : under User's ActiveStay
                    activeStay = await GetActiveStay(keyId, EStayStatus.Scheduled);
                    await _myUserData.SetDataActiveStay(activeStay);

                    OnAddedToServer?.Invoke(stayEntry, EStayStatus.Scheduled);
                    break;

                case null:
                    _statusText.Show("Error : User hasn't pick a date yet.", _userInfo.StatusMustBeFilledColor);
                    return;
            }

            _statusText.Show(_userInfo.StatusRecordAdded.GetLocalizedString(), _userInfo.StatusRecordAddedColor);
        }

        public async void UpdateOnServer()
        {
            if (!IsAdmin()) return;

            if (_systemData.status == null)
            {
                _statusText.Show("Error : Can't Update on server due to Entry's Status is null.", _userInfo.StatusMustBeFilledColor);
                return;
            }

            StayEntry stayEntry = await GetStayEntryFromUserInputData((EStayStatus)_systemData.status);

            // --- Pending / Scheduled / Active ---
            if (_systemData.status == EStayStatus.Pending || _systemData.status == EStayStatus.Active || _systemData.status == EStayStatus.Scheduled)
            {
                // -> UPDATE : under User's ActiveStay
                ActiveStay activeStay = await GetActiveStay(_systemData.keyId, (EStayStatus)_systemData.status);
                await _myUserData.SetDataActiveStay(activeStay);

                // -> UPDATE : under StayRequest/ScheduledStay/ActiveStay's Category
                switch (_systemData.status)
                {
                    case EStayStatus.Pending:
                        await _savingWrapper.SaveDataToExistingKey(ECategoryNode.StayRequests, _systemData.keyId, stayEntry);
                        break;

                    case EStayStatus.Active:
                        await _savingWrapper.SaveDataToExistingKey(ECategoryNode.ActiveStay, _systemData.keyId, stayEntry);
                        break;

                    case EStayStatus.Scheduled:
                        await _savingWrapper.SaveDataToExistingKey(ECategoryNode.ScheduledStay, _systemData.keyId, stayEntry);
                        break;
                }
            }

            // --- Past ---
            if (_systemData.status == EStayStatus.Completed || _systemData.status == EStayStatus.Rejected)
            {
                // -> UPDATE : under User's PastStay
                await _savingWrapper.SaveDataToExistingKeyToMyUser(EParentNode.PastStay, _systemData.keyId, stayEntry);
            }

            OnUpdatedOnServer?.Invoke(stayEntry, _systemData.status);

            _statusText.Show(_userInfo.StatusChangesSaved.GetLocalizedString(), _userInfo.StatusChangesSavedColor);
        }

        public void DeleteFromServer()
        {
            if (!IsAdmin()) return;

            if (_systemData.status == null)
            {
                _statusText.Show("Error : Can't Delete from server due to Entry's Status is null.", _userInfo.StatusMustBeFilledColor);
                return;
            }

            // --- Pending / Scheduled / Active ---
            if (_systemData.status == EStayStatus.Pending || _systemData.status == EStayStatus.Active || _systemData.status == EStayStatus.Scheduled)
            {
                // -> DELETE : under User's ActiveStay
                _savingWrapper.DeleteFromMyUser(EParentNode.ActiveStay);

                // -> DELETE : under StayRequest/ScheduledStay/ActiveStay's Category
                switch (_systemData.status)
                {
                    case EStayStatus.Pending:
                        _savingWrapper.DeleteStayRequestsEntry(_systemData.keyId);
                        break;

                    case EStayStatus.Scheduled:
                        _savingWrapper.DeleteScheduledStayEntry(_systemData.keyId);
                        break;

                    case EStayStatus.Active:
                        _savingWrapper.DeleteActiveStayEntry(_systemData.keyId);
                        break;
                }

                // Reset Data so it UI updates accordingly
                _myUserData.DeleteActiveStay();
                _myUserData.DeleteStayEntry();
            }

            // --- Past ---
            if (_systemData.status == EStayStatus.Completed || _systemData.status == EStayStatus.Rejected)
            {
                // -> DELETE : under User's PastStay
                _savingWrapper.DeleteFromMyUser(EParentNode.PastStay, _systemData.keyId);
            }

            OnDeletedFromServer?.Invoke(null);

            _statusText.Show(_userInfo.StatusRecordDeleted.GetLocalizedString(), _userInfo.StatusRecordDeletedColor);
        }
        #endregion
    }
}