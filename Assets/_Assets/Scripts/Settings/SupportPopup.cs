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
            Debug.LogWarning("Click \"WatKhaoWong\" Button! on Popup");

            _onWatKhaoWongButtonClick?.Invoke();
        }

        public void OnNaraiSongritButtonClick()
        {
            Debug.LogWarning("Click \"NaraiSongrit\" Button! on Popup");

            _onNaraiSongritButtonClick?.Invoke();
        }

        public void OnBestVoxelsButtonClick()
        {
            Debug.LogWarning("Click \"BestVoxels\" Button! on Popup");

            _onBestVoxelsButtonClick?.Invoke();
        }
        #endregion
    }
}