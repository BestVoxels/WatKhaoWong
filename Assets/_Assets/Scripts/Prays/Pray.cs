using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Prays
{
    public class Pray : Page
    {
        #region --Properties-- (Inspector)
        [field: Header("Pray Text")]
        [field: SerializeField] public LocalizedString AllTimeText { get; private set; }
        [field: SerializeField] public LocalizedString TodayText { get; private set; }
        [field: SerializeField] public LocalizedString ChallengeText { get; private set; }

        [field: Space]

        [field: Header("Pray - Settings")]
        [field: SerializeField] public string ValueTextFormatBegin { get; private set; } = "<space=25><b><cspace=-3>";
        [field: SerializeField] public string ValueTextFormatEnd { get; private set; } = "</cspace></b>";
        #endregion


        #region --Events-- (UnityEvent)
        [Header("Pray UI Event")]
        [SerializeField] private UnityEvent _onUserProfileClick;
        [SerializeField] private UnityEvent _onUserStatsClick;
        [Space]
        [SerializeField] private UnityEvent _onDoneButtonClick;
        [SerializeField] private UnityEvent _onPlaySoundButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnUserProfileClick()
        {
            _onUserProfileClick?.Invoke();
        }

        public void OnUserStatsClick()
        {
            _onUserStatsClick?.Invoke();
        }

        public void OnDoneButtonClick()
        {
            _onDoneButtonClick?.Invoke();
        }

        public void OnPlaySoundButtonClick()
        {
            _onPlaySoundButtonClick?.Invoke();
        }
        #endregion
    }
}