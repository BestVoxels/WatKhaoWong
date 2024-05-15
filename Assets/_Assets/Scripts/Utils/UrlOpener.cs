using UnityEngine;
using System.Collections;

namespace WatKhaoWong.Utils
{
    public class UrlOpener : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private string _url;
        private Coroutine _previousCoroutine;
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator DelayOpenUrl(int delaySec)
        {
            yield return new WaitForSeconds(delaySec);

            OpenUrl(_url);

            _previousCoroutine = null;

            yield break;
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }

        public void SetUrl(string url) => _url = url;

        public void DelayOpenSetUrl(int delaySec)
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(DelayOpenUrl(delaySec));
        }
        #endregion
    }
}