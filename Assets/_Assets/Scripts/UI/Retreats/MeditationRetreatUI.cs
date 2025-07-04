using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;
using WatKhaoWong.Identities;
using WatKhaoWong.Attributes;
using ChartAndGraph;
using System;

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



        #region --Methods-- (Built In)
        private void Awake()
        {
            _meditationRetreat = GameObject.FindWithTag("Player").GetComponentInChildren<MeditationRetreat>();
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
            _serverTime = FindAnyObjectByType<ServerTime>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            _bar.BarHovered.AddListener(BarHovered);
            _bar.NonHovered.AddListener(NonHovered);

            _submitInfoButton.onClick.AddListener(SubmitInfo);
            _stayFormButton.onClick.AddListener(StayForm);
            _myInfoButton.onClick.AddListener(MyInfo);

            UIRefresher.OnMeditationRetreatRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.

            UIRefresher.OnLocalizeDynamicString += RefreshStatUI;
        }

        private void OnEnable()
        {
            RefreshUI(); // To Make Graph Value not reduced by 'BarAnimation.cs'
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _meditationRetreat.OnBackButtonClick();
        private void ChangeLang() => _meditationRetreat.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void BarHovered(BarChart.BarEventArgs args)
        {
            _barInfoText.text = _meditationRetreat.DaysString((int)args.Value);

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
            // Reset
            _barInfoText.text = string.Empty;

            // Update Bar Chart
            byte[] monthsValue = new byte[12] { 3, 8, 0, 13, 20, 0, 0, 7, 14, 21, 0, 3 }; // TODO Get Value from '_myUserData' (From Server).

            for (byte i = 0; i < 12; i++)
            {
                _bar.DataSource.SetValue(_bar.DataSource.GetCategoryName(i), _bar.DataSource.GetGroupName(0), monthsValue[i]);
            }

            RefreshStatUI();
        }

        private async void RefreshStatUI()
        {
            _totalDaysText.text = _meditationRetreat.BulletString() + _meditationRetreat.DaysString(_myUserData.GetTotalTMPoints());
            _totalVisitsText.text = _meditationRetreat.BulletString() + _meditationRetreat.VisitsString(_myUserData.GetTodayTMPoints());
            _totalAvgText.text = _meditationRetreat.BulletString() + _meditationRetreat.AvgString(_myUserData.GetTotalTMPoints(), _myUserData.GetTodayTMPoints());

            _yearlyDaysText.text = _meditationRetreat.BulletString() + _meditationRetreat.DaysString(_myUserData.GetTotalTMPoints());
            _yearlyVisitsText.text = _meditationRetreat.BulletString() + _meditationRetreat.VisitsString(_myUserData.GetTodayTMPoints());
            _yearlyAvgText.text = _meditationRetreat.BulletString() + _meditationRetreat.AvgString(_myUserData.GetTotalTMPoints(), _myUserData.GetTodayTMPoints());

            DateTime nowDate = await _serverTime.Now();
            _yearlyHeaderText.text = _meditationRetreat.YearlyHeaderString(nowDate.Year);
        }
        #endregion
    }
}