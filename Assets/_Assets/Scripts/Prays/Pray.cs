using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identities;

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

        [field: SerializeField] public LocalizedString MeditateText { get; private set; }
        [field: SerializeField] public LocalizedString ContinueText { get; private set; }

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
        [SerializeField] private UnityEvent<string> _onMeditateAndUploadScoreSucceeded;
        [SerializeField] private UnityEvent _onMeditateAndUploadScoreFailed;
        [Space]
        [SerializeField] private UnityEvent _onRecordManuallyButtonClick;
        [SerializeField] private UnityEvent _onPlaySoundButtonClick;
        [SerializeField] private UnityEvent _onPauseSoundButtonClick;
        [SerializeField] private UnityEvent _onEndSoundButtonClick;
        #endregion



        #region --Properties-- (Computed)
        public bool IsPlayingSound => _audioSource.isPlaying;
        public bool CanUploadToServer => _tmCounter > 0;
        public bool ToStartMeditateText => _audioSource.time <= 0f && _tmCounter == 0;
        #endregion



        #region --Fields-- (In Class)
        private int _tmCounter = 0;

        private bool _isAdded = false;
        private MyUserData _myUserData;
        #endregion



        #region --Fields-- (Constant)
        private const byte MarginForCompareTime = 1;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
        }

        private void Update()
        {
            if (!IsPlayingSound) return;

            if (_audioSource.time >= _audioClipTM.length - MarginForCompareTime && _isAdded == false)
            {
                ++_tmCounter;

                _isAdded = true;
            }

            if (_audioSource.time <= 0f + MarginForCompareTime)
            {
                _isAdded = false;
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
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

        public void OnPlaySoundButtonClick()
        {
            _onPlaySoundButtonClick?.Invoke();

            PlayTMClipLooping();
        }

        public void OnPauseSoundButtonClick()
        {
            _onPauseSoundButtonClick?.Invoke();

            PauseTMClip();
        }

        public void OnEndSoundButtonClick()
        {
            _onEndSoundButtonClick?.Invoke();

            EndTMClip();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void PlayTMClipLooping()
        {
            _audioSource.loop = true;

            _audioSource.clip = _audioClipTM;

            _audioSource.Play();
        }

        private void PauseTMClip()
        {
            _audioSource.Pause();
        }

        private void EndTMClip()
        {
            _audioSource.loop = false;

            _audioSource.clip = null;

            _audioSource.Stop();

            if (CanUploadToServer)
                UploadToServerSucceeded();
            else
                UploadToServerFailed();
        }

        private void UploadToServerSucceeded()
        {
            _myUserData.AddTotalTMPoints(_tmCounter);
            _myUserData.AddTodayTMPoints(_tmCounter);
            _myUserData.AddChallengeTMPointsText(_tmCounter);

            _onMeditateAndUploadScoreSucceeded?.Invoke(_tmCounter.ToString());

            _tmCounter = 0;
        }

        private void UploadToServerFailed()
        {
            _onMeditateAndUploadScoreFailed?.Invoke();
        }
        #endregion
    }
}