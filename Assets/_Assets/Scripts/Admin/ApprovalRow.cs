using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace WatKhaoWong.Admin
{
    public class ApprovalRow : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("Row UI Event")]
        [SerializeField] private UnityEvent _onRowClick;
        [SerializeField] private UnityEvent _onClickReject;
        [SerializeField] private UnityEvent _onClickAccept;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Text")]
        [field: SerializeField] public LocalizedString NoDataText { get; private set; }
        [field: Header("Day Format on Result Text")]
        [field: SerializeField] public string DayFormat { get; private set; } = "d/M/yyyy";
        #endregion



        #region --Methods-- (Custom PUBLIC) ~UI~
        public void OnRowClick()
        {
            _onRowClick?.Invoke();
        }

        public void OnClickReject()
        {
            _onClickReject?.Invoke();
        }

        public void OnClickAccept()
        {
            _onClickAccept?.Invoke();
        }
        #endregion
    }
}