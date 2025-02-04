using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

namespace WatKhaoWong.Utils
{
    public class SceneLoader : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private string _sceneName;
        private Coroutine _previousCoroutine;
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator DelayLoadScene(int delaySec)
        {
            yield return new WaitForSeconds(delaySec);

            LoadScene(_sceneName);

            _previousCoroutine = null;

            yield break;
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void SetSceneName(string sceneName) => _sceneName = sceneName;

        public void DelayLoadSetSceneName(int delaySec)
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(DelayLoadScene(delaySec));
        }
        #endregion
    }
}