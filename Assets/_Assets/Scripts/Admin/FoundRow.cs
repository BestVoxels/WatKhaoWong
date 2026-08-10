using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Admin
{
    public class FoundRow : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Row UI Event")]
        [SerializeField] private UnityEvent _onRowClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~UI~
        public void OnRowClick()
        {
            _onRowClick?.Invoke();
        }
        #endregion
    }
}