using System;
using UnityEngine;
using WatKhaoWong.CoreItems;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;
using Firebase.Auth;

namespace WatKhaoWong.Identity
{
    /// <summary>
    /// --NOTE--
    /// Can't use Inheritance because 'MyUserData.cs' MUST inherit from Monobehavior BUT 'OtherUserData.cs' MUST NOT inherit from Monobehavior.
    /// SO have to use Composition for 'MyUserData.cs' & 'OtherUserData.cs' instead of Inheritance (which is to avoid over Composition anyways).
    /// ALSO 'MyUserData.cs' & 'OtherUserData.cs' implement interface 'IUserData.cs' to use Polymorphism concept, so both classes can be under 'IUserData.cs'.
    /// --------
    /// </summary>
    public class MyUserData : MonoBehaviour, IUserData
    {
        #region --Fields-- (Inspector)
        [Header("Account Stuffs")]
        [SerializeField] private string _defaultFirstName;
        [SerializeField] private string _defaultLastName;
        [SerializeField] private string _defaultMemberSince;
        [SerializeField] private ProfileIconItem _defaultProfileIcon;
        [SerializeField] private int _defaultLevel;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnMyUserDataUpdated;
        #endregion



        #region --Fields-- (In Class)
        private readonly Data _data = new Data();

        private SavingWrapper _savingWrapper;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged; // This will trigger on Start() too so don't have to call LoadSave() on Start()
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }
        #endregion


        
        #region --Methods-- (Custom PUBLIC) ~Setter~
        public void SetFirstName(string input)
        {
            _data.FirstName = input;

            _savingWrapper.Save(EValueNode.FirstName, _data.FirstName);
            OnMyUserDataUpdated?.Invoke();
        }

        public void SetLastName(string input)
        {
            _data.LastName = input;

            _savingWrapper.Save(EValueNode.LastName, _data.LastName);
            OnMyUserDataUpdated?.Invoke();
        }

        public void SetMemberSinceText(DateTime input)
        {
            _data.MemberSince = input;

            _savingWrapper.Save(EValueNode.MemberSince, _data.MemberSince.ToString());
            OnMyUserDataUpdated?.Invoke();
        }

        public void SetProfileIcon(ProfileIconItem input)
        {
            _data.ProfileIcon = input;

            _savingWrapper.Save(EValueNode.ProfileIconID, _data.ProfileIcon.ItemID);
            OnMyUserDataUpdated?.Invoke();
        }

        public void SetRole(EUserRole role)
        {
            print("***Role Changed***");
            _data.Role = role;

            _savingWrapper.Save(EValueNode.Role, _data.Role.ToString());
            OnMyUserDataUpdated?.Invoke();
        }

        public void SetLevelText(int input)
        {
            _data.Level = input;

            _savingWrapper.Save(EValueNode.Level, _data.Level);
            OnMyUserDataUpdated?.Invoke();
        }

        public void AddTotalTMPoints(int input)
        {
            _data.TotalTMPoints += input;

            _savingWrapper.Save(EValueNode.TotalTMPoint, _data.TotalTMPoints);
            OnMyUserDataUpdated?.Invoke();
        }

        public void AddTodayTMPoints(int input)
        {
            if (input <= 0) return;

            ResetTMPointsDaily();

            if (_data.TodayTMPoints == 0)
            {
                _data.FirstUploadTimeOfDay = DateTime.Now;
                _savingWrapper.Save(EValueNode.FirstUploadTimeOfDay, DateTime.Now.ToString());
            }

            _data.TodayTMPoints += input;
            _savingWrapper.Save(EValueNode.TodayTMPoint, _data.TodayTMPoints);

            OnMyUserDataUpdated?.Invoke();
        }

        public void AddTotalWonTMChallenge(int input)
        {
            _data.TotalWonTMChallenge += input;

            _savingWrapper.Save(EValueNode.ChallengeWon, _data.TotalWonTMChallenge);
            OnMyUserDataUpdated?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ResetTMPointsDaily()
        {
            if (_data.FirstUploadTimeOfDay.Date != DateTime.Today && _data.TodayTMPoints > 0)
            {
                _data.TodayTMPoints = 0;

                _savingWrapper.ForceSave(EValueNode.TodayTMPoint, 0);
                OnMyUserDataUpdated?.Invoke();
            }
        }

        private void SetRoleToGuestIfNoAuthen()
        {
            if (!FirebaseUtils.IsAuthenticated())
                _data.Role = EUserRole.Guest;
        }

        private async void LoadSave()
        {
            var data = await _savingWrapper.Load(EValueNode.FirstName);
            if (data != null)
                _data.FirstName = data.Value.ToString();

            data = await _savingWrapper.Load(EValueNode.LastName);
            if (data != null)
                _data.LastName = data.Value.ToString();

            data = await _savingWrapper.Load(EValueNode.MemberSince);
            if (data != null)
            {
                if (DateTime.TryParse(data.Value.ToString(), out DateTime result))
                _data.MemberSince = result;
            }

            data = await _savingWrapper.Load(EValueNode.ProfileIconID);
            if (data != null)
            {
                string id = data.Value.ToString();
                _data.ProfileIcon = BaseItem.GetFromID(id) as ProfileIconItem;
            }

            data = await _savingWrapper.Load(EValueNode.Role);
            if (data != null)
            {
                string roleString = data.Value.ToString();
                _data.Role = (EUserRole)Enum.Parse(typeof(EUserRole), roleString);
            }

            data = await _savingWrapper.Load(EValueNode.Level);
            if (data != null)
                _data.Level = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(EValueNode.TotalTMPoint);
            if (data != null)
                _data.TotalTMPoints = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(EValueNode.TodayTMPoint);
            if (data != null)
                _data.TodayTMPoints = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(EValueNode.ChallengeWon);
            if (data != null)
                _data.TotalWonTMChallenge = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(EValueNode.FirstUploadTimeOfDay);
            if (data != null)
            {
                if (DateTime.TryParse(data.Value.ToString(), out DateTime result))
                    _data.FirstUploadTimeOfDay = result;

                ResetTMPointsDaily();
            }

            OnMyUserDataUpdated?.Invoke();
        }
        #endregion



        #region --Methods-- (Interface) ~Getter~
        public string GetUserNameText()
        {
            if (string.IsNullOrEmpty(_data.FirstName) || string.IsNullOrEmpty(_data.LastName))
            {
                _data.FirstName = _defaultFirstName;
                _data.LastName = _defaultLastName;
            }

            return _data.GetUserNameText();
        }

        public string GetMemberSinceText()
        {
            if (_data.MemberSince == null)
                return $"{_defaultMemberSince}";


            return _data.GetMemberSinceText();
        }

        public ProfileIconItem GetProfileIcon()
        {
            if (_data.ProfileIcon == null)
                _data.ProfileIcon = _defaultProfileIcon;

            return _data.GetProfileIcon();
        }

        public EUserRole GetRole() => _data.GetRole();

        public string GetLevelText()
        {
            if (_data.Level == 0)
                _data.Level = _defaultLevel;

            return _data.GetLevelText();
        }

        public string GetTotalTMPointsText() => _data.GetTotalTMPointsText();

        public string GetTodayTMPointsText() => _data.GetTodayTMPointsText();

        public string GetTotalWonTMChallengeText() => _data.GetTotalWonTMChallengeText();
        #endregion



        #region --Methods-- (Interface)
        public void UpdateProfileIcon(ProfileIconInspector oldUI, ProfileIconItem newIcon, float multiplierRatioForDecorator)
        {
            _data.UpdateProfileIcon(oldUI, newIcon, multiplierRatioForDecorator);
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