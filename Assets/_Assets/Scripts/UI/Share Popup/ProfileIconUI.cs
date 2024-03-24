using UnityEngine;
using UnityEngine.UI;

namespace WatKhaoWong.UI.SharePopup
{
    public class ProfileIconUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [field: Header("UI Stuffs")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private AspectRatioFitter _aspectRatioFitter;
        [SerializeField] private RectTransform _iconRect;
        #endregion



        #region --Properties-- (Computed)
        public Color32 BackgroundColor => _backgroundImage.color;
        public Sprite Icon => _iconImage.sprite;
        public float AspectRatio => _aspectRatioFitter.aspectRatio;
        public float IconPivotY => _iconRect.pivot.y;
        #endregion
    }
}