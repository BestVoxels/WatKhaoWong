using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace WatKhaoWong.Authentication
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
            // TODO show iOS/Android's Native Alert Window (like Signout Now or Cancel?) - do it like Shopee, Facebook, Lazada.

            // NO NEED to manually set AccountRole back to Guest. Because it has "FirebaseAuth.DefaultInstance.StateChanged" that subscribed with "SetRoleToGuestIfNoAuthen()".
            FirebaseAuth.DefaultInstance.SignOut();

            // Reload the Scene to make it reset back! reset value back
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            _onLogoutButtonClick?.Invoke();
        }
        #endregion
    }
}