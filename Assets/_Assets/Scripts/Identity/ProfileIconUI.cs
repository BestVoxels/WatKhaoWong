using System;
using UnityEngine;
using UnityEngine.UI;

namespace WatKhaoWong.Identity
{
    public class ProfileIconUI : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        [field: Header("UI Stuffs")]
        [field: SerializeField] public ProfileIconItem Icon { get; private set; }
        [field: SerializeField] public ProfileIconInspector UI { get; private set; }
        [field: SerializeField] public Toggle Toggle { get; private set; }
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<ProfileIconItem, bool> OnToggleChanged;
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
            OnToggleChanged?.Invoke(Icon, isOn);
        }
        #endregion
    }
}