using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using WatKhaoWong.CoreItems;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;
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
        [SerializeField] private int _defaultLevel;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnMyUserDataUpdated;
        public event Action<int> OnTodayTMPointsAdded;
        public event Action<int> OnChallengeTMPointsAdded;
        #endregion



        #region --Fields-- (In Class)
        private readonly Data _data = new Data();

        private Challenge _challenge;
        private SavingWrapper _savingWrapper;
        private ServerTime _serverTime;
        #endregion



        #region --Properties-- (Auto)
        public bool IsLoadingFromServer { get; private set; } = true;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _challenge = GameObject.FindWithTag("Player").GetComponentInChildren<Challenge>();
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

        private async void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
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

        public void ForceSetRole(EUserRole role)
        {
            _data.Role = role;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.Role, _data.Role.ToString());
            OnMyUserDataUpdated?.Invoke();
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

        public void AddTotalTMPoints(int input)
        {
            _data.TotalTMPoints += input;

            _savingWrapper.Save(ECategoryNode.Users, EValueNode.TotalTMPoint, _data.TotalTMPoints);
            OnMyUserDataUpdated?.Invoke();
        }

        public async void AddTodayTMPoints(int input)
        {
            await ResetTMPointsDaily();

            if (input <= 0) return;

            AssignTodayUploadTime();

            _data.TodayTMPoints += input;
            _savingWrapper.Save(ECategoryNode.Users, EValueNode.TodayTMPoint, _data.TodayTMPoints);
            OnMyUserDataUpdated?.Invoke();

            OnTodayTMPointsAdded?.Invoke(_data.TodayTMPoints);
        }

        public async void AddChallengeTMPointsText(int input)
        {
            await ResetTMPointsAfterChallengeEnd();

            if (input <= 0 || !await _challenge.CanLiveNow()) return;

            AssignChallengeUploadTime();

            _data.ChallengeTMPoints += input;
            _savingWrapper.Save(ECategoryNode.Users, EValueNode.ChallengeTMPoint, _data.ChallengeTMPoints);
            OnMyUserDataUpdated?.Invoke();

            OnChallengeTMPointsAdded?.Invoke(_data.ChallengeTMPoints);
        }

        public void AddTotalWonTMChallenge(int input)
        {
            _data.TotalChallengeTMWon += input;

            _savingWrapper.Save(ECategoryNode.Users, EValueNode.ChallengeTMWon, _data.TotalChallengeTMWon);
            OnMyUserDataUpdated?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async Task ResetTMPointsDaily()
        {
            if (_data.FirstUploadTimeOfDayTM == default) return;

            DateTime nowDate = await _serverTime.Now();
            if (_data.FirstUploadTimeOfDayTM.Date != nowDate.Date && _data.TodayTMPoints > 0)
            {
                _data.TodayTMPoints = 0;

                _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.TodayTMPoint, 0);
                OnMyUserDataUpdated?.Invoke();
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

        private async void LoadSave()
        {
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

            IsLoadingFromServer = false;

            OnMyUserDataUpdated?.Invoke();
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
        #endregion



        #region --Methods-- (Interface)
        public void UpdateProfileIcon(ProfileIconInspector oldUI, ProfileIconItem newIcon, float multiplierRatioForDecorator)
        {
            _data.UpdateProfileIcon(oldUI, newIcon, multiplierRatioForDecorator);
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