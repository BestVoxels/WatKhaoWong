using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Attributes
{
    public class Page : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Page Header UI Event")]
        [SerializeField] private UnityEvent _onBackButtonClick;
        [SerializeField] private UnityEvent _onChangeLangButtonClick;
        #endregion



        #region MyRegion
        private bool _isOpenOpposite;
        private IShowHidePagePopupUI _iShowHidePagePopupUI;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page Header UI~
        public void OnBackButtonClick()
        {
            _onBackButtonClick?.Invoke();
        }

        public void OnChangeLangButtonClick()
        {
            _onChangeLangButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void OpenPreviousPage()
        {
            if (_isOpenOpposite)
                _iShowHidePagePopupUI.OpenPageOpposite(0f);
            else
                _iShowHidePagePopupUI.OpenPage(0f);
        }

        public void SetPreviousPageToOpen(GameObject showHidePagePopupUI)
        {
            _iShowHidePagePopupUI = showHidePagePopupUI.GetComponent<IShowHidePagePopupUI>();

            _isOpenOpposite = false;
        }

        public void SetPreviousPageToOpenOpposite(GameObject showHidePagePopupUI)
        {
            _iShowHidePagePopupUI = showHidePagePopupUI.GetComponent<IShowHidePagePopupUI>();

            _isOpenOpposite = true;
        }
        #endregion
    }
}