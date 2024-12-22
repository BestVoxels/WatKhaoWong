using UnityEngine;
using System;

namespace WatKhaoWong.Core
{
    /// <summary>
    /// Attach this script on any GameObject in the Scene, GameObject must not be disabled. Any Level is fine, parent or child level, fine.
    /// RECOMMEND NAME for GameObject in the Scene -> "Frame Rate Setter (for Mobile Platform ONLY)"
    /// </summary>
    public class FrameRateSetter : MonoBehaviour
    {
        #region --Methods-- (Built In)
        private void Start()
        {
            SetToMaxFrameRate();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void SetToMaxFrameRate()
        {
            if (Application.isMobilePlatform) // return true if the application is running on iOS, Android, or WSA (Windows Subsystem for Android)
            {
                double maxRefreshRate = Screen.currentResolution.refreshRateRatio.value;
                int maxRefreshRateRounded = (int)Math.Round(maxRefreshRate, MidpointRounding.AwayFromZero);

                Application.targetFrameRate = maxRefreshRateRounded;
                // NO NEED to modify 'QualitySettings.vSyncCount' since it get IGNORE on Mobile.
            }
        }

        public void SetTo60FrameRate()
        {
            if (Application.isMobilePlatform) // return true if the application is running on iOS, Android, or WSA (Windows Subsystem for Android)
            {
                Application.targetFrameRate = 60;
                // NO NEED to modify 'QualitySettings.vSyncCount' since it get IGNORE on Mobile.
            }
        }

        public void SetToMinFrameRate()
        {
            if (Application.isMobilePlatform) // return true if the application is running on iOS, Android, or WSA (Windows Subsystem for Android)
            {
                Application.targetFrameRate = 30;
                // NO NEED to modify 'QualitySettings.vSyncCount' since it get IGNORE on Mobile.
            }
        }
        #endregion
    }
}