using UnityEngine;

namespace WatKhaoWong.Utils.Core
{
    public static class PlayerPrefsX
    {
        #region --Methods-- (Custom PUBLIC) ~STATIC~ ~Bool~
        public static void SetBool(string key, bool value)
        {
            // 1 == true | 0 == false
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            // 1 == true | 0 == false
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        public static bool GetBool(string key)
        {
            // 1 == true | 0 == false
            return PlayerPrefs.GetInt(key) == 1;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~STATIC~ ~Bool Array~
        public static void SetBools(string key, bool[] values)
        {
            BoolArrayObject arrayObject = new()
            {
                boolArray = (values == null) ? null : (bool[])values.Clone()
            };

            PlayerPrefs.SetString(key, JsonUtility.ToJson(arrayObject));
        }

        public static bool[] GetBools(string key, bool[] defaultValues)
        {
            BoolArrayObject arrayObject = new()
            {
                boolArray = (defaultValues == null) ? null : (bool[])defaultValues.Clone()
            };

            return JsonUtility.FromJson<BoolArrayObject>(PlayerPrefs.GetString(key, JsonUtility.ToJson(arrayObject))).boolArray;
        }

        public static bool[] GetBools(string key)
        {
            return JsonUtility.FromJson<BoolArrayObject>(PlayerPrefs.GetString(key)).boolArray;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        // Need to wrap Array inside a serializable class, this way Json will work.
        [System.Serializable]
        public class BoolArrayObject
        {
            public bool[] boolArray;
        }
        #endregion
    }
}