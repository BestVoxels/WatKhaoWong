using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Authentication;

namespace WatKhaoWong.UI.Authentication
{
    public class DeleteButtonUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        //[Header("Popup Header UI Stuffs")]
        //[SerializeField] private Button _closeButton;

        [Header("Delete Button UI Stuffs")]
        [SerializeField] private Button _deleteButton;
        #endregion



        #region --Fields-- (In Class)
        private DeleteButton _playerDeleteButton;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerDeleteButton = GameObject.FindWithTag("Player").GetComponentInChildren<DeleteButton>();

            //_closeButton.onClick.AddListener(Close);

            _deleteButton.onClick.AddListener(Delete);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        //private void Close() => _playerDeleteButton.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Delete() => _playerDeleteButton.OnDeleteButtonClick();
        #endregion
    }
}