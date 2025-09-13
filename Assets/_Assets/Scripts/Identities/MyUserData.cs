using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using WatKhaoWong.CoreItems;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.Utils.Conditions;
using WatKhaoWong.Challenges;
using Firebase.Auth;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Identities
{
    /// <summary>
    /// --NOTE--
    /// Can't use Inheritance because 'MyUserData.cs' MUST inherit from Monobehavior BUT 'OtherUserData.cs' MUST NOT inherit from Monobehavior.
    /// SO have to use Composition for 'MyUserData.cs' & 'OtherUserData.cs' instead of Inheritance (which is to avoid over Composition anyways).
    /// ALSO 'MyUserData.cs' & 'OtherUserData.cs' implement interface 'IUserData.cs' to use Polymorphism concept, so both classes can be under 'IUserData.cs'.
    /// --------
    /// </summary>
    public class MyUserData : MonoBehaviour, IUserData, IConditionEvaluator
    {
        #region --Fields-- (Inspector)
        [Header("Account Stuffs")]
        [SerializeField] private LocalizedString _defaultFirstName;
        [SerializeField] private LocalizedString _defaultLastName;
        [SerializeField] private LocalizedString _defaultMemberSince;
        [SerializeField] private LocalizedString _loading;
        [SerializeField] private ProfileIconItem _defaultProfileIcon;
        [SerializeField] private EAccountStatus _defaultAccountStatus;
        [SerializeField] private bool _defaultIsCustomTMPointCap = false;
        [SerializeField] private int _defaultLevel;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnMyUserDataUpdated;
        public event Action OnRoleUpdated;
        public event Action<int> OnTodayTMPointsAdded;
        public event Action<int> OnChallengeTMPointsAdded;
        #endregion



        #region --Fields-- (In Class)
        private readonly Data _data = new Data();

        private PointUploadEvents _pointUploadEvents;
        private Challenge _challenge;
        private SavingWrapper _savingWrapper;
        private ServerTime _serverTime;
        #endregion



        #region --Fields-- (Constant)
        private const string KeySentCapRequest = "KeySentCapRequest";
        #endregion



        #region --Properties-- (Auto)
        public bool IsLoadingFromServer { get; private set; } = true;

        // IMPORTANT : SetTMPointCap() on PointCapSetter.cs will use 'LoadCompletionSource' to check and wait
        // until MyUserData.cs' LoadSave() is fully loaded because they use some value here to check in their condition.
        // If don't do this, we can't guarantee MyUserData.cs' LoadSave() will loaded prior and value they use to check
        // will be wrong
        public TaskCompletionSource<bool> LoadCompletionSource { get; } = new TaskCompletionSource<bool>();
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _pointUploadEvents = player.GetComponentInChildren<PointUploadEvents>();
            _challenge = player.GetComponentInChildren<Challenge>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _serverTime = FindAnyObjectByType<ServerTime>();
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged; // This will trigger on Start() too so don't have to call LoadSave() on Start()
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }

        private async void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                await ResetTMPointsDaily();

                await ResetTMPointsAfterChallengeEnd();
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Setter~
        public void ForceSetFirstName(string input)
        {
            _data.FirstName = input;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.FirstName, _data.FirstName);
            OnMyUserDataUpdated?.Invoke();
        }

        public void ForceSetLastName(string input)
        {
            _data.LastName = input;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.LastName, _data.LastName);
            OnMyUserDataUpdated?.Invoke();
        }

        public void ForceSetMemberSinceText(DateTime input)
        {
            _data.MemberSince = input;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.MemberSince, _data.MemberSince.ToGregorianString());
            OnMyUserDataUpdated?.Invoke();
        }

        public void SetProfileIcon(ProfileIconItem input)
        {
            _data.ProfileIcon = input;

            _savingWrapper.Save(ECategoryNode.Users, EValueNode.ProfileIconID, _data.ProfileIcon.ItemID);
            OnMyUserDataUpdated?.Invoke();
        }

        public async Task SetAccountStatusDefault()
        {
            await SetDataAccountStatus(updateCheckinAt: true, _defaultAccountStatus);
        }

        public async Task SetDataAccountStatus(bool updateCheckinAt, EAccountStatus? eStatus = null, DateTime? banEndDate = null, string notesText = null, string notesColor = null)
        {
            DateTime nowDate = await _serverTime.Now();

            AccountStatus oldStatus = (_data.AccountStatus == null) ? new AccountStatus() : _data.AccountStatus;
            string lastCheckinAtText = updateCheckinAt ? nowDate.ToGregorianString() : oldStatus.LastCheckinAt;
            string banEndDateText = (banEndDate == null) ? oldStatus.BanEndDate : banEndDate.ToGregorianString();

            StatusInfo oldStatusInfo = (oldStatus.StatusInfo == null) ? new StatusInfo() : oldStatus.StatusInfo;
            string statusText = (eStatus == null) ? oldStatusInfo.Status : eStatus.ToString();
            string statusUpdatedAtText = (eStatus == null || (oldStatusInfo.Status == eStatus.ToString() && oldStatusInfo.StatusUpdatedAt != null)) ? oldStatusInfo.StatusUpdatedAt : nowDate.ToGregorianString();

            NotesInfo oldNotesInfo = (oldStatus.NotesInfo == null) ? new NotesInfo() : oldStatus.NotesInfo;
            string notesInfoText = (notesText == null) ? oldNotesInfo.Text : notesText;
            string notesInfoColor = (notesColor == null) ? oldNotesInfo.Color : notesColor;

            _data.AccountStatus = new AccountStatus()
            {
                LastCheckinAt = lastCheckinAtText,
                StatusInfo = new StatusInfo()
                {
                    Status = statusText,
                    StatusUpdatedAt = statusUpdatedAtText
                },
                BanEndDate = banEndDateText,
                NotesInfo = new NotesInfo()
                {
                    Text = notesInfoText,
                    Color = notesInfoColor
                }
            };

            await _savingWrapper.SaveDataToMyUser(EParentNode.AccountStatus, _data.AccountStatus);
            OnMyUserDataUpdated?.Invoke();
        }

        public async Task SetPartialAccountStatus(string pathUnderAccountStatus, string value)
        {
            AccountStatus accountStatus = _data.AccountStatus;

            // _data.AccountStatus have to be updated for UI to changed "_data.AccountStatus = ..."

            await _savingWrapper.SaveToMyUser(EParentNode.AccountStatus, pathUnderAccountStatus, value);
            OnMyUserDataUpdated?.Invoke();
        }

        public void ForceSetRole(EUserRole role)
        {
            _data.Role = role;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.Role, _data.Role.ToString());
            OnMyUserDataUpdated?.Invoke();
            OnRoleUpdated?.Invoke();
        }

        public void ForceSetTitle(string title)
        {
            _data.Title = title;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.Title, _data.Title);
            OnMyUserDataUpdated?.Invoke();
        }

        public void SetLevelText(int input)
        {
            _data.Level = input;

            _savingWrapper.Save(ECategoryNode.Users, EValueNode.Level, _data.Level);
            OnMyUserDataUpdated?.Invoke();
        }

        public async void AddTMPoints(int input, bool capRoundPoints = true)
        {
            await AddChallengeTMPoints(input, capRoundPoints);
            AddTotalTMPoints(input, capRoundPoints);

            await AddTodayTMPoints(input, capRoundPoints); // Call this last, since it changes 'TodayTMPoints' value and it will messup how 'CanAddPoints()' is calculate.
        }

        public void AddTotalWonTMChallenge(int input)
        {
            if (input < 0) return;

            _data.TotalChallengeTMWon += input;

            _savingWrapper.Save(ECategoryNode.Users, EValueNode.ChallengeTMWon, _data.TotalChallengeTMWon);
            OnMyUserDataUpdated?.Invoke();
        }

        public bool IncrementTMPointCapRequest()
        {
            if (PlayerPrefsX.GetBool(KeySentCapRequest, false))
                return false;

            _data.TMPointCapRequest += 1;
            PlayerPrefsX.SetBool(KeySentCapRequest, true);

            _savingWrapper.Save(ECategoryNode.Users, EValueNode.TMPointCapRequest, _data.TMPointCapRequest);
            return true;
        }

        public void ForceSetTMPointCapRound(int input)
        {
            if (input <= 0) return;

            _data.TMPointCapRound = input;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.TMPointCapRound, _data.TMPointCapRound);
        }

        public void ForceSetTMPointCap(int input)
        {
            if (input < 0) return;

            _data.TMPointCap = input;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.TMPointCap, _data.TMPointCap);
        }

        public void ForceSetIsCustomTMPointCap(bool input)
        {
            _data.IsCustomTMPointCap = input;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.IsCustomTMPointCap, _data.IsCustomTMPointCap);
        }

        public async void SetTempleGuideConfirmedToTrue()
        {
            if (_data.TempleGuideConfirmed == true) return; // Only Allow set for the First Time

            _data.TempleGuideConfirmed = true;
            _savingWrapper.Save(ECategoryNode.Users, EValueNode.TempleGuideConfirmed, true);

            DateTime nowDate = await _serverTime.Now();
            _savingWrapper.Save(ECategoryNode.Users, EValueNode.TempleGuideConfirmedAt, nowDate.ToGregorianString());
        }

        public async Task SetDataGeneralInfo(string phoneNumber, string medical, string urgentPhoneNumber, string relation, string line, string fb, string ig, string tiktok)
        {
            _data.GeneralInfo = new GeneralInfo()
            {
                PhoneNumber = phoneNumber,
                MedicalCondition = medical,
                EmergencyContact = new EmergencyContact()
                {
                    PhoneNumber = urgentPhoneNumber,
                    Relation = relation
                },
                SocialAccounts = new SocialAccounts()
                {
                    Line = line,
                    Facebook = fb,
                    Instagram = ig,
                    Tiktok = tiktok
                }
            };

            await _savingWrapper.SaveDataToMyUser(EParentNode.GeneralInfo, _data.GeneralInfo);
        }

        public async Task SetDataActiveStay(string keyId, EStayStatus status)
        {
            DateTime nowDate = await _serverTime.Now();

            _data.ActiveStay = new ActiveStay()
            {
                KeyId = keyId,
                StatusInfo = new StatusInfo()
                {
                    Status = status.ToString(),
                    StatusUpdatedAt = nowDate.ToGregorianString()
                }
            };

            await _savingWrapper.SaveDataToMyUser(EParentNode.ActiveStay, _data.ActiveStay);
        }
        #endregion



        #region --Methods-- (Interface) ~Getter~
        public string GetUserNameText()
        {
            if (!FirebaseUtils.IsAuthenticated())
            {
                _data.FirstName = _defaultFirstName.GetLocalizedString();
                _data.LastName = _defaultLastName.GetLocalizedString();
            }
            else if (FirebaseUtils.IsAuthenticated() && IsLoadingFromServer == true)
            {
                return _loading.GetLocalizedString();
            }

            return _data.GetUserNameText();
        }

        public string GetMemberSinceText()
        {
            if (!FirebaseUtils.IsAuthenticated())
                return $"{_defaultMemberSince.GetLocalizedString()}";
            else if (FirebaseUtils.IsAuthenticated() && IsLoadingFromServer == true)
                return "...";

            return _data.GetMemberSinceText();
        }

        public ProfileIconItem GetProfileIcon()
        {
            if (_data.ProfileIcon == null)
                _data.ProfileIcon = _defaultProfileIcon;

            return _data.GetProfileIcon();
        }

        public AccountStatus GetAccountStatus()
        {
            if (!IsAccountStatusExists())
            {
                AccountStatus accountStatus = new AccountStatus()
                {
                    StatusInfo = new StatusInfo()
                    {
                        Status = _defaultAccountStatus.ToString(),
                    }
                };

                _data.AccountStatus = accountStatus;
            }

            return _data.GetAccountStatus();
        }

        public EUserRole GetRole() => _data.GetRole();

        public string GetTitleText()
        {
            if (FirebaseUtils.IsAuthenticated() && IsLoadingFromServer == true)
                return "...";

            return _data.GetTitleText();
        }

        public string GetLevelText()
        {
            if (_data.Level == 0)
                _data.Level = _defaultLevel;

            return _data.GetLevelText();
        }

        public string GetTotalTMPointsText() => _data.GetTotalTMPointsText();

        public string GetTodayTMPointsText() => _data.GetTodayTMPointsText();

        public string GetChallengeTMPointsText() => _data.GetChallengeTMPointsText();

        public string GetTotalChallengeTMWonText() => _data.GetTotalChallengeTMWonText();

        public int GetTotalTMPoints() => _data.TotalTMPoints;

        public int GetTodayTMPoints() => _data.TodayTMPoints;

        public int GetChallengeTMPoints() => _data.ChallengeTMPoints;

        public int GetTotalChallengeTMWon() => _data.TotalChallengeTMWon;

        public int GetTMPointCapRequest() => _data.TMPointCapRequest;

        public int GetTMPointCap() => _data.TMPointCap;

        public int GetTMPointCapRound() => _data.TMPointCapRound;

        public bool GetIsCustomTMPointCap() => _data.IsCustomTMPointCap;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Setter~
        public bool GetTempleGuideConfirmed() => _data.TempleGuideConfirmed;

        public async Task GetDataGeneralInfo()
        {
            // TODO create private methods to check like 'GetMyEntryFromStayRequests'
            if (_data.GeneralInfo == null)
            {
                _data.GeneralInfo = await _savingWrapper.LoadDataFromMyUser<GeneralInfo>(EParentNode.GeneralInfo);
            }

            // '_data.GeneralInfo.SocialAccounts.Line == null' is the way to check if there is no value

        }

        public async Task GetDataActiveStay()
        {
            // TODO create private methods to check like 'GetMyEntryFromStayRequests'
            if (_data.ActiveStay == null)
            {
                _data.ActiveStay = await _savingWrapper.LoadDataFromMyUser<ActiveStay>(EParentNode.ActiveStay);
            }
        }

        public async Task<StayEntry> GetMyEntryFromStayRequests()
        {
            if (!IsStayEntryExists())
            {
                await LoadMyEntryFromStayRequests();

                if (!IsStayEntryExists()) return null; // Incase can't find my 'StayEntry' under 'StayRequests' Category
            }

            return _data.StayEntry;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public async Task LoadMyEntryFromStayRequests()
        {
            _data.StayEntry = await _savingWrapper.LoadMyEntryFromStayRequests();
        }

        public bool IsStayEntryExists() => !(_data.StayEntry == null);

        public async Task LoadAccountStatus()
        {
            _data.AccountStatus = await _savingWrapper.LoadDataFromMyUser<AccountStatus>(EParentNode.AccountStatus);
        }

        public bool IsAccountStatusExists() => !(_data.AccountStatus == null);
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void AddTotalTMPoints(int input, bool capRoundPoints = true)
        {
            bool didCap = false;

            if (input < 0)
            {
                _pointUploadEvents.OnTMPointsUploadFailedNegative?.Invoke();
                return;
            }
            if (input == 0)
            {
                _pointUploadEvents.OnTMPointsUploadFailedZero?.Invoke();
                return;
            }

            if (capRoundPoints) CapRoundPoints(ref input, out didCap);

            if (!CanAddDailyPoints(input, out int availableToAdd))
            {
                _pointUploadEvents.OnTMPointsUploadFailedCap?.Invoke();
                return;
            }


            if (input == availableToAdd)
                _pointUploadEvents.OnTMPointsUploadSucceeded?.Invoke(availableToAdd);
            else if (input != availableToAdd)
                _pointUploadEvents.OnTMPointsUploadSucceededPartial?.Invoke(availableToAdd);
            else if (didCap)
                _pointUploadEvents.OnTMPointsUploadSucceededCapRound?.Invoke(input);


            _data.TotalTMPoints += availableToAdd;

            _savingWrapper.Save(ECategoryNode.Users, EValueNode.TotalTMPoint, _data.TotalTMPoints);
            OnMyUserDataUpdated?.Invoke();
        }

        private async Task AddTodayTMPoints(int input, bool capRoundPoints = true)
        {
            await ResetTMPointsDaily();

            if (input <= 0) return;
            if (capRoundPoints) CapRoundPoints(ref input, out bool didCap);
            if (!CanAddDailyPoints(input, out int availableToAdd)) return;

            AssignTodayUploadTime();

            _data.TodayTMPoints += availableToAdd;
            _savingWrapper.Save(ECategoryNode.Users, EValueNode.TodayTMPoint, _data.TodayTMPoints);
            OnMyUserDataUpdated?.Invoke();

            OnTodayTMPointsAdded?.Invoke(_data.TodayTMPoints);
        }

        private async Task AddChallengeTMPoints(int input, bool capRoundPoints = true)
        {
            await ResetTMPointsAfterChallengeEnd();

            if (input <= 0 || !await _challenge.CanLiveNow()) return;
            if (capRoundPoints) CapRoundPoints(ref input, out bool didCap);
            if (!CanAddDailyPoints(input, out int availableToAdd)) return;

            AssignChallengeUploadTime();

            _data.ChallengeTMPoints += availableToAdd;
            _savingWrapper.Save(ECategoryNode.Users, EValueNode.ChallengeTMPoint, _data.ChallengeTMPoints);
            OnMyUserDataUpdated?.Invoke();

            OnChallengeTMPointsAdded?.Invoke(_data.ChallengeTMPoints);
        }

        private async Task ResetTMPointsDaily()
        {
            if (_data.FirstUploadTimeOfDayTM == default) return;

            DateTime nowDate = await _serverTime.Now();
            if (_data.FirstUploadTimeOfDayTM.Date != nowDate.Date && _data.TodayTMPoints > 0)
            {
                _data.TodayTMPoints = 0;

                _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.TodayTMPoint, 0);
                OnMyUserDataUpdated?.Invoke();

                PlayerPrefsX.SetBool(KeySentCapRequest, false);
            }
        }

        private async void AssignTodayUploadTime()
        {
            if (_data.TodayTMPoints == 0)
            {
                DateTime nowDate = await _serverTime.Now();

                _data.FirstUploadTimeOfDayTM = nowDate;
                _savingWrapper.Save(ECategoryNode.Users, EValueNode.FirstUploadTimeOfDayTM, nowDate.ToGregorianString());
            }
        }

        private async Task ResetTMPointsAfterChallengeEnd()
        {
            if (_data.FirstUploadTimeOfChallengeTM == default) return;

            if ((!_challenge.CanLive(_data.FirstUploadTimeOfChallengeTM) || !await _challenge.CanLiveNow()) && _data.ChallengeTMPoints > 0)
            {
                _data.ChallengeTMPoints = 0;
                _data.FirstUploadTimeOfChallengeTM = default;

                _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.ChallengeTMPoint, 0);
                _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.FirstUploadTimeOfChallengeTM, _data.FirstUploadTimeOfChallengeTM.ToGregorianString());

                OnMyUserDataUpdated?.Invoke();
            }
        }

        private async void AssignChallengeUploadTime()
        {
            if (_data.ChallengeTMPoints == 0)
            {
                DateTime nowDate = await _serverTime.Now();

                _data.FirstUploadTimeOfChallengeTM = nowDate;
                _savingWrapper.Save(ECategoryNode.Users, EValueNode.FirstUploadTimeOfChallengeTM, nowDate.ToGregorianString());
            }
        }

        private void SetRoleToGuestIfNoAuthen()
        {
            if (!FirebaseUtils.IsAuthenticated())
                _data.Role = EUserRole.Guest;
        }

        private void CapRoundPoints(ref int inputPoints, out bool didCap)
        {
            didCap = inputPoints > GetTMPointCapRound();

            inputPoints = Mathf.Clamp(inputPoints, 0, GetTMPointCapRound());
        }

        private bool CanAddDailyPoints(int inputPoints, out int availableToAdd)
        {
            int remaining = GetTMPointCap() - GetTodayTMPoints();

            availableToAdd = Mathf.Clamp(inputPoints, 0, remaining);

            return availableToAdd > 0;
        }

        private async void LoadSave()
        {
            if (!FirebaseUtils.IsAuthenticated()) return;

            bool isChallengeSaveLoaded = await _challenge.LoadCompletionSource.Task;

            if (isChallengeSaveLoaded == false)
            {
                Debug.LogError("Could not continue LoadSave() on MyUserData.cs because Challenge.cs LoadSave() is not completed.");
                return;
            }

            IsLoadingFromServer = true;

            var data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.FirstName);
            if (data != null)
                _data.FirstName = data.Value.ToString();

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.LastName);
            if (data != null)
                _data.LastName = data.Value.ToString();

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.MemberSince);
            if (data != null)
            {
                if (data.Value.ToString().TryParseGregorian(out DateTime result))
                    _data.MemberSince = result;
            }

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.ProfileIconID);
            if (data != null)
            {
                string id = data.Value.ToString();
                _data.ProfileIcon = BaseItem.GetFromID(id) as ProfileIconItem;
            }

            _data.AccountStatus = await _savingWrapper.LoadDataFromMyUser<AccountStatus>(EParentNode.AccountStatus);


            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.Role);
            if (data != null)
            {
                string roleString = data.Value.ToString();
                _data.Role = (EUserRole)Enum.Parse(typeof(EUserRole), roleString);
            }

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.Title);
            if (data != null)
                _data.Title = data.Value.ToString();

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.Level);
            if (data != null)
                _data.Level = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.TotalTMPoint);
            if (data != null)
                _data.TotalTMPoints = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.TodayTMPoint);
            if (data != null)
                _data.TodayTMPoints = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.ChallengeTMPoint);
            if (data != null)
                _data.ChallengeTMPoints = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.ChallengeTMWon);
            if (data != null)
                _data.TotalChallengeTMWon = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.TMPointCapRequest);
            if (data != null)
                _data.TMPointCapRequest = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.TMPointCap);
            if (data != null)
                _data.TMPointCap = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.IsCustomTMPointCap);
            if (data != null)
            {
                _data.IsCustomTMPointCap = bool.Parse(data.Value.ToString());
            }
            else if (FirebaseUtils.IsAuthenticated())
            {
                ForceSetIsCustomTMPointCap(_defaultIsCustomTMPointCap);
            }

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.TempleGuideConfirmed);
            if (data != null)
                _data.TempleGuideConfirmed = bool.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.FirstUploadTimeOfDayTM);
            if (data != null)
            {
                if (data.Value.ToString().TryParseGregorian(out DateTime result))
                    _data.FirstUploadTimeOfDayTM = result;

                await ResetTMPointsDaily();
            }

            data = await _savingWrapper.Load(ECategoryNode.Users, EValueNode.FirstUploadTimeOfChallengeTM);
            if (data != null)
            {
                if (data.Value.ToString().TryParseGregorian(out DateTime result))
                    _data.FirstUploadTimeOfChallengeTM = result;

                await ResetTMPointsAfterChallengeEnd();
            }

            LoadCompletionSource.TrySetResult(true);
            IsLoadingFromServer = false;
            
            OnMyUserDataUpdated?.Invoke();
        }
        #endregion



        #region --Methods-- (Interface)
        public void UpdateProfileIcon(ProfileIconInspector oldUI, ProfileIconItem newIcon, float multiplierRatioForDecorator)
        {
            _data.UpdateProfileIcon(oldUI, newIcon, multiplierRatioForDecorator);
        }

        public void UpdateAccountStatus(AccountStatusInspector oldStatus, AccountStatus newStatus, Localizer localizer)
        {
            _data.UpdateAccountStatus(oldStatus, newStatus, localizer);
        }

        bool? IConditionEvaluator.Evaluate(EConditionType conditionType, EConditionValue[] conditionValues)
        {
            switch (conditionType)
            {
                case EConditionType.IsRoleEquals:
                    byte stringStartIndex = (byte)EConditionType.IsRoleEquals;
                    string enumString = conditionValues[0].ToString()[stringStartIndex..];

                    if (!Enum.TryParse(enumString, true, out EUserRole result))
                        return false;

                    return GetRole() == result;

                case EConditionType.IsAuthenticated:
                    return FirebaseUtils.IsAuthenticated();

                case EConditionType.HasAllTimeTMPoint:
                    return _data.TotalTMPoints > 0;

                case EConditionType.HasTodayTMPoint:
                    return _data.TodayTMPoints > 0;

                case EConditionType.HasChallengeTMPoint:
                    return _data.ChallengeTMPoints > 0;

                case EConditionType.HasChallengeTMWon:
                    return _data.TotalChallengeTMWon > 0;

                //// ---DEBUGGER PURPOSE--- search for 'EConditionType.cs | MyUserData.cs | ShowHideUIByCondition.cs'
                //case EConditionType.True:
                //    return true;

                //case EConditionType.False:
                //    return false;
            }

            return null;
        }
        #endregion



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        /// </summary>
        private void HandleStateChanged(object obj, EventArgs args)
        {
            LoadSave(); // So Don't have to call on Awake()

            SetRoleToGuestIfNoAuthen(); // So Don't have to call on Awake()
        }
        #endregion
    }
}