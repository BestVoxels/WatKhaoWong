using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Leaderboards;

namespace WatKhaoWong.UI.Leaderboards
{
    public class RankingUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;

        //[Header("Ranking UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private Ranking _playerRanking;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerRanking = GameObject.FindWithTag("Player").GetComponentInChildren<Ranking>();

            _backButton.onClick.AddListener(Back);
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerRanking.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}