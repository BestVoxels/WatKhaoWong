using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Utils.Localization
{
    public class Localizer : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Localize")]
        [SerializeField] private LocalizedAccountStatusEntry[] _LocalizedAccountStatusEntries;
        [SerializeField] private LocalizedTitleEntry[] _LocalizedTitleEntries;
        [SerializeField] private LocalizedActivityEntry[] _LocalizedActivityEntries;
        [SerializeField] private LocalizedBuildingNameEntry[] _LocalizedBuildingNameEntries;
        [SerializeField] private LocalizedHasCarEntry[] _LocalizedHasCarEntries;
        [SerializeField] private LocalizedReputationEntry[] _LocalizedReputationEntries;

        [Header("Colorize")]
        [Tooltip("Default Color when Enum Parsing is not working corrently.")]
        [SerializeField] private Color32 _defaultColor;
        [Space]
        [SerializeField] private ColorizedAccountStatusEntry[] _colorizedAccountStatusEntries;
        [SerializeField] private ColorizedReputationEntry[] _colorizedReputationEntries;

        [Header("Formatter")]
        [SerializeField] private LocalizedString _dateEndsOn;
        [SerializeField] private string _dayFormat;
        [Space]
        [SerializeField] private LocalizedString _ageText;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public string FormatBanEndDate(DateTime endDate)
        {
            return $"({_dateEndsOn.GetLocalizedString()} : {endDate.ToGregorianString(_dayFormat)})";
        }

        public string FormatAge(int age)
        {
            return _ageText.GetLocalizedString(age);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Localize~
        public string LocalizeAccountStatus(string eAccountStatusText)
        {
            if (Enum.TryParse(eAccountStatusText, true, out EAccountStatus accountStatus))
                return _LocalizedAccountStatusEntries.First(e => e.accountStatus == accountStatus).localizedString.GetLocalizedString();

            return eAccountStatusText;
        }

        public string LocalizeAccountStatusLangCombined(string eAccountStatusText)
        {
            Locale thai = LocalizationSettings.AvailableLocales.GetLocale("th");
            Locale english = LocalizationSettings.AvailableLocales.GetLocale("en");

            string result = eAccountStatusText;

            if (Enum.TryParse(eAccountStatusText, true, out EAccountStatus accountStatus))
            {
                LocalizedAccountStatusEntry firstMatch = _LocalizedAccountStatusEntries.First(e => e.accountStatus == accountStatus);
                
                var table = firstMatch.localizedString.TableReference;
                var entry = firstMatch.localizedString.TableEntryReference;

                result = $"{LocalizationSettings.StringDatabase.GetLocalizedString(table, entry, thai)} {LocalizationSettings.StringDatabase.GetLocalizedString(table, entry, english)}";
            }

            return result;
        }

        public string LocalizeUserTitle(string eTitleText)
        {
            if (Enum.TryParse(eTitleText, true, out EUserTitle title))
                return _LocalizedTitleEntries.First(e => e.userTitle == title).localizedString.GetLocalizedString();

            return eTitleText;
        }

        public string LocalizeActivityType(string eActivityText)
        {
            if (Enum.TryParse(eActivityText, true, out EActivityType activity))
                return _LocalizedActivityEntries.First(e => e.activityType == activity).localizedString.GetLocalizedString();

            return eActivityText;
        }

        public string LocalizeBuildingName(string eBuildingNameText)
        {
            if (Enum.TryParse(eBuildingNameText, true, out EBuildingName buildingName))
                return _LocalizedBuildingNameEntries.First(e => e.buildingName == buildingName).localizedString.GetLocalizedString();

            return eBuildingNameText;
        }

        public string LocalizeBuildingNameLangCombined(string eBuildingNameText)
        {
            Locale thai = LocalizationSettings.AvailableLocales.GetLocale("th");
            Locale english = LocalizationSettings.AvailableLocales.GetLocale("en");

            string result = eBuildingNameText;

            if (Enum.TryParse(eBuildingNameText, true, out EBuildingName buildingName))
            {
                LocalizedBuildingNameEntry firstMatch = _LocalizedBuildingNameEntries.First(e => e.buildingName == buildingName);
                
                var table = firstMatch.localizedString.TableReference;
                var entry = firstMatch.localizedString.TableEntryReference;

                result = $"{LocalizationSettings.StringDatabase.GetLocalizedString(table, entry, thai)} {LocalizationSettings.StringDatabase.GetLocalizedString(table, entry, english)}";
            }

            return result;
        }

        public string LocalizeHasCar(string eHasCarText)
        {
            if (Enum.TryParse(eHasCarText, true, out EHasCar hasCar))
                return _LocalizedHasCarEntries.First(e => e.hasCar == hasCar).localizedString.GetLocalizedString();

            return eHasCarText;
        }

        public string LocalizeReputation(string eReputationText)
        {
            if (Enum.TryParse(eReputationText, true, out EReputation reputation))
                return _LocalizedReputationEntries.First(e => e.reputation == reputation).localizedString.GetLocalizedString();

            return eReputationText;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Colorize~
        public Color32 ColorizeAccountStatus(string eAccountStatusText)
        {
            if (Enum.TryParse(eAccountStatusText, true, out EAccountStatus accountStatus))
                return _colorizedAccountStatusEntries.First(e => e.accountStatus == accountStatus).color;

            return _defaultColor;
        }

        public Color32 ColorizeReputation(string eReputationText)
        {
            if (Enum.TryParse(eReputationText, true, out EReputation reputation))
                return _colorizedReputationEntries.First(e => e.reputation == reputation).color;

            return _defaultColor;
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        [System.Serializable]
        public class LocalizedAccountStatusEntry
        {
            public LocalizedString localizedString;
            public EAccountStatus accountStatus = EAccountStatus.Normal;
        }

        [System.Serializable]
        public class LocalizedTitleEntry
        {
            public LocalizedString localizedString;
            public EUserTitle userTitle = EUserTitle.Guest;
        }

        [System.Serializable]
        public class LocalizedActivityEntry
        {
            public LocalizedString localizedString;
                public EActivityType activityType = EActivityType.MeditationRetreat;
        }

        [System.Serializable]
        public class LocalizedBuildingNameEntry
        {
            public LocalizedString localizedString;
            public EBuildingName buildingName = EBuildingName.MonkHut;
        }

        [System.Serializable]
        public class LocalizedHasCarEntry
        {
            public LocalizedString localizedString;
            public EHasCar hasCar = EHasCar.None;
        }

        [System.Serializable]
        public class LocalizedReputationEntry
        {
            public LocalizedString localizedString;
            public EReputation reputation = EReputation.Normal;
        }

        [System.Serializable]
        public class ColorizedAccountStatusEntry
        {
            public Color32 color;
            public EAccountStatus accountStatus = EAccountStatus.Normal;
        }

        [System.Serializable]
        public class ColorizedReputationEntry
        {
            public Color32 color;
            public EReputation reputation = EReputation.Normal;
        }
        #endregion
    }
}