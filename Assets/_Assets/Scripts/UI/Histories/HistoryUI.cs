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
        [SerializeField] private Button _changeLangButton;

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
            _changeLangButton.onClick.AddListener(ChangeLang);
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerHistory.OnBackButtonClick();
        private void ChangeLang() => _playerHistory.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}