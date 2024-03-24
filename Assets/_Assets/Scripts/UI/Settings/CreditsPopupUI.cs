using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Settings;

namespace WatKhaoWong.UI.Settings
{
    public class CreditsPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Credits Popup UI Stuffs")]
        [SerializeField] private Button _closeButton3;
        #endregion



        #region --Fields-- (In Class)
        private CreditsPopup _playerCreditsPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerCreditsPopup = GameObject.FindWithTag("Player").GetComponentInChildren<CreditsPopup>();

            _closeButton.onClick.AddListener(Close);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerCreditsPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        #endregion
    }
}