using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.SharePopup;

namespace WatKhaoWong.UI.SharePopup
{
    public class LogoutButtonUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        //[Header("Popup Header UI Stuffs")]
        //[SerializeField] private Button _closeButton;

        [Header("Logout Button UI Stuffs")]
        [SerializeField] private Button _logoutButton;
        #endregion



        #region --Fields-- (In Class)
        private LogoutButton _playerLogoutButton;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerLogoutButton = GameObject.FindWithTag("Player").GetComponentInChildren<LogoutButton>();

            //_closeButton.onClick.AddListener(Close);

            _logoutButton.onClick.AddListener(Logout);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        //private void Close() => _playerLogoutButton.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Logout() => _playerLogoutButton.OnLogoutButtonClick();
        #endregion
    }
}