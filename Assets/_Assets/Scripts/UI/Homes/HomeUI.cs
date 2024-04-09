using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Homes;

namespace WatKhaoWong.UI.Homes
{
    public class HomeUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Home UI Stuffs")]
        [SerializeField] private Button _historyButton;
        [SerializeField] private Button _prayButton;
        [SerializeField] private Button _settingButton;
        [Space]
        [SerializeField] private Image _coverImage;
        [SerializeField] private TMP_Text _welcomeText;
        #endregion



        #region --Fields-- (In Class)
        private Home _playerHome;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerHome = GameObject.FindWithTag("Player").GetComponentInChildren<Home>();

            _historyButton.onClick.AddListener(History);
            _prayButton.onClick.AddListener(Pray);
            _settingButton.onClick.AddListener(Setting);
        }

        private void OnEnable()
        {
            RefreshUI();

            UIRefresher.OnHomeRefreshed += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            UIRefresher.OnHomeRefreshed -= RefreshUI;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void History() => _playerHome.OnHistoryButtonClick();
        private void Pray() => _playerHome.OnPrayButtonClick();
        private void Setting() => _playerHome.OnSettingButtonClick();

        private void RefreshUI()
        {
            _coverImage.overrideSprite = _playerHome.GetCoverImage();
            _welcomeText.text = _playerHome.GetWelcomeText();
        }
        #endregion
    }
}