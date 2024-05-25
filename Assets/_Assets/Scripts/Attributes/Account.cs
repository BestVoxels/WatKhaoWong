using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WatKhaoWong.Attributes
{
    public class Account : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        [field: Header("Account Stuffs")]
        [field: SerializeField] public ProfileIcon DefaultProfileIcon { get; private set; }
        [field: Space]
        [field: Header("Debugger Stuffs")]
        [field: SerializeField] public EAccountRole Role { get; private set; } = EAccountRole.Member;
        #endregion



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

        public IconData GetIconData()
        {
            // TODO SAVE CODE - TEMP CODE - have to change save method because InstaceID will be changed everytime we load UnityEditor.
            IconData iconData = JsonUtility.FromJson<IconData>(PlayerPrefs.GetString("AccountProfileIcon"));

            // TODO fetch data from Server

            if (iconData == null)
                return new IconData(DefaultProfileIcon.UI.BackgroundColor, DefaultProfileIcon.UI.Icon, DefaultProfileIcon.UI.AspectRatio, DefaultProfileIcon.UI.IconPivotY, DefaultProfileIcon.UI.Decorators);

            return iconData;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~SAVE User Data~
        //public void SetUserNameText(string input)
        //{
        //    // TODO SAVE data TO server
        //}

        //public void SetUserLevelText(string input)
        //{
        //    // TODO SAVE data TO server
        //}

        //public void SetAllTimeTMPoints(int input)
        //{
        //    // TODO SAVE data TO server
        //}

        //public void SetTodayTMPoints(int input)
        //{
        //    // TODO SAVE data TO server
        //}

        //public void SetTotalWonTMChallenge(int input)
        //{
        //    // TODO SAVE data TO server
        //}

        //public void SetMemberSinceText(string input)
        //{
        //    // TODO SAVE data TO server
        //}

        public void SetIconData(IconUI iconUI)
        {
            SetIconData(iconUI.BackgroundColor, iconUI.Icon, iconUI.AspectRatio, iconUI.IconPivotY, iconUI.Decorators);
        }

        public void SetIconData(Color32 backgroundColor, Sprite icon, float aspectRatio, Vector2 iconPivotY, List<GameObject> decorators)
        {
            IconData iconData = new(backgroundColor, icon, aspectRatio, iconPivotY, decorators);

            SetIconData(iconData);
        }

        public void SetIconData(IconData iconData)
        {
            // TODO SAVE CODE - TEMP CODE
            PlayerPrefs.SetString("AccountProfileIcon", JsonUtility.ToJson(iconData));

            // TODO SAVE data TO server
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void UpdateProfileIcon(IconUI oldUI, IconUI newUI, float multiplierRatioForDecorator)
        {
            IconData iconData = new(newUI.BackgroundColor, newUI.Icon, newUI.AspectRatio, newUI.IconPivotY, newUI.Decorators);

            UpdateProfileIcon(oldUI, iconData, multiplierRatioForDecorator);
        }

        public void UpdateProfileIcon(IconUI oldUI, IconData newData, float multiplierRatioForDecorator)
        {
            // Clear Spawned Decorators (no error if there are not)
            foreach (Transform each in oldUI.decoratorSpawnParent)
                Destroy(each.gameObject);

            // Replicate Toggle Profile to Main Profile
            oldUI.backgroundImage.color = newData.backgroundColor;
            oldUI.iconImage.sprite = newData.icon;
            oldUI.aspectRatioFitter.aspectRatio = newData.aspectRatio;
            oldUI.iconRect.pivot = newData.iconPivotY;

            if (newData.decorators != null)
            {
                foreach (GameObject each in newData.decorators)
                {
                    if (each == null) return; // Guard check MUST DO because InstaceID will be changed everytime we load UnityEditor.

                    GameObject result = Instantiate(each, oldUI.decoratorSpawnParent, false);

                    RectTransform rt = result.GetComponent<RectTransform>();
                    rt.localPosition = new Vector2(rt.localPosition.x * multiplierRatioForDecorator, rt.localPosition.y * multiplierRatioForDecorator);
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x * multiplierRatioForDecorator, rt.sizeDelta.y * multiplierRatioForDecorator);
                }
            }

            SetIconData(newData);
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
            public List<GameObject> decorators;



            #region --Constructors-- (PUBLIC)
            public IconData(Color32 bgColor, Sprite ic, float apRatio, Vector2 icPivotY, List<GameObject> decors)
            {
                backgroundColor = bgColor;
                icon = ic;
                aspectRatio = apRatio;
                iconPivotY = icPivotY;
                decorators = decors;
            }
            #endregion
        }

        [System.Serializable]
        public class IconUI
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
        #endregion
    }
}