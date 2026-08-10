using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Localization;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;
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



        #region --Events-- (Delegate as Action)
        public event Action UserStatsUpdated;
        #endregion



        #region --Fields-- (In Class)
        private NumberFormatInfo _nfi;

        private MyUserData _myUserData;
        private AccommodationSetTimePopup _setTimePopup;
        private SavingWrapper _savingWrapper;
        #endregion



        #region --Properties-- (Auto)
        public AllYearlyStats StayStats { get; set; } = new();
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _setTimePopup = player.GetComponentInChildren<AccommodationSetTimePopup>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void Start()
        {
            _nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            _nfi.NumberGroupSeparator = " ";
        }

        // // For Testing Purpose
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.Space))
        //     {
        //         print($"All Years total days : {StayStats.TotalDays}");
        //         print($"All Years total stays : {StayStats.TotalStays}");
        //         foreach (var each in StayStats.ByYear)
        //         {
        //             print($"-> ByYear.Key (Year) : {each.Key} / ByYear.Value (Total days) : {each.Value.TotalDays} / ByYear.Value (Total stays) : {each.Value.TotalStays}");
        //             foreach (var each2 in each.Value.DaysByMonth)
        //             {
        //                 print($"-> (month) : {each2.Key} / (how many days in this month) : {each2.Value}");
        //             }
        //         }
        //     }
        // }
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

            return _avgText.GetLocalizedString($"{_valueBegin}{result.ToString("#,0.#", _nfi)}{_valueEnd} {_dayText.GetLocalizedString()}{S(result)}");
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



        #region --Methods-- (Custom PUBLIC) ~User Stay Stats~
        public async void UpdateUserStats()
        {
            if (!FirebaseUtils.IsAuthenticated()) return;

            // Past Entry
            IAsyncEnumerable<(StayEntry, string)> pastRows = _savingWrapper.LoadPastEntryFromUser(_myUserData.GetUserKeyID());
            // Active Entry (Pending/Scheduled/Active)
            StayEntry activeStayEntry = await _myUserData.GetActiveStayEntry();

            // Past Entry
            StayStats = new();
            if (pastRows != null)
            {
                await foreach ((StayEntry stayEntry, string keyId) in pastRows)
                {
                    if (stayEntry == null) continue;
                    if (!Enum.TryParse(stayEntry.StatusInfo.Status, true, out EStayStatus eStatus)) continue;
                    if (eStatus != EStayStatus.Completed) continue; // ONLY add data from 'Completed' status since this is PastEntry

                    stayEntry.StayInfo.StartDate.TryParseGregorian(out DateTime startDate);
                    stayEntry.StayInfo.EndDate.TryParseGregorian(out DateTime endDate);
                    AssignDataToStayStats(startDate, endDate);
                }
            }

            // Active Entry (Pending/Scheduled/Active)
            if (activeStayEntry != null)
            {
                activeStayEntry.StayInfo.StartDate.TryParseGregorian(out DateTime startDate2);
                activeStayEntry.StayInfo.EndDate.TryParseGregorian(out DateTime endDate2);
                AssignDataToStayStats(startDate2, endDate2);
            }

            UserStatsUpdated?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~User Stay Stats~
        private void AssignDataToStayStats(DateTime startDate, DateTime endDate)
        {
            // Get Stay Duration
            int totalDays = (endDate == default) ? 1 : (int)_setTimePopup.GetDuration(startDate, endDate).TotalDays; // IF not staying, totalDays is '1'
            // Get what month from StartDate
            int year = startDate.Year;
            // Get what year from StartDate
            int month = startDate.Month;

            // Assign Data
            StayStats.TotalDays += totalDays;
            StayStats.TotalStays++;

            if (!StayStats.ByYear.TryGetValue(year, out YearlyStats yearlyStats))
            {
                yearlyStats = new();
                StayStats.ByYear[year] = yearlyStats;
            }

            yearlyStats.TotalDays += totalDays;
            yearlyStats.TotalStays += 1;
            yearlyStats.DaysByMonth[month] = yearlyStats.DaysByMonth.GetValueOrDefault(month) + totalDays;
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        public class AllYearlyStats
        {
            // All years combined
            public int TotalDays { get; set; }
            public int TotalStays { get; set; }

            // Key = year
            public Dictionary<int, YearlyStats> ByYear { get; } = new();
        }

        public class YearlyStats
        {
            public int TotalDays { get; set; }
            public int TotalStays { get; set; }

            // Key = month (1-12)
            public Dictionary<int, int> DaysByMonth { get; } = new();
        }
        #endregion
    }
}