using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Prays
{
    public class Pray : Page
    {
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