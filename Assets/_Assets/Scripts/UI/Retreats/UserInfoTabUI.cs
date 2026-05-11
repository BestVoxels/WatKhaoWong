using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;

namespace WatKhaoWong.UI.Retreats
{
    public class UserInfoTabUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Filter Settings")]
        [SerializeField] private EUserInfoTab _eTab;

        [Space]

        [Header("Filter Stuffs")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonImage;
        #endregion



        #region --Fields-- (In Class)
        private UserInfo _userInfo;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _button.onClick.AddListener(SetETab);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(UserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        public void UpdateColor()
        {
            if (_userInfo == null) return;

            _buttonImage.color = (_eTab == _userInfo.Tab) ? _userInfo.SelectedColor : _userInfo.UnselectedColor;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void SetETab()
        {
            if (_userInfo == null) return;
            if (_eTab == _userInfo.Tab) return;

            _userInfo.Tab = _eTab;
        }
        #endregion
    }
}