using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;
using WatKhaoWong.Identities;
using WatKhaoWong.Attributes;
using ChartAndGraph;
using System;
using System.Collections.Generic;

namespace WatKhaoWong.UI.Retreats
{
    public class MeditationRetreatUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        [Header("MeditationRetreat UI Stuffs")]
        [SerializeField] private BarChart _bar;
        [SerializeField] private TMP_Text _barInfoText;
        [Space]
        [SerializeField] private TMP_Text _yearlyHeaderText;
        [SerializeField] private TMP_Text _yearlyDaysText;
        [SerializeField] private TMP_Text _yearlyVisitsText;
        [SerializeField] private TMP_Text _yearlyAvgText;
        [Space]
        [SerializeField] private TMP_Text _totalDaysText;
        [SerializeField] private TMP_Text _totalVisitsText;
        [SerializeField] private TMP_Text _totalAvgText;
        [Space]
        [SerializeField] private Button _submitInfoButton;
        [SerializeField] private Button _stayFormButton;
        [SerializeField] private Button _myInfoButton;
        #endregion



        #region --Fields-- (In Class)
        private MeditationRetreat _meditationRetreat;
        private MyUserData _myUserData;
        private ServerTime _serverTime;
        #endregion



        #region --Fields-- (Constant)
        private const byte MinimumValueForBarChart = 1;
        private const float WaitUIToTurnOffOnStartTime = 3.5f;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _meditationRetreat = player.GetComponentInChildren<MeditationRetreat>();
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _serverTime = FindAnyObjectByType<ServerTime>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            _bar.BarHovered.AddListener(BarHovered);
            _bar.NonHovered.AddListener(NonHovered);

            _submitInfoButton.onClick.AddListener(SubmitInfo);
            _stayFormButton.onClick.AddListener(StayForm);
            _myInfoButton.onClick.AddListener(MyInfo);

            UIRefresher.OnMeditationRetreatRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
            _meditationRetreat.UserStatsUpdated += RefreshStatUI;

            UIRefresher.OnLocalizeDynamicString += RefreshStatUI;
        }

        private void Start()
        {
            _meditationRetreat.UpdateUserStats();

            RefreshUI();
        }

        private void OnEnable()
        {
            if (Time.time < WaitUIToTurnOffOnStartTime) return; // Prevent OnEnable() on first Start when UI are seting itself which then it will hide itself. We only want OnEnable() when user open UI.

            _meditationRetreat.UpdateUserStats();

            RefreshUI(); // To Make Graph Value not reduced by 'BarAnimation.cs'
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ShowHideButtons()
        {
            // Submit Info Button
            _submitInfoButton.gameObject.SetActive(!_myUserData.IsGeneralInfoExists());

            // Stay Form Button & My Info Button
            _stayFormButton.gameObject.SetActive(_myUserData.IsGeneralInfoExists());
            _myInfoButton.gameObject.SetActive(_myUserData.IsGeneralInfoExists());
            // "_myUserData.GetTempleGuideConfirmed()" FYI code that can be checks incase need to...
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _meditationRetreat.OnBackButtonClick();
        private void ChangeLang() => _meditationRetreat.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void BarHovered(BarChart.BarEventArgs args)
        {
            _barInfoText.text = _meditationRetreat.DaysString(((int)args.Value) - MinimumValueForBarChart);

            _meditationRetreat.OnBarHovered();
        }

        private void NonHovered()
        {
            _barInfoText.text = string.Empty;

            _meditationRetreat.OnNonHovered();
        }

        private void SubmitInfo()
        {
            _meditationRetreat.OnSubmitInfoButtonClick();
        }

        private void StayForm()
        {
            _meditationRetreat.OnStayFormButtonClick();
        }

        private void MyInfo()
        {
            _meditationRetreat.OnMyInfoButtonClick();
        }

        private void RefreshUI()
        {
            ShowHideButtons();

            RefreshStatUI();
        }

        private void RefreshStatUI()
        {
            int currentYear = _serverTime.NowSinceAppStart().Year;
            var thisYear = _meditationRetreat.StayStats.ByYear.GetValueOrDefault(currentYear);

            // Bar
            _barInfoText.text = string.Empty; // Reset

            for (byte i = 0; i < 12; i++)
            {
                int days = 0;
                if (thisYear != null && thisYear.DaysByMonth.TryGetValue(i + 1, out int daysInMonth))
                {
                    days = daysInMonth;
                }
                _bar.DataSource.SetValue(_bar.DataSource.GetCategoryName(i), _bar.DataSource.GetGroupName(0), days + MinimumValueForBarChart);
            }

            // Texts
            _totalDaysText.text = _meditationRetreat.BulletString() + _meditationRetreat.DaysString(_meditationRetreat.StayStats.TotalDays);
            _totalVisitsText.text = _meditationRetreat.BulletString() + _meditationRetreat.VisitsString(_meditationRetreat.StayStats.TotalStays);
            _totalAvgText.text = _meditationRetreat.BulletString() + _meditationRetreat.AvgString(_meditationRetreat.StayStats.TotalDays, _meditationRetreat.StayStats.TotalStays);

            _yearlyDaysText.text = _meditationRetreat.BulletString() + _meditationRetreat.DaysString(thisYear == null ? 0 : thisYear.TotalDays);
            _yearlyVisitsText.text = _meditationRetreat.BulletString() + _meditationRetreat.VisitsString(thisYear == null ? 0 : thisYear.TotalStays);
            _yearlyAvgText.text = _meditationRetreat.BulletString() + _meditationRetreat.AvgString(thisYear == null ? 0 : thisYear.TotalDays, thisYear == null ? 0 : thisYear.TotalStays);

            _yearlyHeaderText.text = _meditationRetreat.YearlyHeaderString(currentYear);
        }
        #endregion
    }
}