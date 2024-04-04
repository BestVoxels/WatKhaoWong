using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Attributes;

namespace WatKhaoWong.UI.SharePopup
{
    public class ProfileIconUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("UI Stuffs")]
        [SerializeField] private Account.IconUI _icon;
        [Space]
        [Header("Other Stuffs")]
        [SerializeField] private Toggle _toggle;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action<ProfileIconUI, bool> OnToggleChanged;
        #endregion



        #region --Properties-- (Computed)
        // TODO Maybe moved to Account.cs
        public Color32 BackgroundColor => _icon.backgroundImage.color;
        public Sprite Icon => _icon.iconImage.sprite;
        public float AspectRatio => _icon.aspectRatioFitter.aspectRatio;
        public Vector2 IconPivotY => _icon.iconRect.pivot;
        public IEnumerable<GameObject> Decorators
        {
            get
            {
                foreach (Transform each in _icon.decoratorSpawnParent)
                    yield return each.gameObject;
            }
        }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }
        #endregion


        #region --Methods-- (Subscriber)
        private void OnValueChanged(bool isOn)
        {
            OnToggleChanged?.Invoke(this, isOn);
        }
        #endregion
    }
}