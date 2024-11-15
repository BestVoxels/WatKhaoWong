using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Settings
{
    public class LanguagePopup : Popup
    {
        #region --Fields-- (Inspector)
        //[Header("Setting Stuffs")]
        //[Tooltip("Check for Language Index on 'Window/Asset Management/Localization Tables/New Table Collection'.")]
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Language Popup UI Event")]
        [SerializeField] private UnityEvent _onThaiToggleIsOn;
        [SerializeField] private UnityEvent _onEngToggleIsOn;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnLanguageToggleIsOn; // used to use in UIRefresher.cs, keep it here just it case it is useful in future.
        #endregion



        #region --Fields-- (Constant)
        private const string KeyLanguageIndex = "LanguageIndex";
        private const string KeyLanguageToggles = "LanguageToggles";
        #endregion



        #region --Methods-- (Built In)
        private IEnumerator Start()
        {
            // Wait for the localization system to initialize
            yield return LocalizationSettings.InitializationOperation;

            SetLanguage(LoadLanguageIndex());
        }

        //// ---DEBUGGER PURPOSE---
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        SetLanguage(0);
        //    }

        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        SetLanguage(1);
        //    }
        //}
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnThaiToggleIsOn()
        {
            _onThaiToggleIsOn?.Invoke();
        }

        public void OnEngToggleIsOn()
        {
            _onEngToggleIsOn?.Invoke();
        }

        public void InvokeOnLanguageToggleIsOn()
        {
            OnLanguageToggleIsOn?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Save/Load~
        public int LoadLanguageIndex() => PlayerPrefs.GetInt(KeyLanguageIndex, -1);

        public void SaveLanguageIndex(int languageIndex) => PlayerPrefs.SetInt(KeyLanguageIndex, languageIndex);

        public bool[] LoadToggleIsOnValues()
        {
            return PlayerPrefsX.GetBools(KeyLanguageToggles, null);
        }

        public void SaveToggleValues(Toggle[] values)
        {
            bool[] isOnValues = values.Select(t => t.isOn).ToArray();

            PlayerPrefsX.SetBools(KeyLanguageToggles, isOnValues);
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void SetLanguage(int languageIndex)
        {
            if (languageIndex == -1) return;

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];

            SaveLanguageIndex(languageIndex);
        }
        #endregion
    }
}