using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WatKhaoWong.Attributes
{
    public class Account : MonoBehaviour
    {
        #region --Methods-- (Custom PUBLIC) ~LOAD User Data~
        public string GetUserNameText()
        {
            // TODO fetch data from Server
            return "Thanitsak Leuangsupornpong";
        }

        public string GetUserLevelText()
        {
            // TODO fetch data from Server
            return "LV. 1";
        }

        public int GetAllTimeTMPoints()
        {
            // TODO fetch data from Server
            return 88888888;
        }

        public int GetTodayTMPoints()
        {
            // TODO fetch data from Server
            return 108;
        }

        public int GetTotalWonTMChallenge()
        {
            // TODO fetch data from Server
            return 88888;
        }

        public string GetMemberSinceText()
        {
            // TODO fetch data from Server
            return "2/3/2001";
        }

        public IconData GetProfileIcon()
        {
            return null;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~LOAD User Data~
        //public string GetUserNameText()
        //{
        //    // TODO fetch data from Server
        //    return "Thanitsak Leuangsupornpong";
        //}

        //public string GetUserLevelText()
        //{
        //    // TODO fetch data from Server
        //    return "LV. 1";
        //}

        //public int GetAllTimeTMPoints()
        //{
        //    // TODO fetch data from Server
        //    return 88888888;
        //}

        //public int GetTodayTMPoints()
        //{
        //    // TODO fetch data from Server
        //    return 108;
        //}

        //public int GetTotalWonTMChallenge()
        //{
        //    // TODO fetch data from Server
        //    return 88888;
        //}

        //public string GetMemberSinceText()
        //{
        //    // TODO fetch data from Server
        //    return "2/3/2001";
        //}

        public void SetProfileIcon(Color32 backgroundColor, Sprite icon, float aspectRatio, Vector2 iconPivotY, IEnumerable<GameObject> decorators)
        {
            IconData profileIcon = new IconData();

            profileIcon.backgroundColor = backgroundColor;
            profileIcon.icon = icon;
            profileIcon.aspectRatio = aspectRatio;
            profileIcon.iconPivotY = iconPivotY;
            profileIcon.decorators = decorators;

            // TODO SAVE data TO server
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
        [System.Serializable]
        public class IconData
        {
            public Color32 backgroundColor;
            public Sprite icon;
            public float aspectRatio;
            public Vector2 iconPivotY;
            public IEnumerable<GameObject> decorators;
        }

        [System.Serializable]
        public class IconUI
        {
            public Image backgroundImage;
            public Image iconImage;
            public AspectRatioFitter aspectRatioFitter;
            public RectTransform iconRect;
            public Transform decoratorSpawnParent;
        }
        #endregion
    }
}