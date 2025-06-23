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
        [SerializeField] private Button _templeHistoryButton;
        [SerializeField] private Button _abbotHistoryButton;
        [SerializeField] private Button _templeGuideButton;
        [SerializeField] private Button _mapButton;
        [SerializeField] private Button _donationAccountsButton;
        [SerializeField] private Button _auspiciousCalendarButton;
        [Space]
        [SerializeField] private Button _dhammaButton;
        [SerializeField] private Button _prayButton;
        [SerializeField] private Button _bookMeditationButton;
        [SerializeField] private Button _manageMembersButton;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _rankingButton;
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

            _templeHistoryButton.onClick.AddListener(TempleHistory);
            _abbotHistoryButton.onClick.AddListener(AbbotHistory);
            _templeGuideButton.onClick.AddListener(TempleGuide);
            _mapButton.onClick.AddListener(Map);
            _donationAccountsButton.onClick.AddListener(DonationAccounts);
            _auspiciousCalendarButton.onClick.AddListener(AupiciousCalendar);

            _dhammaButton.onClick.AddListener(Dhamma);
            _prayButton.onClick.AddListener(Pray);
            _bookMeditationButton.onClick.AddListener(BookMeditation);
            _manageMembersButton.onClick.AddListener(ManageMembers);
            _settingButton.onClick.AddListener(Setting);
            _rankingButton.onClick.AddListener(Ranking);

            UIRefresher.OnHomeRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
            UIRefresher.OnLocalizeDynamicString += () => _welcomeText.text = _playerHome.GetWelcomeText();
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void TempleHistory() => _playerHome.OnTempleHistoryButtonClick();
        private void AbbotHistory() => _playerHome.OnAbbotHistoryButtonClick();
        private void TempleGuide() => _playerHome.OnTempleGuideButtonClick();
        private void Map() => _playerHome.OnMapButtonClick();
        private void DonationAccounts() => _playerHome.OnDonationAccountsButtonClick();
        private void AupiciousCalendar() => _playerHome.OnAupiciousCalendarButtonClick();

        private void Dhamma() => _playerHome.OnDhammaButtonClick();
        private void Pray() => _playerHome.OnPrayButtonClick();
        private void BookMeditation() => _playerHome.OnBookMeditationButtonClick();
        private void ManageMembers() => _playerHome.OnManageMembersButtonClick();
        private void Setting() => _playerHome.OnSettingButtonClick();
        private void Ranking() => _playerHome.OnRankingButtonClick();

        private void RefreshUI()
        {
            //_coverImage.sprite = _playerHome.GetCoverImage();
            _welcomeText.text = _playerHome.GetWelcomeText();
        }
        #endregion
    }
}