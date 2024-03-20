using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.SharePopup;

namespace WatKhaoWong.UI.SharePopup
{
    public class AccountPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        //[Header("Account Popup UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private AccountPopup _playerAccountPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerAccountPopup = GameObject.FindWithTag("Player").GetComponentInChildren<AccountPopup>();

            _closeButton.onClick.AddListener(Close);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerAccountPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        #endregion
    }
}