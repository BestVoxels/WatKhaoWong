using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Histories;

namespace WatKhaoWong.UI.Histories
{
    public class HistoryUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;

        //[Header("History UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private History _playerHistory;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerHistory = GameObject.FindWithTag("Player").GetComponentInChildren<History>();

            _backButton.onClick.AddListener(Back);
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerHistory.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}