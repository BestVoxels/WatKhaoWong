using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.Homes
{
    public class DonationAccounts : Page
    {
        #region --Fields-- (Inspector)
        #endregion



        #region --Events-- (UnityEvent)
        [Header("DonationAccounts UI Event")]
        [SerializeField] private UnityEvent _onCopyButton1Click;
        [SerializeField] private UnityEvent _onCopyButton2Click;
        [SerializeField] private UnityEvent _onCopyButton3Click;
        [SerializeField] private UnityEvent _onCopyButton4Click;
        [SerializeField] private UnityEvent _onCopyButton5Click;
        [SerializeField] private UnityEvent _onCopyButton6Click;
        [SerializeField] private UnityEvent _onCopyButton7Click;
        [SerializeField] private UnityEvent _onCopyButton8Click;
        [SerializeField] private UnityEvent _onCopyButton9Click;
        [SerializeField] private UnityEvent _onCopyButton10Click;
        [SerializeField] private UnityEvent _onCopyButton11Click;
        [SerializeField] private UnityEvent _onCopyButton12Click;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnCopyButton1Click()
        {
            _onCopyButton1Click?.Invoke();
        }

        public void OnCopyButton2Click()
        {
            _onCopyButton2Click?.Invoke();
        }

        public void OnCopyButton3Click()
        {
            _onCopyButton3Click?.Invoke();
        }

        public void OnCopyButton4Click()
        {
            _onCopyButton4Click?.Invoke();
        }

        public void OnCopyButton5Click()
        {
            _onCopyButton5Click?.Invoke();
        }

        public void OnCopyButton6Click()
        {
            _onCopyButton6Click?.Invoke();
        }

        public void OnCopyButton7Click()
        {
            _onCopyButton7Click?.Invoke();
        }

        public void OnCopyButton8Click()
        {
            _onCopyButton8Click?.Invoke();
        }

        public void OnCopyButton9Click()
        {
            _onCopyButton9Click?.Invoke();
        }

        public void OnCopyButton10Click()
        {
            _onCopyButton10Click?.Invoke();
        }

        public void OnCopyButton11Click()
        {
            _onCopyButton11Click?.Invoke();
        }

        public void OnCopyButton12Click()
        {
            _onCopyButton12Click?.Invoke();
        }
        #endregion
    }
}