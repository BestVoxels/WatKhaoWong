using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Identities;
using WatKhaoWong.Retreats;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Utils.Core;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.Pool;

namespace WatKhaoWong.UI.Retreats
{
    public class StayEntryRowUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private StayEntryRow.RowType _rowType = StayEntryRow.RowType.Past;
        [Space]
        [SerializeField] private StayEntryInspector _ui;
        [Space]
        [SerializeField] private GameObject _modifierButtonsPanel;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _editButton;
        #endregion



        #region --Fields-- (In Class)
        private static GameObject s_curEditRowUI;
        private bool _isInitialized = false;
        private bool _isEditing = false;

        private UserInfo _userInfo;
        private StayEntryRow _stayEntryRow;
        private AccommodationSetTimePopup _setTimePopup;
        private InputFieldValidator _inputFieldValidator;
        private IObjectPool<StayEntryRowUI> _rowUIPool;
        private MyUserData _myUserData;
        private StatusText _statusText;

        // For -Setter-
        private byte _activityIndex;
        private byte _buildingIndex;
        private string _roomNumber;
        private string _plateNumber;
        private string _notes;
        private byte _reputationIndex;

        private string _resultKeyId;
        private short _rowIndex;
        private EStayStatus? _resultEStayStatus = null;

        // For -Viewer-
        private StayEntry _resultStayEntry;
        private Localizer _localizer;
        private AccommodationFormUI _accommodationFormUI;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            RefreshUI();

