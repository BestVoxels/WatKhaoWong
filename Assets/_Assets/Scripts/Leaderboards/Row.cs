using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Leaderboards
{
    public class Row : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        [field: Header("Row Settings")]
        [field: SerializeField] public string DefaultNullScoreText { get; private set; } = "null";
        #endregion



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