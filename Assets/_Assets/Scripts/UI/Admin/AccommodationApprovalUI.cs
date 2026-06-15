using WatKhaoWong.Admin;
using UnityEngine.UI;
using UnityEngine;

namespace WatKhaoWong.UI.Admin
{
    public class AccommodationApprovalUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        //[Header("AccommodationApproval UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private AccommodationApproval _accommodationApproval;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _accommodationApproval = GameObject.FindWithTag("Player").GetComponentInChildren<AccommodationApproval>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _accommodationApproval.OnBackButtonClick();
        private void ChangeLang() => _accommodationApproval.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}