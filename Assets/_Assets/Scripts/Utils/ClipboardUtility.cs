using UnityEngine;
using UnityEngine.Localization;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Utils
{
    public class ClipboardUtility : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Clipboard Status Text")]
        [SerializeField] private LocalizedString _statusCopySucceeded;
        [SerializeField] private Color32 _statusCopySucceededColor;
        [Space]
        [SerializeField] private LocalizedString _statusCopyErrored;
        [SerializeField] private Color32 _statusCopyErroredColor;
        #endregion



        #region --Fields-- (In Class)
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _statusText = FindAnyObjectByType<StatusText>();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void CopyToClipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;

            _statusText.Show(_statusCopySucceeded.GetLocalizedString(text), _statusCopySucceededColor);
        }
        #endregion
    }
}