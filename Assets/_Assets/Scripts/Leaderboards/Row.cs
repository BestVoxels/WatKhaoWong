using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Leaderboards
{
    public class Row : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        [field: Header("Row Settings")]
        [field: Tooltip("Shows when Leaderboard is NOT Exists / Has no Data from Server.")]
        [field: SerializeField] public string NullRankText { get; private set; } = "no rank";
        [field: Tooltip("Shows when Leaderboard is NOT Exists / Has no Data from Server.")]
        [field: SerializeField] public string NullScoreText { get; private set; } = "no score";
        [field: Space]
        [field: Tooltip("Shows when Leaderboard is Exists BUT user's score is not high enough to be in leaderboard.")]
        [field: SerializeField] public string NotInLeaderboardTextBegin { get; private set; } = ">";
        [field: Tooltip("Shows when Leaderboard is Exists BUT user's score is not high enough to be in leaderboard.")]
        [field: SerializeField] public bool ShowRankIfNotInLeaderboard { get; private set; } = true;
        [field: Tooltip("Shows when Leaderboard is Exists BUT user's score is not high enough to be in leaderboard.")]
        [field: SerializeField] public string NotInLeaderboardTextEnd { get; private set; } = "";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Row UI Event")]
        [SerializeField] private UnityEvent _onClickMyselfRow;
        [SerializeField] private UnityEvent _onClickOtherUserRow;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~UI~
        public void OnClickMyselfRow()
        {
            _onClickMyselfRow?.Invoke();
        }

        public void OnClickOtherUserRow()
        {
            _onClickOtherUserRow?.Invoke();
        }
        #endregion
    }
}