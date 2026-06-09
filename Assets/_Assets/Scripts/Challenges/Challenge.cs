using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Utils.Conditions;
using WatKhaoWong.SceneManagement;
using Firebase.Auth;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Challenges
{
    public class Challenge : MonoBehaviour, IConditionEvaluator
    {
        private enum RefreshUI
        {
            Yes,
            No
        }



        #region --Fields-- (Inspector)
        [Header("Challenge Stuffs - Text")]
        [SerializeField] private LocalizedString _dayText;
        [SerializeField] private LocalizedString _sText;
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Challenge Event")]
        [SerializeField] private UnityEvent _onChallengeCreationButtonClick;
        [SerializeField] private UnityEvent _onChallengePendingButtonClick;
        [Space]
        [SerializeField] private UnityEvent _onCountDownBannerClick;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnDataUpdated;
        #endregion



        #region --Fields-- (In Class)
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _duration;
        private EChallengeStatus _status;

        private SavingWrapper _savingWrapper;
        private ServerTime _serverTime;
        #endregion



        #region --Properties-- (Auto)
        // IMPORTANT : LoadSave() on MyUserData.cs & Leaderboard.cs will use 'LoadCompletionSource' to check and wait
        // until Challenge.cs' LoadSave() is fully loaded because they use some value here to check in their condition.
        // If don't do this, we can't guarantee Challenge.cs' LoadSave() will loaded prior and value they use to check
        // will be wrong and trigger 'DeleteChallengeLeaderboard', 'DeleteChallengePoints' and more disaster...
        public TaskCompletionSource<bool> LoadCompletionSource { get; } = new TaskCompletionSource<bool>();
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
            _serverTime = FindAnyObjectByType<ServerTime>();
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged; // This will trigger on Start() too so don't have to call LoadSave() on Start()
        }

        //// ---DEBUGGER PURPOSE---
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //        _status = EChallengeStatus.None;

        //    if (Input.GetKeyDown(KeyCode.Alpha2))
        //        _status = EChallengeStatus.Pending;

        //    if (Input.GetKeyDown(KeyCode.Alpha3))
        //        _status = EChallengeStatus.Live;

        //    if (Input.GetKeyDown(KeyCode.KeypadEnter))
        //        print(_status);
        //}

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                // IMPORTANT : MUST 'LoadSave()' when open from background, To Get Latest Data from Server. ONLY for 'LoadSave()' that use 'Share Categories', eg LeaderboardStats, ServerStats, RemoteConfig.
                // Why? check 'Leadeboard.cs'
                LoadSave(RefreshUI.No);
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public async void CreatePendingChallenge(DateTime startDate, DateTime endDate, TimeSpan duration)
        {
            if (startDate == default && endDate == default) return; // Guard check Input because StartDate & EndDate will have default value from Server.
            if (_status == EChallengeStatus.Pending) return;

            SetStartDate(startDate);
            SetEndDate(endDate);
            SetDuration(duration);
            SetStatus(EChallengeStatus.Pending);

            await AutoLiveChallenge(); // when Admin choose StartDate is Today Date.

            OnDataUpdated?.Invoke();
        }

        public void DeletePendingChallenge()
        {
            if (StartAndEndDateAreDefault()) return; // Guard check incase StartDate & EndDate didn't Load Data from Server. (Avoid Overwrite)
            if (_status != EChallengeStatus.Pending) return;

            SetStartDate(default);
            SetEndDate(default);
            SetDuration(default);
            SetStatus(EChallengeStatus.None);

            OnDataUpdated?.Invoke();
        }

        public async Task<bool> CanLiveNow()
        {
            if (StartAndEndDateAreDefault()) return false; // Guard check incase StartDate & EndDate didn't Load Data from Server. (Avoid Overwrite)
            DateTime nowDate = await _serverTime.Now();

            return nowDate >= _startDate && nowDate <= _endDate;
        }

        public bool CanLive(DateTime compareTime) => compareTime >= _startDate && compareTime <= _endDate;

        public async Task<int> GetChallengeEndDaysLeft()
        {
            if (_status != EChallengeStatus.Live || !await CanLiveNow()) return -1; // Challenge is not yet started

            TimeSpan daysLeft = _endDate.Date - (await _serverTime.Now()).Date;

            return (int)Math.Round(daysLeft.TotalDays, MidpointRounding.AwayFromZero);
        }

        public async Task<int> GetChallengeStartDaysLeft()
        {
            if (_status == EChallengeStatus.Live || await CanLiveNow()) return -1; // Challenge is already started

            TimeSpan daysLeft = _startDate.Date - (await _serverTime.Now()).Date;

            return (int)Math.Round(daysLeft.TotalDays, MidpointRounding.AwayFromZero);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Getter~
        public DateTime GetStartDate() => _startDate;

        public DateTime GetEndDate() => _endDate;

        public TimeSpan GetDuration() => _duration;

        public EChallengeStatus GetStatus() => _status;

        public string GetID() => $"({_startDate.ToGregorianString("d-M-yyyy HH-mm-ss")}) -> ({_endDate.ToGregorianString("d-M-yyyy HH-mm-ss")})";
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Displaying~
        public string FormatDateString(DateTime date, string format) => (date == default) ? "-" : $"<u>{date.ToGregorianString(format)}</u>";

        public string FormatDurationString(TimeSpan duration)
        {
            if (duration == default)
                return "-";

            int totalDays = (int)Math.Round(duration.TotalDays, MidpointRounding.AwayFromZero);

            return $"<u>{totalDays} {_dayText.GetLocalizedString()}{S(totalDays)}</u>";
        }

        public string DaysString(int days)
        {
            if (days < 0)
                return $"??? {_dayText.GetLocalizedString()}";

            return $"{days} {_dayText.GetLocalizedString()}{S(days)}";
        }

        public string S(int input) => input > 1 ? _sText.GetLocalizedString() : "";
        #endregion



        #region --Methods-- (Custom PUBLIC) ~UI Buttons~
        public void OnChallengeCreationButtonClick()
        {
            _onChallengeCreationButtonClick?.Invoke();
        }

        public void OnChallengePendingButtonClick()
        {
            _onChallengePendingButtonClick?.Invoke();
        }

        public void OnCountDownBannerClick()
        {
            _onCountDownBannerClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Setter~
        private void SetStartDate(DateTime input)
        {
            _startDate = input;

            _savingWrapper.Save(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMStartDate, _startDate.ToGregorianString());
        }

        private void SetEndDate(DateTime input)
        {
            _endDate = input;

            _savingWrapper.Save(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMEndDate, _endDate.ToGregorianString());
        }

        private void SetDuration(TimeSpan input)
        {
            _duration = input;

            _savingWrapper.Save(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMDuration, _duration.ToString());
        }

        private void SetStatus(EChallengeStatus input)
        {
            _status = input;

            _savingWrapper.Save(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMStatus, _status.ToString());
        }

        // Need ForceSave() so that it can save on Start for AutoLive() or AutoEnd().
        private void SetStartDateForceSave(DateTime input)
        {
            _startDate = input;

            _savingWrapper.ForceSave(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMStartDate, _startDate.ToGregorianString());
        }

        private void SetEndDateForceSave(DateTime input)
        {
            _endDate = input;

            _savingWrapper.ForceSave(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMEndDate, _endDate.ToGregorianString());
        }

        private void SetDurationForceSave(TimeSpan input)
        {
            _duration = input;

            _savingWrapper.ForceSave(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMDuration, _duration.ToString());
        }

        private void SetStatusForceSave(EChallengeStatus input)
        {
            _status = input;

            _savingWrapper.ForceSave(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMStatus, _status.ToString());
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async Task AutoLiveChallenge()
        {
            if (_status == EChallengeStatus.Pending && await CanLiveNow())
                LiveChallenge();
        }

        private async Task AutoEndChallenge()
        {
            if (_status == EChallengeStatus.Live && !await CanLiveNow())
                EndChallenge();
        }

        private void LiveChallenge()
        {
            if (StartAndEndDateAreDefault()) return; // Guard check incase StartDate & EndDate didn't Load Data from Server. (Avoid Overwrite)
            if (_status == EChallengeStatus.Live) return;

            SetStatusForceSave(EChallengeStatus.Live);

            OnDataUpdated?.Invoke();
        }

        private void EndChallenge()
        {
            if (StartAndEndDateAreDefault()) return; // Guard check incase StartDate & EndDate didn't Load Data from Server. (Avoid Overwrite)
            if (_status == EChallengeStatus.None) return;

            SetStartDateForceSave(default);
            SetEndDateForceSave(default);
            SetDurationForceSave(default);
            SetStatusForceSave(EChallengeStatus.None);

            OnDataUpdated?.Invoke();
        }

        private bool StartAndEndDateAreDefault() => _startDate == default && _endDate == default;

        private async void LoadSave(RefreshUI refreshUI)
        {
            var data = await _savingWrapper.Load(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMStartDate);
            if (data != null)
            {
                if (data.Value.ToString().TryParseGregorian(out DateTime result))                
                    _startDate = result;
            }

            data = await _savingWrapper.Load(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMEndDate);
            if (data != null)
            {
                if (data.Value.ToString().TryParseGregorian(out DateTime result))
                    _endDate = result;
            }

            data = await _savingWrapper.Load(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMDuration);
            if (data != null)
            {
                if (TimeSpan.TryParse(data.Value.ToString(), out TimeSpan result))
                    _duration = result;
            }

            data = await _savingWrapper.Load(ECategoryNode.LeaderboardStats, EValueNode.ChallengeTMStatus);
            if (data != null)
            {
                string statusString = data.Value.ToString();
                _status = (EChallengeStatus)Enum.Parse(typeof(EChallengeStatus), statusString);
            }

            LoadCompletionSource.TrySetResult(true);

            // Wait a little before change state so that MyUserData.cs or Leaderboard.cs can use loaded data first. If AutoLive() or AutoEnd() run the loaded data might get changed.
            await Task.Delay(1500); // 1.5 sec

            await AutoLiveChallenge();
            await AutoEndChallenge();

            if (refreshUI == RefreshUI.Yes)
            {
                OnDataUpdated?.Invoke();
            }
        }
        #endregion



        #region --Methods-- (Interface)
        bool? IConditionEvaluator.Evaluate(EConditionType conditionType, EConditionValue[] conditionValues)
        {
            switch (conditionType)
            {
                case EConditionType.IsChallengeStatusEquals:
                    byte stringStartIndex = (byte)EConditionType.IsChallengeStatusEquals;
                    string enumString = conditionValues[0].ToString()[stringStartIndex..];

                    if (!Enum.TryParse(enumString, true, out EChallengeStatus result))
                        return false;

                    return _status == result;
            }

            return null;
        }
        #endregion



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake(). And at time of assiging to 'FirebaseAuth.DefaultInstance.StateChanged'
        /// </summary>
        private void HandleStateChanged(object obj, EventArgs args)
        {
            LoadSave(RefreshUI.Yes); // So Don't have to call on Awake()
        }
        #endregion
    }
}