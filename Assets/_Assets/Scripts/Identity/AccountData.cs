using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.CoreItems;
using WatKhaoWong.Saving;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.Identity
{
    public class AccountData : MonoBehaviour, ISaveable
    {
        #region --Fields-- (Inspector)
        [Header("Account Stuffs")]
        [SerializeField] private ProfileIcon _defaultProfileIcon;
        [SerializeField] private string _defaultFirstName;
        [SerializeField] private string _defaultLastName;
        [SerializeField] private int _defaultLevel;
        [SerializeField] private string _defaultMemberSince;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnAccountDataUpdated;
        #endregion



        #region --Fields-- (In Class)
        private ProfileIcon _profileIcon;
        private string _firstName;
        private string _lastName;
        private int _level;
        private int _totalTMPoints;
        private int _todayTMPoints;
        private int _totalWonTMChallenge;
        private string _memberSince;

        private SavingWrapper _savingWrapper;
        private NumberFormatInfo _nfi;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void Start()
        {
            _nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            _nfi.NumberGroupSeparator = " ";
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Getter~
        public ProfileIcon GetProfileIcon()
        {
            if (_profileIcon == null)
                _profileIcon = _defaultProfileIcon;

            return _profileIcon;
        }

        public string GetUserNameText()
        {
            if (string.IsNullOrEmpty(_firstName) || string.IsNullOrEmpty(_lastName))
            {
                _firstName = _defaultFirstName;
                _lastName = _defaultLastName;
            }

            return $"{_firstName} {_lastName}";
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

        public string GetMemberSinceText()
        {
            if (string.IsNullOrEmpty(_memberSince))
                _memberSince = _defaultMemberSince;

            return $"{_memberSince}";
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Setter~
        public void SetFirstName(string input)
        {
            _firstName = input;
            
            OnAccountDataUpdated?.Invoke();
            _savingWrapper.Save();
        }

        public void SetLastName(string input)
        {
            _lastName = input;

            OnAccountDataUpdated?.Invoke();
            _savingWrapper.Save();
        }

        public void SetLevelText(int input)
        {
            _level = input;

            OnAccountDataUpdated?.Invoke();
            _savingWrapper.Save();
        }

        public void SetTotalTMPointsText(int input)
        {
            _totalTMPoints = input;

            OnAccountDataUpdated?.Invoke();
            _savingWrapper.Save();
        }

        public void SetTodayTMPointsText(int input)
        {
            _todayTMPoints = input;

            OnAccountDataUpdated?.Invoke();
            _savingWrapper.Save();
        }

        public void SetTotalWonTMChallengeText(int input)
        {
            _totalWonTMChallenge = input;

            OnAccountDataUpdated?.Invoke();
            _savingWrapper.Save();
        }

        public void SetMemberSinceText(string input)
        {
            _memberSince = input;

            OnAccountDataUpdated?.Invoke();
            _savingWrapper.Save();
        }

        public void SetProfileIcon(ProfileIcon input)
        {
            _profileIcon = input;

            OnAccountDataUpdated?.Invoke();
            _savingWrapper.Save();
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



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["ProfileIcon"] = _profileIcon.ItemID;
            data["FirstName"] = _firstName;
            data["LastName"] = _lastName;
            data["Level"] = _level;
            data["TotalTMPoints"] = _totalTMPoints;
            data["TodayTMPoints"] = _todayTMPoints;
            data["TotalWonTMChallenge"] = _totalWonTMChallenge;
            data["MemberSince"] = _memberSince;

            return data;
        }

        void ISaveable.RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;

            string id = (string)stateDict["ProfileIcon"];
            _profileIcon = BaseItem.GetFromID(id) as ProfileIcon;

            _firstName = (string)stateDict["FirstName"];
            _lastName = (string)stateDict["LastName"];
            _level = (int)stateDict["Level"];
            _totalTMPoints = (int)stateDict["TotalTMPoints"];
            _todayTMPoints = (int)stateDict["TodayTMPoints"];
            _totalWonTMChallenge = (int)stateDict["TotalWonTMChallenge"];
            _memberSince = (string)stateDict["MemberSince"];

            OnAccountDataUpdated?.Invoke();
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