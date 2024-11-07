using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Homes;

namespace WatKhaoWong.UI.Homes
{
    public class TempleGuideUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;

        //[Header("TempleGuide UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private TempleGuide _playerTempleGuide;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerTempleGuide = GameObject.FindWithTag("Player").GetComponentInChildren<TempleGuide>();

            _backButton.onClick.AddListener(Back);
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerTempleGuide.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void RefreshUI()
        {

        }
        #endregion
    }
}