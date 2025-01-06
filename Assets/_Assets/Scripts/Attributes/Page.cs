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
    }
}