using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Leaderboards
{
    public class Row : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Row UI Event")]
        [SerializeField] private UnityEvent _onClickMyselfRow;
        [SerializeField] private UnityEvent _onClickOtherUserRow;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~UI~
        public void OnClickMyselfRow()
        {
            Debug.LogWarning("Click \"Myself Row\" UI!");

            _onClickMyselfRow?.Invoke();
        }

        public void OnClickOtherUserRow()
        {
            Debug.LogWarning("Click \"Other User Row\" UI!");

            _onClickOtherUserRow?.Invoke();
        }
        #endregion
    }
}