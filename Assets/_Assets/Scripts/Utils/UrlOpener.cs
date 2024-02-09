using UnityEngine;

namespace WatKhaoWong.Utils
{
    public class UrlOpener : MonoBehaviour
    {
        #region --Methods-- (Subscriber) ~UnityEvent~
        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
        #endregion
    }
}