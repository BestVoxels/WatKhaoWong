using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Settings
{
    public class Setting : Page
    {
        #region --Fields-- (Inspector)
        [Header("Setting Stuffs")]
        [Range(0f, 1f)]
        [SerializeField] private float _defaultSfxSliderValue;
        [Range(0f, 1f)]
        [SerializeField] private float _defaultMusicSliderValue;
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Setting UI Event")]
        [SerializeField] private UnityEvent _onAccountButtonClick;
        [SerializeField] private UnityEvent _onNotificationButtonClick;
        [SerializeField] private UnityEvent _onLanguageButtonClick;
        [SerializeField] private UnityEvent _onSupportButtonClick;
        [SerializeField] private UnityEvent _onCreditsButtonClick;
        #endregion



        #region --Fields-- (Constant)
        private const string KeySfxSliderValue = "SfxSliderValue";
        private const string KeyMusicSliderValue = "MusicSliderValue";
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Sliders~
        public float LoadSfxValue() => PlayerPrefs.GetFloat(KeySfxSliderValue, _defaultSfxSliderValue); // return _defaultValue IF 'Key' doesn't exist.
        public float LoadMusicValue() => PlayerPrefs.GetFloat(KeyMusicSliderValue, _defaultMusicSliderValue); // return _defaultValue IF 'Key' doesn't exist.

        public void SaveSfxValue(float value) => PlayerPrefs.SetFloat(KeySfxSliderValue, value);
        public void SaveMusicValue(float value) => PlayerPrefs.SetFloat(KeyMusicSliderValue, value);
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnAccountButtonClick()
        {
            Debug.Log("Click \"Account\" Button!");

            _onAccountButtonClick?.Invoke();
        }

        public void OnNotificationButtonClick()
        {
            Debug.Log("Click \"Notification\" Button!");

            _onNotificationButtonClick?.Invoke();
        }

        public void OnLanguageButtonClick()
        {
            Debug.Log("Click \"Language\" Button!");

            _onLanguageButtonClick?.Invoke();
        }

        public void OnSupportButtonClick()
        {
            Debug.Log("Click \"Support\" Button!");

            _onSupportButtonClick?.Invoke();
        }

        public void OnCreditsButtonClick()
        {
            Debug.Log("Click \"Credits\" Button!");

            _onCreditsButtonClick?.Invoke();
        }
        #endregion
    }
}