using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Utils.UI
{
    public class PointUploadEvents : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("TM Points Events")]
        public UnityEvent<int> OnTMPointsUploadSucceeded;
        public UnityEvent<int> OnTMPointsUploadSucceededPartial;
        public UnityEvent<int> OnTMPointsUploadSucceededCapRound;
        public UnityEvent OnTMPointsUploadFailedCap;
        public UnityEvent OnTMPointsUploadFailedZero;
        public UnityEvent OnTMPointsUploadFailedNegative;

        // [Header("OtherKind Points Events")]
        #endregion
    }
}