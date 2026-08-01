using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Admin;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.Admin
{
    public class ApprovalFilterUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Filter Settings")]
        [SerializeField] private EApprovalCategory _category;

        [Space]

        [Header("Filter Stuffs")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonImage;
        #endregion



        #region --Fields-- (In Class)
        private ApprovalBoard _board;
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
        public void Setup(ApprovalBoard board)
        {
            _board = board;
        }

        public void RefreshUI()
        {
            if (_board == null) return;

            _buttonImage.color = (_category == _board.Category) ? _board.SelectedColor : _board.UnselectedColor;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void SetFilter()
        {
            if (_board == null) return;
            if (_category == _board.Category) return;

            if (ApprovalBoard.IsAsyncRunning)
            {
                string categoryName = _board.CategoryName.First(e => e.category == _board.Category).localizedString.GetLocalizedString();

                _statusText.Show(_board.CantChangeCategory.GetLocalizedString(categoryName), _board.CantChangeCategoryColor);
                ApprovalBoard.ShowStatusRowLoaded = true;

                return;
            }

            ApprovalBoard.ShowStatusRowLoaded = false;
            _board.Category = _category;
        }
        #endregion
    }
}