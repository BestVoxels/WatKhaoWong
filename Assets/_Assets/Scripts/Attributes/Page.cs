using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Attributes
{
    public class Page : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Page Header UI Event")]
        [SerializeField] private UnityEvent _onBackButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page Header UI~
        public void OnBackButtonClick()
        {
            _onBackButtonClick?.Invoke();
        }
        #endregion
    }
}