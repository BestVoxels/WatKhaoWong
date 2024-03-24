using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Settings;

namespace WatKhaoWong.UI.Settings
{
    public class LanguagePopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        //[Header("Language Popup UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private LanguagePopup _playerLanguagePopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerLanguagePopup = GameObject.FindWithTag("Player").GetComponentInChildren<LanguagePopup>();

            _closeButton.onClick.AddListener(Close);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerLanguagePopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        #endregion
    }
}