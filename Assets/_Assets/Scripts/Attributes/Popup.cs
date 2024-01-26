using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Attributes
{
    public class Popup : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Popup Header UI Event")]
        [SerializeField] private UnityEvent _onCloseButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page Header UI~
        public virtual void OnCloseButtonClick()
        {
            Debug.LogWarning("Click \"Close\" Button! on Popup");

            _onCloseButtonClick?.Invoke();
        }
        #endregion
    }
}