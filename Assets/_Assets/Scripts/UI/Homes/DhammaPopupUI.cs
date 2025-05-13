using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Homes;

namespace WatKhaoWong.UI.Homes
{
    public class DhammaPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Dhamma Popup UI Stuffs")]
        [SerializeField] private Button _facebook1Button;
        [SerializeField] private Button _facebook2Button;
        [SerializeField] private Button _facebook3Button;
        [SerializeField] private Button _facebook4Button;

        [SerializeField] private Button _instagram1Button;

        [SerializeField] private Button _youtube1Button;

        [SerializeField] private Button _tiktok1Button;

        [SerializeField] private Button _line1Button;
        #endregion



        #region --Fields-- (In Class)
        private DhammaPopup _playerDhammaPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerDhammaPopup = GameObject.FindWithTag("Player").GetComponentInChildren<DhammaPopup>();

            _closeButton.onClick.AddListener(Close);

            _facebook1Button.onClick.AddListener(Facebook1Button);
            _facebook2Button.onClick.AddListener(Facebook2Button);
            _facebook3Button.onClick.AddListener(Facebook3Button);
            _facebook4Button.onClick.AddListener(Facebook4Button);

            _instagram1Button.onClick.AddListener(Instagram1Button);

            _youtube1Button.onClick.AddListener(Youtube1Button);
            _tiktok1Button.onClick.AddListener(Tiktok1Button);
            _line1Button.onClick.AddListener(Line1Button);
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerDhammaPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Facebook1Button()
        {
            _playerDhammaPopup.OnFacebook1ButtonClick();
        }

        private void Facebook2Button()
        {
            _playerDhammaPopup.OnFacebook2ButtonClick();
        }

        private void Facebook3Button()
        {
            _playerDhammaPopup.OnFacebook3ButtonClick();
        }

        private void Facebook4Button()
        {
            _playerDhammaPopup.OnFacebook4ButtonClick();
        }


        private void Instagram1Button()
        {
            _playerDhammaPopup.OnInstagram1ButtonClick();
        }

        private void Youtube1Button()
        {
            _playerDhammaPopup.OnYoutube1ButtonClick();
        }

        private void Tiktok1Button()
        {
            _playerDhammaPopup.OnTiktok1ButtonClick();
        }

        private void Line1Button()
        {
            _playerDhammaPopup.OnLine1ButtonClick();
        }
        #endregion
    }
}