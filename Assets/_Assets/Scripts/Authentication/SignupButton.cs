using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Authentication
{
    public class SignupButton : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Signup Button UI Event")]
        [SerializeField] private UnityEvent _onSignupButtonClick;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~UI Button~
        public void OnSignupButtonClick()
        {
            _onSignupButtonClick?.Invoke();
        }
        #endregion
    }
}