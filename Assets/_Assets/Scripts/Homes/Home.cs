using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Identity;
using WatKhaoWong.Utils.Core;

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
        private MyUserData _myUserData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");

            _myUserData = player.GetComponentInChildren<MyUserData>();
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
            
            if (FirebaseUtils.IsAuthenticated())
                text = $"{_welcomeTextForUser}\n{_myUserData.GetUserNameText()}";
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