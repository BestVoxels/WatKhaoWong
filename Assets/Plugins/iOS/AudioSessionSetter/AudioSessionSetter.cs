using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace WatKhaoWong.Plugins
{
    public class AudioSessionSetter : MonoBehaviour
    {
#if !UNITY_EDITOR
        private void Awake()
        {
            SetAudioSession();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            SetAudioSession();
        }
#endif


#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _SetAudioSession();

        public static void SetAudioSession()
        {
            _SetAudioSession();
        }

#elif !UNITY_EDITOR
        public static void SetAudioSession()
        {
            //not implemented --> fallback
        }
#endif
    }
}