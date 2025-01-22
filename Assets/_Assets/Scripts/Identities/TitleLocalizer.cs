using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

namespace WatKhaoWong.Identities
{
    public class TitleLocalizer : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Clipboard Status Text")]
        [SerializeField] private LocalizedTitleEntry[] _LocalizedTitleEntries;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public string Localize(string titleText)
        {
            if (Enum.TryParse(titleText, true, out EUserTitle title))
            {
                return _LocalizedTitleEntries.First(e => e.userTitle == title).localizedString.GetLocalizedString();
            }
            else
            {
                return titleText;
            }
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        public class LocalizedTitleEntry
        {
            public LocalizedString localizedString;
            public EUserTitle userTitle = EUserTitle.Guest;
        }
        #endregion
    }
}