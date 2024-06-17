using System;
using UnityEngine;
using UnityEngine.UI;

namespace WatKhaoWong.Identity
{
    public class ProfileIcon : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        [field: Header("UI Stuffs")]
        [field: SerializeField] public AccountData.IconUI UI { get; private set; }
        [field: SerializeField] public Toggle Toggle { get; private set; }
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<AccountData.IconUI, bool> OnToggleChanged;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            Toggle.onValueChanged.AddListener(OnValueChanged);
        }
        #endregion


        #region --Methods-- (Subscriber)
        private void OnValueChanged(bool isOn)
        {
            OnToggleChanged?.Invoke(UI, isOn);
        }
        #endregion
    }
}