using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Prays
{
    public class Pray : Page
    {
        #region --Events-- (UnityEvent)
        [Header("Pray UI Event")]
        [SerializeField] private UnityEvent _onDoneButtonClick;
        [SerializeField] private UnityEvent _onPlaySoundButtonClick;
        [SerializeField] private UnityEvent _onChallengeButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~User~
        public string GetUsernameText()
        {
            // TODO fetch data from Server
            return "Thanitsak Leuangsupornpong";
        }

        public int GetAllTimePoints()
        {
            // TODO fetch data from Server
            return 0;
        }

        public int GetTodayPoints()
        {
            // TODO fetch data from Server
            return 0;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Challenge~
        public string GetChallengeText()
        {
            return "No Challenge Avaiable";
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnDoneButtonClick()
        {
            Debug.LogWarning("Click \"Done\" Button!");

            _onDoneButtonClick?.Invoke();
        }

        public void OnPlaySoundButtonClick()
        {
            Debug.LogWarning("Click \"Sound\" Button!");

            _onPlaySoundButtonClick?.Invoke();
        }

        public void OnChallengeButtonClick()
        {
            Debug.LogWarning("Click \"Start Challenge\" Button!");

            _onChallengeButtonClick?.Invoke();
        }
        #endregion
    }
}