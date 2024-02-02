using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Homes
{
    public class Home : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        //[Header("Home Stuffs")]
        // TODO CoverImage refresh time
        // TODO about Welcome Text maybe?
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Home UI Event")]
        [SerializeField] private UnityEvent _onHistoryButtonClick;
        [SerializeField] private UnityEvent _onPrayButtonClick;
        [SerializeField] private UnityEvent _onSettingButtonClick;
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
            // TODO get username from somewhere, maybe central script???

            return "Welcome back !!! (username)";
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