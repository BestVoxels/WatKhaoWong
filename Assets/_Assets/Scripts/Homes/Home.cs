using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Homes
{
    public class Home : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Home Stuffs - Welcome Text")]
        [SerializeField] private LocalizedString _welcomeTextForUser;
        [Space]
        [SerializeField] private LocalizedString _welcomeTextForGuest;

        [Space]
        [SerializeField] private LocalizedString _loading;

        //[Space]
        //[Header("Home Stuffs - Settings")]
        //[SerializeField] private float _coverImageRefreshTime = 99999999f;
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Home UI Event")]
        [SerializeField] private UnityEvent _onTempleHistoryButtonClick;
        [SerializeField] private UnityEvent _onAbbotHistoryButtonClick;
        [SerializeField] private UnityEvent _onTempleGuideButtonClick;
        [SerializeField] private UnityEvent _onMapButtonClick;
        [SerializeField] private UnityEvent _onDonationAccountsButtonClick;
        [SerializeField] private UnityEvent _onAupiciousCalendarButtonClick;
        [Space]
        [SerializeField] private UnityEvent _onDhammaButtonClick;
        [SerializeField] private UnityEvent _onPrayButtonClick;
        [SerializeField] private UnityEvent _onBookMeditationButtonClick;
        [SerializeField] private UnityEvent _onSettingButtonClick;
        [SerializeField] private UnityEvent _onRankingButtonClick;
        #endregion



        #region --Fields-- (In Class)
        private MyUserData _myUserData;
        private TitleLocalizer _titleLocalizer;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _myUserData = player.GetComponentInChildren<MyUserData>();
            _titleLocalizer = FindAnyObjectByType<TitleLocalizer>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Cover Image~
        public Sprite GetCoverImage()
        {
            // TODO create CoverImage changer system, need an event to invoke() in this class, AND UIRefresher.cs need to subscribe to this home class.
            return null;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Welcome Text~
        public string GetWelcomeText()
        {
            string text;

            if (FirebaseUtils.IsAuthenticated() && _myUserData.IsLoadingFromServer == true)
                text = _welcomeTextForUser.GetLocalizedString(_loading.GetLocalizedString());
            else if (FirebaseUtils.IsAuthenticated())
                text = _welcomeTextForUser.GetLocalizedString( $"{_titleLocalizer.Localize(_myUserData.GetTitleText())}\n{_myUserData.GetUserNameText()}" );
            else
                text = _welcomeTextForGuest.GetLocalizedString();

            return text;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnTempleHistoryButtonClick()
        {
            _onTempleHistoryButtonClick?.Invoke();
        }

        public void OnAbbotHistoryButtonClick()
        {
            _onAbbotHistoryButtonClick?.Invoke();
        }

        public void OnTempleGuideButtonClick()
        {
            _onTempleGuideButtonClick?.Invoke();
        }

        public void OnMapButtonClick()
        {
            _onMapButtonClick?.Invoke();
        }

        public void OnDonationAccountsButtonClick()
        {
            _onDonationAccountsButtonClick?.Invoke();
        }

        public void OnAupiciousCalendarButtonClick()
        {
            _onAupiciousCalendarButtonClick?.Invoke();
        }


        public void OnDhammaButtonClick()
        {
            _onDhammaButtonClick?.Invoke();
        }

        public void OnPrayButtonClick()
        {
            _onPrayButtonClick?.Invoke();
        }

        public void OnBookMeditationButtonClick()
        {
            _onBookMeditationButtonClick?.Invoke();
        }

        public void OnSettingButtonClick()
        {
            _onSettingButtonClick?.Invoke();
        }

        public void OnRankingButtonClick()
        {
            _onRankingButtonClick?.Invoke();
        }
        #endregion
    }
}