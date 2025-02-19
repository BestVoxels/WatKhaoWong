using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Homes
{
    public class DhammaPopup : Popup
    {
        #region --Events-- (UnityEvent)
        [Header("Dhamma Popup UI Event")]
        [SerializeField] private UnityEvent _onFacebook1ButtonClick;
        [SerializeField] private UnityEvent _onFacebook2ButtonClick;
        [SerializeField] private UnityEvent _onFacebook3ButtonClick;
        [SerializeField] private UnityEvent _onFacebook4ButtonClick;

        [SerializeField] private UnityEvent _onYoutube1ButtonClick;

        [SerializeField] private UnityEvent _onTiktok1ButtonClick;

        [SerializeField] private UnityEvent _onLine1ButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnFacebook1ButtonClick()
        {
            _onFacebook1ButtonClick?.Invoke();
        }

        public void OnFacebook2ButtonClick()
        {
            _onFacebook2ButtonClick?.Invoke();
        }

        public void OnFacebook3ButtonClick()
        {
            _onFacebook3ButtonClick?.Invoke();
        }

        public void OnFacebook4ButtonClick()
        {
            _onFacebook4ButtonClick?.Invoke();
        }


        public void OnYoutube1ButtonClick()
        {
            _onYoutube1ButtonClick?.Invoke();
        }

        public void OnTiktok1ButtonClick()
        {
            _onTiktok1ButtonClick?.Invoke();
        }

        public void OnLine1ButtonClick()
        {
            _onLine1ButtonClick?.Invoke();
        }
        #endregion
    }
}