using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Prays
{
    public class Popup : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Popup Header UI Event")]
        [SerializeField] private UnityEvent _onCloseButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page Header UI~
        public void OnCloseButtonClick()
        {
            Debug.LogWarning("Click \"Close\" Button! on Popup");

            _onCloseButtonClick?.Invoke();
        }
        #endregion
    }
}