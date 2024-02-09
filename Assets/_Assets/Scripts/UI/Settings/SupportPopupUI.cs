using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Settings;

namespace WatKhaoWong.UI.Settings
{
    public class SupportPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Support Popup UI Stuffs")]
        [SerializeField] private Button _watKhaoWongButton;
        [SerializeField] private Button _naraiSongritButton;
        [SerializeField] private Button _bestVoxelsButton;
        #endregion



        #region --Fields-- (In Class)
        private SupportPopup _playerSupportPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerSupportPopup = GameObject.FindWithTag("Player").GetComponentInChildren<SupportPopup>();

            _closeButton.onClick.AddListener(Close);

            _watKhaoWongButton.onClick.AddListener(WatKhaoWongButton);
            _naraiSongritButton.onClick.AddListener(NaraiSongritButton);
            _bestVoxelsButton.onClick.AddListener(BestVoxelsButton);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerSupportPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void WatKhaoWongButton()
        {
            _playerSupportPopup.OnWatKhaoWongButtonClick();
        }

        private void NaraiSongritButton()
        {
            _playerSupportPopup.OnNaraiSongritButtonClick();
        }

        private void BestVoxelsButton()
        {
            _playerSupportPopup.OnBestVoxelsButtonClick();
        }
        #endregion
    }
}