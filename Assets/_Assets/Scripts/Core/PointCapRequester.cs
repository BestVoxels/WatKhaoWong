using UnityEngine;
using UnityEngine.Localization;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Core
{
    public class PointCapRequester : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Point Cap Requester - Status Text")]
        [Space]
        [SerializeField] private LocalizedString _statusRequestSucceeded;
        [SerializeField] private Color32 _statusRequestSucceededColor;
        [Space]
        [SerializeField] private LocalizedString _statusRequestFailedAlreadySent;
        [SerializeField] private Color32 _statusRequestFailedAlreadySentColor;
        #endregion



        #region --Fields-- (In Class)
        private MyUserData _myUserData;
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
            _statusText = FindAnyObjectByType<StatusText>();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public async void RequestCapIncrease()
        {
            bool isMyUserDataSaveLoaded = await MyUserData.LoadCompletionSource.Task;

            if (isMyUserDataSaveLoaded == false)
            {
                Debug.LogError("Could not continue RequestCapIncrease() on PointCapRequester.cs because MyUserData.cs LoadSave() is not completed.");
                return;
            }

            if (_myUserData.IncrementTMPointCapRequest())
            {
                _statusText.Show(_statusRequestSucceeded.GetLocalizedString(), _statusRequestSucceededColor);
            }
            else
            {
                _statusText.Show(_statusRequestFailedAlreadySent.GetLocalizedString(), _statusRequestFailedAlreadySentColor);
            }
        }
        #endregion
    }
}