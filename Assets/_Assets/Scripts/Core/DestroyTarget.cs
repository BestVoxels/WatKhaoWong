using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Core
{
    public class DestroyTarget : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Destroy Target Stuffs")]
        [SerializeField] private GameObject _targetToDestroy = null;
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Destroy Target Event")]
        [SerializeField] private UnityEvent _onBeforeDestroy;
        #endregion



        #region --Methods-- (Animation Event)
        private void DestroyTargetGameObject()
        {
            _onBeforeDestroy?.Invoke();

            Destroy(_targetToDestroy);
        }
        #endregion
    }
}