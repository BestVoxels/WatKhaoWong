using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Challenges
{
    public class ChallengePendingPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Challenge Pending Popup - Status Text")]
        [SerializeField] private LocalizedString _statusDeleteSucceeded;
        [SerializeField] private Color32 _statusDeleteSucceededColor;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Challenge Pending Popup - Settings")]
        [field: SerializeField] public string DateStringFormat { get; private set; } = "dddd, MMMM d, yyyy\nHH:mm";
        [field: Space]
        [field: SerializeField] public LocalizedString StartDateFormat { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString EndDateFormat { get; private set; }
        [field: Space]
        [field: SerializeField] public LocalizedString DurationFormat { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Challenge Pending Popup UI Event")]
        [SerializeField] private UnityEvent _onDeleteButtonClick;
        #endregion



        #region --Fields-- (In Class)
        private StatusText _statusText;
        private Challenge _challenge;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _challenge = GameObject.FindWithTag("Player").GetComponentInChildren<Challenge>();
            _statusText = FindAnyObjectByType<StatusText>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnDeleteButtonClick()
        {
            _onDeleteButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void DeleteChallenge()
        {
            _statusText.Show(_statusDeleteSucceeded.GetLocalizedString(), _statusDeleteSucceededColor);

            _challenge.DeletePendingChallenge();
        }
        #endregion
    }
}