using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Identity;
using Firebase.Auth;

namespace WatKhaoWong.Homes
{
    public class Home : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Home Stuffs - Welcome Text")]
        [TextArea]
        [SerializeField] private string _welcomeTextForGuest;
        [Space]
        [TextArea]
        [SerializeField] private string _welcomeTextForUser;

        //[Space]
        //[Header("Home Stuffs - Settings")]
        //[SerializeField] private float _coverImageRefreshTime = 99999999f;
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Home UI Event")]
        [SerializeField] private UnityEvent _onHistoryButtonClick;
        [SerializeField] private UnityEvent _onPrayButtonClick;
        [SerializeField] private UnityEvent _onSettingButtonClick;
        #endregion



        #region --Fields-- (In Class)
        private AccountData _accountData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _accountData = player.GetComponentInChildren<AccountData>();
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
            
            if (FirebaseAuth.DefaultInstance.CurrentUser != null)
                text = $"{_welcomeTextForUser}\n{_accountData.GetUserNameText()}";
            else
                text = _welcomeTextForGuest;

            return text;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnHistoryButtonClick()
        {
            Debug.Log("Click \"History\" Button!");

            _onHistoryButtonClick?.Invoke();
        }

        public void OnPrayButtonClick()
        {
            Debug.Log("Click \"Pray\" Button!");

            _onPrayButtonClick?.Invoke();
        }

        public void OnSettingButtonClick()
        {
            Debug.Log("Click \"Setting\" Button!");

            _onSettingButtonClick?.Invoke();
        }
        #endregion
    }
}