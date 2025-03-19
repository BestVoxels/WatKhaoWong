using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;
using WatKhaoWong.Settings;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Prays
{
    public class Pray : Page
    {
        #region --Fields-- (Inspector)
        [Header("Sound - Settings")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClipTM;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Pray Text")]
        [field: SerializeField] public LocalizedString AllTimeText { get; private set; }
        [field: SerializeField] public LocalizedString TodayText { get; private set; }
        [field: SerializeField] public LocalizedString ChallengeText { get; private set; }

        [field: Space]

        [field: Header("Pray - Settings")]
        [field: SerializeField] public string ValueTextFormatBegin { get; private set; } = "<space=25><b><cspace=-3>";
        [field: SerializeField] public string ValueTextFormatEnd { get; private set; } = "</cspace></b>";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Pray UI Event")]
        [SerializeField] private UnityEvent _onUserProfileClick;
        [SerializeField] private UnityEvent _onUserStatsClick;
        [Space]
        [SerializeField] private UnityEvent _onRecordManuallyButtonClick;
        [SerializeField] private UnityEvent _onPlaySoundButtonClick;
        [SerializeField] private UnityEvent _onPlaySoundButtonClickIfGuest;
        [SerializeField] private UnityEvent _onContinueButtonClick;
        [SerializeField] private UnityEvent _onPauseSoundButtonClick;
        [SerializeField] private UnityEvent _onEndSoundButtonClick;
        #endregion



        #region --Properties-- (Computed)
        public bool IsPlayingSound => _audioSource.isPlaying;
        public bool ToStartMeditateText => _audioSource.time <= 0f && _tmCounter == 0;
        #endregion



        #region --Fields-- (In Class)
        private DateTime _startPlayTime = default;
        private DateTime _startPauseTime = default;
        private double _totalPausedTime = 0d;
        private int _tmCounter = 0;

        private MyUserData _myUserData;
        private Setting _playerSetting;
        private ServerTime _serverTime;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _myUserData = player.GetComponentInChildren<MyUserData>();
            _playerSetting = player.GetComponentInChildren<Setting>();
            _serverTime = FindAnyObjectByType<ServerTime>();
        }

        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnUserProfileClick()
        {
            _onUserProfileClick?.Invoke();
        }

        public void OnUserStatsClick()
        {
            _onUserStatsClick?.Invoke();
        }

        public void OnRecordManuallyButtonClick()
        {
            _onRecordManuallyButtonClick?.Invoke();
        }

        public async void OnPlaySoundButtonClick()
        {
            if (_myUserData.GetRole() == EUserRole.Guest)
            {
                _onPlaySoundButtonClickIfGuest?.Invoke();
                return;
            }

            _onPlaySoundButtonClick?.Invoke();

            await PlayTMClipLooping();
        }

        public async void OnContinueButtonClick()
        {
            _onContinueButtonClick?.Invoke();

            await ContinueTMClip();
        }

        public async void OnPauseSoundButtonClick()
        {
            _onPauseSoundButtonClick?.Invoke();

            await PauseTMClip();
        }

        public async void OnEndSoundButtonClick()
        {
            _onEndSoundButtonClick?.Invoke();

            await EndTMClip();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async Task<int> GetLoopCount()
        {
            _totalPausedTime += await GetPauseTime();

            double playedTime = ((await _serverTime.Now()) - _startPlayTime).TotalSeconds;
            double actualPlayedTime = playedTime - _totalPausedTime;
            decimal dividedResult = (decimal)(actualPlayedTime / _audioClipTM.length);

            if (dividedResult.IsNegative()) return 0; // If the logic is correct this line won't ever need to execute, because _startPlayTime will always bigger than _startPauseTime since it starts first.

            _totalPausedTime = 0d;
            _startPauseTime = default;

            return (int)Math.Floor(dividedResult);
        }

        private async Task<double> GetPauseTime()
        {
            if (_startPauseTime == default) return 0d;

            double pausedTime = ((await _serverTime.Now()) - _startPauseTime).TotalSeconds;

            _startPauseTime = default;

            return pausedTime;
        }

        private async Task PlayTMClipLooping()
        {
            _audioSource.loop = true;
            _audioSource.clip = _audioClipTM;
            _audioSource.volume = _playerSetting.LoadMusicValue();
            _audioSource.Play();

            _startPlayTime = await _serverTime.Now();
        }

        private async Task ContinueTMClip()
        {
            _audioSource.Play();

            _totalPausedTime += await GetPauseTime();
        }

        private async Task PauseTMClip()
        {
            _audioSource.Pause();

            _startPauseTime = await _serverTime.Now();
        }

        private async Task EndTMClip()
        {
            _audioSource.loop = false;
            _audioSource.clip = null;
            _audioSource.Stop();

            _tmCounter = await GetLoopCount();

            UploadToServer();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Upload to Server Stuffs~
        private void UploadToServer()
        {
            _myUserData.AddTMPoints(_tmCounter);

            _tmCounter = 0;
        }
        #endregion
    }
}