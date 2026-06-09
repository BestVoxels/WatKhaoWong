using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Identities
{
    public class OtherAccountPopup : Popup
    {
        #region --Events-- (UnityEvent)
        [Header("Account Popup UI Event")]
        [SerializeField] private UnityEvent _onUserProfileButtonClick;
        #endregion



        #region --Fields-- (In Class)
        #endregion



        #region --Methods-- (Built In)
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnUserProfileButtonClick()
        {
            _onUserProfileButtonClick?.Invoke();
        }
        #endregion
    }
}