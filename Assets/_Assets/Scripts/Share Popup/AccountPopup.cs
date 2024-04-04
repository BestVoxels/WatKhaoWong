using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.SharePopup
{
    public class AccountPopup : Popup
    {
        #region --Properties-- (Inspector)
        [field: Header("Account Popup Status Text")]
        [field: SerializeField] public string StatusInformUser { get; private set; } = "Click any Profile Icon on the right section to change Profile Picture";
        [field: SerializeField] public Color32 StatusInformUserColor { get; private set; }
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Account Popup UI Event")]
        [SerializeField] private UnityEvent _onAccountProfileChanged;
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnAccountProfileChanged()
        {
            _onAccountProfileChanged?.Invoke();
        }
        #endregion
    }
}