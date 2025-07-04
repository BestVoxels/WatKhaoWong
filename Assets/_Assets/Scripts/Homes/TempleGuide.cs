using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Homes
{
    public class TempleGuide : Page
    {
        #region --Fields-- (Inspector)
        #endregion



        #region --Events-- (UnityEvent)
        [Header("TempleGuide UI Event - while showing Consent")]
        [SerializeField] private UnityEvent _onBackButtonClickWithConsent;
        [SerializeField] private UnityEvent _onSubmitInfoButtonClick;
        #endregion



        #region --Fields-- (In Class)
        #endregion



        #region --Properties-- (Auto)
        public static bool ShowConsent { get; set; } = false;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnBackButtonClickWithConsent()
        {
            _onBackButtonClickWithConsent?.Invoke();
        }

        public void OnSubmitInfoButtonClick()
        {
            _onSubmitInfoButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void SetShowConsentToTrue()
        {
            ShowConsent = true;
        }
        #endregion
    }
}