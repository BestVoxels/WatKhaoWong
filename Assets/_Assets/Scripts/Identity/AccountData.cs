using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.CoreItems;
using WatKhaoWong.SceneManagement;
using Firebase.Auth;

namespace WatKhaoWong.Identity
{
    public class AccountData : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Account Stuffs")]
        [SerializeField] private string _defaultFirstName;
        [SerializeField] private string _defaultLastName;
        [SerializeField] private string _defaultMemberSince;
        [SerializeField] private ProfileIcon _defaultProfileIcon;
        [SerializeField] private int _defaultLevel;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnAccountDataUpdated;
        #endregion



        #region --Fields-- (In Class)
        private string _firstName;
        private string _lastName;
        private DateTime? _memberSince = null;
        private ProfileIcon _profileIcon = null;
        private int _level;
        private int _totalTMPoints;
        private int _todayTMPoints;
        private int _totalWonTMChallenge;
        private DateTime _firstUploadTimeOfDay;

        private SavingWrapper _savingWrapper;
        private NumberFormatInfo _nfi;
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

        private void Start()
        {
            _nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            _nfi.NumberGroupSeparator = " ";
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Getter~
        public string GetUserNameText()
        {
            if (string.IsNullOrEmpty(_firstName) || string.IsNullOrEmpty(_lastName))
            {
                _firstName = _defaultFirstName;
                _lastName = _defaultLastName;
            }

            return $"{_firstName} {_lastName}";
        }

        public string GetMemberSinceText()
        {
            if (_memberSince == null)
                return $"{_defaultMemberSince}";


            return $"{_memberSince:d/M/yyyy}";
        }

        public ProfileIcon GetProfileIcon()
        {
            if (_profileIcon == null)
                _profileIcon = _defaultProfileIcon;

            return _profileIcon;
        }

        public string GetLevelText()
        {
            if (_level == 0)
                _level = _defaultLevel;

            return $"LV. {_level.ToString("#,0", _nfi)}";
        }

        public string GetTotalTMPointsText() => $"{_totalTMPoints.ToString("#,0", _nfi)}";

        public string GetTodayTMPointsText() => $"{_todayTMPoints.ToString("#,0", _nfi)}";

        public string GetTotalWonTMChallengeText() => $"{_totalWonTMChallenge.ToString("#,0", _nfi)}";
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Setter~
        public void SetFirstName(string input)
        {
            _firstName = input;

            _savingWrapper.Save(EValueNode.FirstName, _firstName);
            OnAccountDataUpdated?.Invoke();
        }

        public void SetLastName(string input)
        {
            _lastName = input;

            _savingWrapper.Save(EValueNode.LastName, _lastName);
            OnAccountDataUpdated?.Invoke();
        }

        public void SetMemberSinceText(DateTime input)
        {
            _memberSince = input;

            _savingWrapper.Save(EValueNode.MemberSince, _memberSince.ToString());
            OnAccountDataUpdated?.Invoke();
        }

        public void SetProfileIcon(ProfileIcon input)
        {
            _profileIcon = input;

            _savingWrapper.Save(EValueNode.ProfileIconID, _profileIcon.ItemID);
            OnAccountDataUpdated?.Invoke();
        }

        public void SetLevelText(int input)
        {
            _level = input;

            _savingWrapper.Save(EValueNode.Level, _level);
            OnAccountDataUpdated?.Invoke();
        }

        public void AddTotalTMPoints(int input)
        {
            _totalTMPoints += input;

            _savingWrapper.Save(EValueNode.TotalTMPoint, _totalTMPoints);
            OnAccountDataUpdated?.Invoke();
        }

        public void AddTodayTMPoints(int input)
        {
            if (input <= 0) return;

            ResetTMPointsDaily();

            if (_todayTMPoints == 0)
            {
                _firstUploadTimeOfDay = DateTime.Now;
                _savingWrapper.Save(EValueNode.FirstUploadTimeOfDay, DateTime.Now.ToString());
            }

            _todayTMPoints += input;
            _savingWrapper.Save(EValueNode.TodayTMPoint, _todayTMPoints);

            OnAccountDataUpdated?.Invoke();
        }

        public void AddTotalWonTMChallenge(int input)
        {
            _totalWonTMChallenge += input;

            _savingWrapper.Save(EValueNode.ChallengeWon, _totalWonTMChallenge);
            OnAccountDataUpdated?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void UpdateProfileIcon(IconUI oldUI, ProfileIcon newIcon, float multiplierRatioForDecorator)
        {
            if (newIcon == null)
            {
                Debug.LogError("Can't Update ProfileIcon to new one because it is Null.");
                return;
            }

            // Clear Spawned Decorators (no error if there are not)
            foreach (Transform each in oldUI.decoratorSpawnParent)
                Destroy(each.gameObject);

            // Replicate Toggle Profile to Main Profile
            oldUI.backgroundImage.color = newIcon.ProfileIconUI.UI.BackgroundColor;
            oldUI.iconImage.sprite = newIcon.ProfileIconUI.UI.Icon;
            oldUI.aspectRatioFitter.aspectRatio = newIcon.ProfileIconUI.UI.AspectRatio;
            oldUI.iconRect.pivot = newIcon.ProfileIconUI.UI.IconPivotY;

            if (newIcon.ProfileIconUI.UI.Decorators != null)
            {
                foreach (GameObject each in newIcon.ProfileIconUI.UI.Decorators)
                {
                    if (each == null) return; // Guard check MUST DO because InstaceID will be changed everytime we load UnityEditor.

                    GameObject result = Instantiate(each, oldUI.decoratorSpawnParent, false);

                    RectTransform rt = result.GetComponent<RectTransform>();
                    rt.localPosition = new Vector2(rt.localPosition.x * multiplierRatioForDecorator, rt.localPosition.y * multiplierRatioForDecorator);
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x * multiplierRatioForDecorator, rt.sizeDelta.y * multiplierRatioForDecorator);
                }
            }

            // Don't Call "_savingWrapper.Save()" because at the beginning it will saves default value and the actual save file will be gone.
            _profileIcon = newIcon; // Don't Call "SetProfileIcon()" because don't want "OnAccountDataUpdated?.Invoke()" to run. PREVENT Infinite Loop & Program Crashes.
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        public void ResetTMPointsDaily()
        {
            if (_firstUploadTimeOfDay.Date != DateTime.Today && _todayTMPoints > 0)
            {
                _todayTMPoints = 0;

                _savingWrapper.ForceSave(EValueNode.TodayTMPoint, 0);
                OnAccountDataUpdated?.Invoke();
            }
        }

        private async void LoadSave()
        {
            var data = await _savingWrapper.Load(EValueNode.FirstName);
            if (data != null)
                _firstName = data.Value.ToString();

            data = await _savingWrapper.Load(EValueNode.LastName);
            if (data != null)
                _lastName = data.Value.ToString();

            data = await _savingWrapper.Load(EValueNode.MemberSince);
            if (data != null)
            {
                if (DateTime.TryParse(data.Value.ToString(), out DateTime result))
                _memberSince = result;
            }

            data = await _savingWrapper.Load(EValueNode.ProfileIconID);
            if (data != null)
            {
                string id = data.Value.ToString();
                _profileIcon = BaseItem.GetFromID(id) as ProfileIcon;
            }

            data = await _savingWrapper.Load(EValueNode.Level);
            if (data != null)
                _level = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(EValueNode.TotalTMPoint);
            if (data != null)
                _totalTMPoints = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(EValueNode.TodayTMPoint);
            if (data != null)
                _todayTMPoints = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(EValueNode.ChallengeWon);
            if (data != null)
                _totalWonTMChallenge = int.Parse(data.Value.ToString());

            data = await _savingWrapper.Load(EValueNode.FirstUploadTimeOfDay);
            if (data != null)
            {
                if (DateTime.TryParse(data.Value.ToString(), out DateTime result))
                    _firstUploadTimeOfDay = result;

                ResetTMPointsDaily();
            }

            OnAccountDataUpdated?.Invoke();
        }
        #endregion



        #region --Methods-- (Interface)
        #endregion



        #region --Methods-- (Subscriber)
        private void HandleStateChanged(object obj, EventArgs args)
        {
            LoadSave(); // Don't have to LoadSave() on Awake() because it will be called once after FirebaseAuth instance is created.
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        [System.Serializable]
        public class IconUI
        {
            public Image backgroundImage;
            public Image iconImage;
            public AspectRatioFitter aspectRatioFitter;
            public RectTransform iconRect;
            public Transform decoratorSpawnParent;



            #region --Properties-- (Computed)
            public Color32 BackgroundColor => backgroundImage.color;
            public Sprite Icon => iconImage.sprite;
            public float AspectRatio => aspectRatioFitter.aspectRatio;
            public Vector2 IconPivotY => iconRect.pivot;
            public List<GameObject> Decorators
            {
                get
                {
                    List<GameObject> temp = new List<GameObject>();
                    foreach (Transform each in decoratorSpawnParent)
                        temp.Add(each.gameObject);

                    return temp;
                }
            }
            #endregion
        }
        #endregion
    }
}