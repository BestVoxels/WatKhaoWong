using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WatKhaoWong.Challenges;

namespace WatKhaoWong.UI.Challenges
{
    public class ChallengeUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Challenge Stuffs")]
        [SerializeField] private Button _challengeCreationButton;
        [SerializeField] private Button _challengePendingButton;
        [Space]
        [SerializeField] private EventTrigger[] _countDownBannerEventTriggers;
        #endregion



        #region --Fields-- (In Class)
        private Challenge _challenge;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _challenge = GameObject.FindWithTag("Player").GetComponentInChildren<Challenge>();

            _challengeCreationButton.onClick.AddListener(ChallengeCreation);
            _challengePendingButton.onClick.AddListener(ChallengePending);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => CountDownBanner((PointerEventData)data));

            foreach (EventTrigger each in _countDownBannerEventTriggers)
                each.triggers.Add(entry);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void ChallengeCreation() => _challenge.OnChallengeCreationButtonClick();
        private void ChallengePending() => _challenge.OnChallengePendingButtonClick();

        private void CountDownBanner(PointerEventData data) => _challenge.OnCountDownBannerClick();
        #endregion
    }
}