using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Homes;

namespace WatKhaoWong.UI.Homes
{
    public class AuspiciousCalendarUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;

        //[Header("AuspiciousCalendar UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private AuspiciousCalendar _playerAuspiciousCalendar;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerAuspiciousCalendar = GameObject.FindWithTag("Player").GetComponentInChildren<AuspiciousCalendar>();

            _backButton.onClick.AddListener(Back);
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerAuspiciousCalendar.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}