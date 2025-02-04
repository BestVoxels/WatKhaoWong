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

        // This is IMPORTANT because once it get initialized on Awake() it works fine but until other app run on background,
        // it works fine until some App run in background like Youtube then background audio from this app Will be overridden.
        // SO have to SetAudioSession() again once OnApplicationFocus() is true.
        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
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