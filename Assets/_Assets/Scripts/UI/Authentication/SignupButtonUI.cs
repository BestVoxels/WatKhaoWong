using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Authentication;

namespace WatKhaoWong.UI.Authentication
{
    public class SignupButtonUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        //[Header("Popup Header UI Stuffs")]
        //[SerializeField] private Button _closeButton;

        [Header("Signup Button UI Stuffs")]
        [SerializeField] private Button _signupButton;
        #endregion



        #region --Fields-- (In Class)
        private SignupButton _playerSignupButton;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerSignupButton = GameObject.FindWithTag("Player").GetComponentInChildren<SignupButton>();

            //_closeButton.onClick.AddListener(Close);

            _signupButton.onClick.AddListener(Signup);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        //private void Close() => _playerSignupButton.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Signup() => _playerSignupButton.OnSignupButtonClick();
        #endregion
    }
}