using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.SharePopup
{
    public class LogoutButton : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Logout Button UI Event")]
        [SerializeField] private UnityEvent _onLogoutButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~UI Button~
        public void OnLogoutButtonClick()
        {
            Debug.LogWarning("Click \"Logout\" Button!");

            // TODO show iOS/Android Popup Window - do like Facebook, Shopee, Lazada.

            // TODO deals with Logout stuff, reset Profile Image to default, and all the values back to default!

            _onLogoutButtonClick?.Invoke();
        }
        #endregion
    }
}