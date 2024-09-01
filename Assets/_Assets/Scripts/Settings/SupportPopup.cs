using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Settings
{
    public class SupportPopup : Popup
    {
        #region --Events-- (UnityEvent)
        [Header("Support Popup UI Event")]
        [SerializeField] private UnityEvent _onWatKhaoWongButtonClick;
        [SerializeField] private UnityEvent _onNaraiSongritButtonClick;
        [SerializeField] private UnityEvent _onBestVoxelsButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnWatKhaoWongButtonClick()
        {
            _onWatKhaoWongButtonClick?.Invoke();
        }

        public void OnNaraiSongritButtonClick()
        {
            _onNaraiSongritButtonClick?.Invoke();
        }

        public void OnBestVoxelsButtonClick()
        {
            _onBestVoxelsButtonClick?.Invoke();
        }
        #endregion
    }
}