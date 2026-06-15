using WatKhaoWong.Admin;
using UnityEngine.UI;
using UnityEngine;

namespace WatKhaoWong.UI.Admin
{
    public class SearchUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        //[Header("Search UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private Search _search;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _search = GameObject.FindWithTag("Player").GetComponentInChildren<Search>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _search.OnBackButtonClick();
        private void ChangeLang() => _search.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}