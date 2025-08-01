using System;
using UnityEngine.Localization;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using System.Globalization;

namespace WatKhaoWong.Retreats
{
    public class MeditationRetreat : Page
    {
        #region --Fields-- (Inspector)
        [Header("Meditation Retreat Text")]
        [SerializeField] private LocalizedString _yearlyHeaderText;
        [SerializeField] private LocalizedString _dayText;
        [SerializeField] private LocalizedString _visitText;
        [SerializeField] private LocalizedString _avgText;
        [SerializeField] private LocalizedString _sText;

        [Space]

        [Header("Meditation Retreat - Settings")]
        [SerializeField] private string _bulletBegin = "•<space=50>";
        [SerializeField] private string _bulletEnd = "";

        [SerializeField] private string _valueBegin = "<b><cspace=-3>";
        [SerializeField] private string _valueEnd = "</cspace></b>";
        #endregion



        #region --Properties-- (Inspector)
        #endregion



        #region --Events-- (UnityEvent)
        [Header("MeditationRetreat UI Event")]
        [SerializeField] private UnityEvent _onBarHovered;
        [SerializeField] private UnityEvent _onNonHovered;
        [Space]
        [SerializeField] private UnityEvent _onSubmitInfoButtonClickTGConfirmed;
        [SerializeField] private UnityEvent _onSubmitInfoButtonClickTGNotConfirmed;
        [SerializeField] private UnityEvent _onStayFormButtonClick;
        [SerializeField] private UnityEvent _onMyInfoButtonClick;
        #endregion



        #region --Fields-- (In Class)
        private NumberFormatInfo _nfi;

        private MyUserData _myUserData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
        }

        private void Start()
        {
            _nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            _nfi.NumberGroupSeparator = " ";
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Displaying~
        public string BulletString() => _bulletBegin + _bulletEnd;

        public string YearlyHeaderString(int years)
        {
            if (years < 0)
                return _yearlyHeaderText.GetLocalizedString("???");

            return _yearlyHeaderText.GetLocalizedString(years);
        }

        public string DaysString(int days)
        {
            if (days < 0)
                return $"??? {_dayText.GetLocalizedString()}";

            return $"{_valueBegin}{days.ToString("#,0", _nfi)}{_valueEnd} {_dayText.GetLocalizedString()}{S(days)}";
        }

        public string VisitsString(int visits)
        {
            if (visits < 0)
                return $"??? {_visitText.GetLocalizedString()}";

            return $"{_valueBegin}{visits.ToString("#,0", _nfi)}{_valueEnd} {_visitText.GetLocalizedString()}{S(visits)}";
        }

        public string AvgString(int days, int visits)
        {
            if (days < 0 || visits < 0)
                return _avgText.GetLocalizedString("???");

            decimal result = 0;
            if (days != 0 && visits != 0)
            {
                decimal avg = (decimal)days / visits;

                result = Math.Round(avg, 1, MidpointRounding.AwayFromZero);
            }

            return _avgText.GetLocalizedString($"{_valueBegin}{result.ToString("#,0", _nfi)}{_valueEnd} {_dayText.GetLocalizedString()}{S(result)}");
        }

        public string S(decimal input) => input > 1 ? _sText.GetLocalizedString() : "";
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnBarHovered()
        {
            _onBarHovered?.Invoke();
        }

        public void OnNonHovered()
        {
            _onNonHovered?.Invoke();
        }

        public void OnSubmitInfoButtonClick()
        {
            if (_myUserData.GetTempleGuideConfirmed())
            {
                // Check if "Consent is Ticked" -> Trigger Event "OnConsentTick" [This Event will trigger 'Submit Info' Page UI]
                _onSubmitInfoButtonClickTGConfirmed?.Invoke();
                return;
            }

            // Check if "Consent has not yet Ticked" -> Trigger Event "OnConsentIsNotTick" [This Event will trigger 'Read Temple Guide' Popup UI]
            _onSubmitInfoButtonClickTGNotConfirmed?.Invoke();
        }

        public void OnStayFormButtonClick()
        {
            _onStayFormButtonClick?.Invoke();
        }

        public void OnMyInfoButtonClick()
        {
            _onMyInfoButtonClick?.Invoke();
        }
        #endregion
    }
}