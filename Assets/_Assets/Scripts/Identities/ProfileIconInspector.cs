using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WatKhaoWong.Identities
{
    [System.Serializable]
    public class ProfileIconInspector
    {
        public Image backgroundImage;
        public Image iconImage;
        public AspectRatioFitter aspectRatioFitter;
        public RectTransform iconRect;
        public Transform decoratorSpawnParent;



        #region --Properties-- (Computed)
        public Color32 BackgroundColor => backgroundImage.color;
        public Sprite Icon => iconImage.sprite;
        public float AspectRatio => aspectRatioFitter.aspectRatio;
        public Vector2 IconPivotY => iconRect.pivot;
        public List<GameObject> Decorators
        {
            get
            {
                List<GameObject> temp = new List<GameObject>();
                foreach (Transform each in decoratorSpawnParent)
                    temp.Add(each.gameObject);

                return temp;
            }
        }
        #endregion
    }
}