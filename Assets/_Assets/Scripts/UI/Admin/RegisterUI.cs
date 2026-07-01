using WatKhaoWong.Admin;
using UnityEngine.UI;
using UnityEngine;

namespace WatKhaoWong.UI.Admin
{
    public class RegisterUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        //[Header("Register UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private Register _register;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _register = GameObject.FindWithTag("Player").GetComponentInChildren<Register>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _register.OnBackButtonClick();
        private void ChangeLang() => _register.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}