using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Leaderboards;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.Leaderboards
{
    public class FilterButtonUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Filter Settings")]
        [SerializeField] private ELeaderboardCategory _category;

        [Space]

        [Header("Filter Stuffs")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonImage;
        #endregion



        #region --Fields-- (In Class)
        private Leaderboard _leaderboard;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _statusText = FindAnyObjectByType<StatusText>();

            _button.onClick.AddListener(SetFilter);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(Leaderboard leaderboard)
        {
            _leaderboard = leaderboard;
        }

        public void RefreshUI()
        {
            if (_leaderboard == null) return;

            _buttonImage.color = (_category == _leaderboard.Category) ? _leaderboard.SelectedColor : _leaderboard.UnselectedColor;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void SetFilter()
        {
            if (_leaderboard == null) return;
            if (_category == _leaderboard.Category) return;

            if (Leaderboard.IsAsyncRunning)
            {
                string categoryName = _leaderboard.CategoryName.First(e => e.category == _leaderboard.Category).localizedString.GetLocalizedString();

                _statusText.Show(_leaderboard.CantChangeCategory.GetLocalizedString(categoryName), _leaderboard.CantChangeCategoryColor);
                return;
            }

            _leaderboard.Category = _category;
        }
        #endregion
    }
}