            SetupIndexResultFromDropdown();
        }

        private void OnDestroy()
        {
            if (!IsAdderRow())
            {
                _userInfo.OnModeChanged -= ShowModifierPanelUI;
            }

            UIRefresher.OnMeditationRetreatRefreshed -= RefreshUI;
            UIRefresher.OnLocalizeDynamicString -= ShowResultTextUI;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Pool~
        public void OnCreatedByPool(IObjectPool<StayEntryRowUI> rowUIPool)
        {
            _rowUIPool = rowUIPool;

            Initialize();
        }

        public void Release()
        {
            ClearData();
            _rowUIPool.Release(this);
        }

        public void Setup(StayEntry stayEntry, string keyId, short rowIndex = -1)
        {
            Initialize(); // Incase Awake() got disabled in the beginning.

            _resultStayEntry = stayEntry;
            _resultKeyId = keyId;
            _rowIndex = rowIndex;

            if (Enum.TryParse(stayEntry.StatusInfo.Status, true, out EStayStatus eStatus))
                _resultEStayStatus = eStatus;
            else
                _resultEStayStatus = null;

            RefreshUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UpdateEventToCurrentOne()
        {
            if (IsThisUIEqualsCurEditRowUI()) return;

            s_curEditRowUI = gameObject;

            // Clear Old Subscribers first (from other Entry Row UIs)
            _setTimePopup.ClearOnValidatedSubscribers();
            _setTimePopup.ClearDateData();
            if (!IsAdderRow())
                _stayEntryRow.ClearOnUpdatedToServerSubscribers();

            // then Subscribe again.
            _setTimePopup.OnValidated += (DateTime start, DateTime end) => { UpdateTextOnButtonSetTime(start, end); ShowHideBuildingAndRoomUI(IsStayingOvernightFromUI()); };
            if (!IsAdderRow())
            {
                // Put it here because it needs to Resubscribe Again & Again
                _stayEntryRow.OnUpdatedOnServer += (stayEntry, stayStatus) => { RefreshUIWithServerData(stayEntry, stayStatus); _accommodationFormUI.RefreshUIWithServerData(stayEntry, stayStatus); };
            }
        }

        private bool IsThisUIEqualsCurEditRowUI() => gameObject.Equals(s_curEditRowUI);

        private void RefreshUI()
        {
            ShowResultTextUI();

            UpdateShowHideUI();
        }

        private void SetupIndexResultFromDropdown()
        {
            _activityIndex = (byte)_ui.activityDropdown.index;

            if (!IsPendingRow())
            {
                _buildingIndex = (byte)_ui.buildingDropdown.index;
                _reputationIndex = (byte)_ui.reputationDropdown.index;
            }
        }

        private bool IsAdderRow() => _rowType == StayEntryRow.RowType.Adder;

        private bool IsPendingRow() => _rowType == StayEntryRow.RowType.Pending;

        private bool IsCurrentRow() => _rowType == StayEntryRow.RowType.Current;

        private bool IsPastRow() => _rowType == StayEntryRow.RowType.Past;

        private bool IsAdmin() => _myUserData.GetRole() == EUserRole.Admin;

        private bool GetAllowPastDate()
        {
            if (IsAdderRow() || IsPastRow())
                return true;

            if (IsPendingRow() || IsCurrentRow())
                return false;

            return false;
        }

        private void ClearData()
        {
            if (IsAdderRow()) return;

            // Activity
            _ui.activityResultText.text = _userInfo.NoDataText.GetLocalizedString();

            // Stay Duration
            _ui.setTimeResultText.text = _userInfo.NoDataText.GetLocalizedString();

            if (!IsPendingRow())
            {
                // Building & Room
                _ui.buildingResultText.text = _userInfo.NoDataText.GetLocalizedString();
                _ui.roomNumberResultText.text = _userInfo.NoDataText.GetLocalizedString();

                // Aditional Notes
                _ui.notesResultText.text = _userInfo.NoDataText.GetLocalizedString();
                _ui.notesResultText.color = _userInfo.DefaultNotesTextColor;

                // Reputation
                _ui.reputationResultText.text = _userInfo.NoDataText.GetLocalizedString();
            }

            // HasCar & PlateNumber
            _ui.hasCarResultText.text = _userInfo.NoDataText.GetLocalizedString();
            _ui.plateNumberResultText.text = _userInfo.NoDataText.GetLocalizedString();
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            
            GameObject player = GameObject.FindWithTag("Player");
            _userInfo = player.GetComponentInChildren<UserInfo>();
            _stayEntryRow = player.GetComponentInChildren<StayEntryRow>();
            _setTimePopup = player.GetComponentInChildren<AccommodationSetTimePopup>();
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _localizer = FindAnyObjectByType<Localizer>(FindObjectsInactive.Include);
            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();
            _accommodationFormUI = FindAnyObjectByType<AccommodationFormUI>(FindObjectsInactive.Include);

            if (!IsAdderRow())
            {
                _editButton.onClick.AddListener(() => { _isEditing = !_isEditing; ToggleEditingUI(); });
                _deleteButton.onClick.AddListener(DeleteButton);

                _userInfo.OnModeChanged += ShowModifierPanelUI; // put here so it continues to works event when on PersonalInfo, GenearlInfo tabs
            }

            _ui.activityDropdown.onValueChanged.AddListener(ActivityDropdown);
            _ui.setTimeButton.onClick.AddListener(() => { UpdateEventToCurrentOne(); SetTime(); });
            _ui.hasCarSwitch.onValueChanged.AddListener(ShowHidePlateNumber);
            _ui.plateNumberInputField.onEndEdit.AddListener(inputText => IsPlateNumberValidated());
            // No need for Additional Notes

            if (!IsPendingRow())
            {
                _ui.buildingDropdown.onValueChanged.AddListener(BuildingDropdown);
                _ui.roomNumberInputField.onEndEdit.AddListener(inputText => IsRoomNumberValidated());
                _ui.reputationDropdown.onValueChanged.AddListener(ReputationDropdown);
            }

            _ui.confirmButton.onClick.AddListener(() => { UpdateEventToCurrentOne(); Confirm(); });

            UIRefresher.OnMeditationRetreatRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
            UIRefresher.OnLocalizeDynamicString += ShowResultTextUI;

            _isInitialized = true;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Viewer~
        private void ShowResultTextUI()
        {
            if (_resultStayEntry == null || IsAdderRow()) return;

            // Activity
            _ui.activityResultText.text = _localizer.LocalizeActivityType(_resultStayEntry.Activity);

            // Stay Duration
            _resultStayEntry.StayInfo.StartDate.TryParseGregorian(out DateTime startDate);
            _resultStayEntry.StayInfo.EndDate.TryParseGregorian(out DateTime endDate);
            _ui.setTimeResultText.text = _setTimePopup.FormatButtonString(startDate, endDate, _stayEntryRow.DayFormat);

            if (!IsPendingRow())
            {
                // Building & Room
                if (_resultStayEntry.RoomInfo != null)
                {
                    _ui.buildingResultText.text = _localizer.LocalizeBuildingName(_resultStayEntry.RoomInfo.BuildingName);
                    _ui.roomNumberResultText.text = _resultStayEntry.RoomInfo.RoomNumber;
                }
                else
                {
                    _ui.buildingResultText.text = _userInfo.NoDataText.GetLocalizedString();
                    _ui.roomNumberResultText.text = _userInfo.NoDataText.GetLocalizedString();
                }

                // Aditional Notes
                if (_resultStayEntry.NotesInfo != null)
                {
                    _ui.notesResultText.text = _resultStayEntry.NotesInfo.Text;
                    ColorUtility.TryParseHtmlString(_resultStayEntry.NotesInfo.Color, out Color notesColor);
                    _ui.notesResultText.color = notesColor;
                }

                // Reputation
                if (_resultStayEntry.Reputation != null)
                    _ui.reputationResultText.text = _localizer.LocalizeReputation(_resultStayEntry.Reputation);
            }

            // HasCar & PlateNumber
            _ui.hasCarResultText.text = _localizer.LocalizeHasCar(_resultStayEntry.Transportation.HasCar);
            if (IsHasCarFromResult())
                _ui.plateNumberResultText.text = _resultStayEntry.Transportation.CarPlateNumber;
            else
                _ui.plateNumberResultText.text = _userInfo.NoDataText.GetLocalizedString();
        }

        private void UpdateShowHideUI()
        {
            if (_isEditing || IsAdderRow())
            {
                ShowHidePlateNumber(IsHasCarFromUI());
                ShowHideBuildingAndRoomUI(true); // This always has to reset anyways since popup UI gets reset
            }
            else
            {
                ShowHidePlateNumber(IsHasCarFromResult());
                ShowHideBuildingAndRoomUI(IsStayingOvernightFromResult());
            }
        }

        private bool IsHasCarFromResult()
        {
            if (_resultStayEntry == null)
                return true;

            return ((EHasCar)Enum.Parse(typeof(EHasCar), _resultStayEntry.Transportation.HasCar)) == EHasCar.Has;
        }

        private bool IsStayingOvernightFromResult()
        {
            if (_resultStayEntry == null)
                return true;

            return ((EIsStaying)Enum.Parse(typeof(EIsStaying), _resultStayEntry.StayInfo.IsStaying)) == EIsStaying.Staying;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Setter~
        private bool Validate()
        {
            bool status = true;

            if (!IsSetTimeValidated())
            {
                RevertTextOnButtonSetTime();
                status = false;
            }
            if (!IsPendingRow() && IsStayingOvernightFromUI() && !IsRoomNumberValidated()) status = false;
            if (IsHasCarFromUI() && !IsPlateNumberValidated()) status = false;

            return status;
        }

        private void RevertTextOnButtonSetTime()
        {
            LocalizeStringEvent localizeEvent = _ui.setTimeButton.GetComponentInChildren<LocalizeStringEvent>();
            LocalizedString localizedString = localizeEvent.StringReference;

            localizedString.RefreshString();
        }

        private bool IsSetTimeValidated() => _setTimePopup.ValidateSetTimePopup(GetAllowPastDate());

        private bool IsStayingOvernightFromUI() => _setTimePopup.GetIsStayingOvernight() == EIsStaying.Staying;

        private bool IsHasCarFromUI() => _ui.hasCarSwitch.isOn;
        #endregion



        #region --Methods-- (Subscriber)
        private void ShowModifierPanelUI(EViewEditMode mode)
        {
            if (IsAdderRow()) return;
            if (!IsAdmin()) return;
            
            bool isEditing = mode == EViewEditMode.Edit;

            _modifierButtonsPanel.SetActive(isEditing);

            // Disabled IsEditing and its UI back when EViewEditMode is 'View'
            if (mode == EViewEditMode.View)
            {
                _isEditing = false;
                ToggleEditingUI();
            }
        }

        private void ToggleEditingUI()
        {
            UpdateShowHideUI();

            _ui.activityDropdown.gameObject.SetActive(_isEditing);
            _ui.setTimeButton.gameObject.SetActive(_isEditing);
            _ui.hasCarSwitch.gameObject.SetActive(_isEditing);
            _ui.plateNumberInputField.gameObject.SetActive(_isEditing);

            _ui.activityResultText.gameObject.SetActive(!_isEditing);
            _ui.setTimeResultText.gameObject.SetActive(!_isEditing);
            _ui.hasCarResultText.gameObject.SetActive(!_isEditing);
            _ui.plateNumberResultText.gameObject.SetActive(!_isEditing);

            if (!IsPendingRow())
            {
                _ui.buildingDropdown.gameObject.SetActive(_isEditing);
                _ui.roomNumberInputField.gameObject.SetActive(_isEditing);
                _ui.notesInputField.gameObject.SetActive(_isEditing);
                _ui.reputationDropdown.gameObject.SetActive(_isEditing);

                _ui.buildingResultText.gameObject.SetActive(!_isEditing);
                _ui.roomNumberResultText.gameObject.SetActive(!_isEditing);
                _ui.notesResultText.gameObject.SetActive(!_isEditing);
                _ui.reputationResultText.gameObject.SetActive(!_isEditing);
            }

            _ui.confirmPanelGameObject.gameObject.SetActive(_isEditing);
        }

        private void ActivityDropdown(int index)
        {
            _activityIndex = (byte)index;
        }

        private void SetTime()
        {
            _stayEntryRow.OnSetTimeButtonClick(GetAllowPastDate());
        }

        private void UpdateTextOnButtonSetTime(DateTime startDate, DateTime endDate)
        {
            TMP_Text buttonText = _ui.setTimeButton.GetComponentInChildren<TMP_Text>();

            buttonText.text = _setTimePopup.FormatButtonString(startDate, endDate, _stayEntryRow.DayFormat);
        }

        private void BuildingDropdown(int index)
        {
            _buildingIndex = (byte)index;
        }

        private bool IsRoomNumberValidated() => _inputFieldValidator.ValidateNotNull(
            _ui.roomNumberInputField.text, _ui.roomNumberInputFieldStatus, out _roomNumber,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private void ShowHidePlateNumber(bool hasCar) => _ui.plateNumberMenuGameObject.SetActive(hasCar);

        private bool IsPlateNumberValidated() => _inputFieldValidator.ValidateNotNull(
            _ui.plateNumberInputField.text, _ui.plateNumberInputFieldStatus, out _plateNumber,
            (_userInfo.StatusMustBeFilled.GetLocalizedString(), _userInfo.StatusMustBeFilledColor));

        private void ReputationDropdown(int index)
        {
            _reputationIndex = (byte)index;
        }

        private void ShowHideBuildingAndRoomUI(bool isStaying)
        {
            if (IsPendingRow()) return;
            
            _ui.buildingMenuGameObject.SetActive(isStaying);
            _ui.roomNumberMenuGameObject.SetActive(isStaying);
        }

        private void RefreshUIWithServerData(StayEntry stayEntry, EStayStatus? stayStatus)
        {
            // No need to use 'stayStatus' to filter like on 'AccommodationFormUI.cs'

            _resultStayEntry = stayEntry;

            RefreshUI();
        }

        private void Confirm()
        {
            if (Validate())
            {
                SystemStayEntryData systemData = new SystemStayEntryData()
                {
                    status = _resultEStayStatus,
                    keyId = _resultKeyId,
                    rowIndex = _rowIndex,
                    rowType = _rowType
                };

                EHasCar hasCarData = IsHasCarFromUI() ? EHasCar.Has : EHasCar.None;

                if (!IsPendingRow())
                    _notes = _ui.notesInputField.text;

                UserInputStayEntryData userInputData = new UserInputStayEntryData()
                {
                    activityIndex = _activityIndex,
                    setTimeData = _setTimePopup.GetData(),
                    buildingIndex = _buildingIndex,
                    roomNumber = _roomNumber,
                    hasCar = hasCarData,
                    plateNumber = _plateNumber,
                    notes = _notes,
                    reputationIndex = _reputationIndex
                };

                if (IsAdderRow())
                    _stayEntryRow.OnConfirmAdded(systemData, userInputData);
                else
                    _stayEntryRow.OnConfirmEdited(systemData, userInputData);
            }
            else
            {
                _stayEntryRow.OnValidateFailed();
            }
        }

        private void DeleteButton()
        {
            if (IsAdderRow()) return;
            if (!IsAdmin()) return;

            SystemStayEntryData systemData = new SystemStayEntryData()
            {
                status = _resultEStayStatus,
                keyId = _resultKeyId,
                rowIndex = _rowIndex,
                rowType = _rowType
            };

            _stayEntryRow.OnDeleteButtonClick(systemData);
        }
        #endregion
    }
}