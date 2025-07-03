using System;
using UnityEngine.Localization;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

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
        [SerializeField] private UnityEvent _onSubmitInfoButtonClick;
        [SerializeField] private UnityEvent _onStayFormButtonClick;
        [SerializeField] private UnityEvent _onMyInfoButtonClick;
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

            return $"{_valueBegin}{days}{_valueEnd} {_dayText.GetLocalizedString()}{S(days)}";
        }

        public string VisitsString(int visits)
        {
            if (visits < 0)
                return $"??? {_visitText.GetLocalizedString()}";

            return $"{_valueBegin}{visits}{_valueEnd} {_visitText.GetLocalizedString()}{S(visits)}";
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

            return _avgText.GetLocalizedString($"{_valueBegin}{result}{_valueEnd} {_dayText.GetLocalizedString()}{S(result)}");
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
            _onSubmitInfoButtonClick?.Invoke();
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