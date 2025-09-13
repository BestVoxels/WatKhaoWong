using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Utils.Localization
{
    public class Localizer : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Clipboard Status Text")]
        [SerializeField] private LocalizedAccountStatusEntry[] _LocalizedAccountStatusEntries;
        [SerializeField] private LocalizedTitleEntry[] _LocalizedTitleEntries;
        [SerializeField] private LocalizedActivityEntry[] _LocalizedActivityEntries;
        [SerializeField] private LocalizedHasCarEntry[] _LocalizedHasCarEntries;

        [Header("Formatter")]
        [SerializeField] private LocalizedString _dateEndsOn;
        [SerializeField] private string _dayFormat;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public string LocalizeAccountStatus(string eAccountStatusText)
        {
            if (Enum.TryParse(eAccountStatusText, true, out EAccountStatus accountStatus))
                return _LocalizedAccountStatusEntries.First(e => e.accountStatus == accountStatus).localizedString.GetLocalizedString();

            return eAccountStatusText;
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

        public string LocalizeHasCar(string eHasCarText)
        {
            if (Enum.TryParse(eHasCarText, true, out EHasCar hasCar))
                return _LocalizedHasCarEntries.First(e => e.hasCar == hasCar).localizedString.GetLocalizedString();

            return eHasCarText;
        }

        public string FormatBanEndDate(DateTime endDate)
        {
            return $"({_dateEndsOn.GetLocalizedString()} : {endDate.ToGregorianString(_dayFormat)})";
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
        public class LocalizedHasCarEntry
        {
            public LocalizedString localizedString;
            public EHasCar hasCar = EHasCar.None;
        }
        #endregion
    }
}