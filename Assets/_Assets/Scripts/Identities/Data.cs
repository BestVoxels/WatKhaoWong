using UnityEngine;
using System;
using System.Globalization;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.Identities
{
    /// <summary>
    /// --NOTE--
    /// Limit 'Data.cs' visibility to only within 'Identity' namespace.
    /// SO that other classes outside of 'Identity' namespace is forced to use 'MyUserData.cs' & 'OtherUserData.cs' to deal with User Data instead.
    /// --------
    ///
    /// 
    /// --NOTE--
    /// Can't use Inheritance because 'MyUserData.cs' MUST inherit from Monobehavior BUT 'OtherUserData.cs' MUST NOT inherit from Monobehavior.
    /// SO have to use Composition for 'MyUserData.cs' & 'OtherUserData.cs' instead of Inheritance (which is to avoid over Composition anyways).
    /// ALSO 'MyUserData.cs' & 'OtherUserData.cs' implement interface 'IUserData.cs' to use Polymorphism concept, so both classes can be under 'IUserData.cs'.
    /// --------
    /// </summary>
    internal class Data
    {
        #region --Fields-- (In Class)
        private readonly NumberFormatInfo _nfi;
        #endregion



        #region --Properties-- (Auto)
        internal string FirstName { get; set; }
        internal string LastName { get; set; }
        internal DateTime? MemberSince { get; set; } = null;
        internal ProfileIconItem ProfileIcon { get; set; } = null;
        internal EUserRole Role { get; set; } = EUserRole.Guest;
        internal string Title { get; set; } = EUserTitle.Guest.ToString();
        internal int Level { get; set; } = 1;
        internal int TotalTMPoints { get; set; }
        internal int TodayTMPoints { get; set; }
        internal int ChallengeTMPoints { get; set; }
        internal int TotalChallengeTMWon { get; set; }
        internal int TMPointCapRequest { get; set; }
        internal int TMPointCap { get; set; }
        internal int TMPointCapRound { get; set; }
        internal bool IsCustomTMPointCap { get; set; } = false;
        internal DateTime FirstUploadTimeOfDayTM { get; set; }
        internal DateTime FirstUploadTimeOfChallengeTM { get; set; }
        internal bool TempleGuideConfirmed { get; set; } = false;

        internal GeneralInfo GeneralInfo { get; set; } = null;
        #endregion



        #region --Constructors-- (INTERNAL)
        internal Data()
        {
            _nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            _nfi.NumberGroupSeparator = " ";
        }
        #endregion



        #region --Methods-- (Custom INTERNAL) ~Get Text Format~
        internal string GetUserNameText() => $"{FirstName} {LastName}";

        internal string GetMemberSinceText() => $"{MemberSince.ToGregorianOnlyDateString()}";

        internal ProfileIconItem GetProfileIcon() => ProfileIcon;

        internal EUserRole GetRole() => Role;

        internal string GetTitleText() => Title;

        internal string GetLevelText() => $"LV. {Level.ToString("#,0", _nfi)}";

        internal string GetTotalTMPointsText() => $"{TotalTMPoints.ToString("#,0", _nfi)}";

        internal string GetTodayTMPointsText() => $"{TodayTMPoints.ToString("#,0", _nfi)}";

        internal string GetChallengeTMPointsText() => $"{ChallengeTMPoints.ToString("#,0", _nfi)}";

        internal string GetTotalChallengeTMWonText() => $"{TotalChallengeTMWon.ToString("#,0", _nfi)}";
        #endregion



        #region --Methods-- (Custom INTERNAL)
        internal void UpdateProfileIcon(ProfileIconInspector oldUI, ProfileIconItem newIcon, float multiplierRatioForDecorator)
        {
            if (newIcon == null)
            {
                Debug.LogError("Can't Update ProfileIcon to new one because it is Null.");
                return;
            }

            // Clear Spawned Decorators (no error if there are not)
            foreach (Transform each in oldUI.decoratorSpawnParent)
                UnityEngine.Object.Destroy(each.gameObject);

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

                    GameObject result = UnityEngine.Object.Instantiate(each, oldUI.decoratorSpawnParent, false);

                    RectTransform rt = result.GetComponent<RectTransform>();
                    rt.localPosition = new Vector2(rt.localPosition.x * multiplierRatioForDecorator, rt.localPosition.y * multiplierRatioForDecorator);
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x * multiplierRatioForDecorator, rt.sizeDelta.y * multiplierRatioForDecorator);
                }
            }

            // Don't Call "_savingWrapper.Save()" because at the beginning it will saves default value and the actual save file will be gone.
            ProfileIcon = newIcon; // Don't Call "SetProfileIcon()" because don't want "OnAccountDataUpdated?.Invoke()" to run. PREVENT Infinite Loop & Program Crashes.
        }
        #endregion
    }
}