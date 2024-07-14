using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.CoreItems;

namespace WatKhaoWong.Identity
{
    public class AccountData : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        [field: Header("Account Stuffs")]
        [field: SerializeField] public ProfileIcon DefaultProfileIcon { get; private set; }
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

        public ProfileIcon GetProfileIcon()
        {
            // TODO SAVE CODE - TEMP CODE - have to change save method because InstaceID will be changed everytime we load UnityEditor.
            // TODO fetch data from Server
            string id = PlayerPrefs.GetString("AccountProfileIcon", DefaultProfileIcon.ItemID);

            ProfileIcon icon = BaseItem.GetFromID(id) as ProfileIcon;

            if (icon == null) return DefaultProfileIcon;

            return icon;
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

        public void SetProfileIcon(ProfileIcon icon)
        {
            // TODO SAVE CODE - TEMP CODE
            PlayerPrefs.SetString("AccountProfileIcon", icon.ItemID);

            // TODO SAVE data TO server
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void UpdateProfileIcon(IconUI oldUI, ProfileIcon newIcon, float multiplierRatioForDecorator)
        {
            if (newIcon == null)
            {
                Debug.LogError("Can't Update ProfileIcon to new one because it is Null.");
                return;
            }

            // Clear Spawned Decorators (no error if there are not)
            foreach (Transform each in oldUI.decoratorSpawnParent)
                Destroy(each.gameObject);

            // Replicate Toggle Profile to Main Profile
            oldUI.backgroundImage.color = newIcon.ProfileIconUI.UI.BackgroundColor;
            oldUI.iconImage.sprite = newIcon.ProfileIconUI.UI.Icon;
            oldUI.aspectRatioFitter.aspectRatio = newIcon.ProfileIconUI.UI.AspectRatio;
            oldUI.iconRect.pivot = newIcon.ProfileIconUI.UI.IconPivotY;

            if (newIcon.ProfileIconUI.UI.Decorators != null)
            {
                foreach (GameObject each in newIcon.ProfileIconUI.UI.Decorators)
                {
                    if (each == null) return; // Guard check MUST DO because InstaceID will be changed everytime we load UnityEditor.

                    GameObject result = Instantiate(each, oldUI.decoratorSpawnParent, false);

                    RectTransform rt = result.GetComponent<RectTransform>();
                    rt.localPosition = new Vector2(rt.localPosition.x * multiplierRatioForDecorator, rt.localPosition.y * multiplierRatioForDecorator);
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x * multiplierRatioForDecorator, rt.sizeDelta.y * multiplierRatioForDecorator);
                }
            }
            
            SetProfileIcon(newIcon);
        }
        #endregion



        #region --Classes-- (Custom PUBLIC)
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