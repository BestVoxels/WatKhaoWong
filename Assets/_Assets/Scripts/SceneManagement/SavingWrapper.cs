using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WatKhaoWong.Saving;
using WatKhaoWong.Utils;

namespace WatKhaoWong.SceneManagement
{
    /// <summary>
    /// This component provides the methods to save and load a scene.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    /// </summary>
    public class SavingWrapper : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Saving Wrapper Stuffs")]
        [Tooltip("Amount of time in seconds that Save() won't work in the Beginning of the Game. To Avoid overriding Save file with Default Values of UI or Player Default State.")]
        [Range(1, 60)]
        [SerializeField] private byte _saveProtectionOnStartInSeconds = 3;
        #endregion



        #region --Fields-- (In Class)
        private AutoInit<SavingSystem> _savingSystem;
        #endregion



        #region --Fields-- (Constant)
        private const string SaveFileName = "NewSave18-7-2024";
        private const string CurrentSaveKey = "CurrentSaveKey";
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingSystem = new AutoInit<SavingSystem>(() => GetComponent<SavingSystem>()); // Use AutoInit so that when other classes use public methods in their Start() SavingSystem won't be null

            SetCurrentSaveName(SaveFileName);

            LoadAfterAwakeAndStart();
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.S))
        //    {
        //        Save();
        //    }

        //    if (Input.GetKeyDown(KeyCode.L))
        //    {
        //        Load();
        //    }

        //    if (Input.GetKeyDown(KeyCode.D))
        //    {
        //        Delete();
        //    }

        //    if (Input.GetKeyDown(KeyCode.N))
        //    {
        //        LoadLastScene();
        //    }

        //    if (Input.GetKeyDown(KeyCode.K))
        //    {
        //        LoadAfterAwakeAndStart();
        //    }
        //}
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void LoadAfterAwakeAndStart()
        {
            if (!PlayerPrefs.HasKey(CurrentSaveKey)) return; // Don't Have current save Key yet!
            if (!CurrentSaveFileExists()) return; // Actual SaveFile is NOT Exists!

            StartCoroutine(_savingSystem.value.LoadAfterAwakeAndStart(GetCurrentSaveName()));
        }

        public void LoadLastScene()
        {
            if (!PlayerPrefs.HasKey(CurrentSaveKey)) return; // Don't Have current save Key yet!
            if (!CurrentSaveFileExists()) return; // Actual SaveFile is NOT Exists!

            StartCoroutine(_savingSystem.value.LoadLastScene(GetCurrentSaveName()));
        }

        public void Save()
        {
            if (IsSaveProtectionOnStartActive()) return; // Avoid Override Save file with Default Values of UI or Player Default State.

            _savingSystem.value.Save(GetCurrentSaveName());
        }

        public void Load() => _savingSystem.value.Load(GetCurrentSaveName()); 

        public void Delete() => _savingSystem.value.Delete(GetCurrentSaveName());
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool CurrentSaveFileExists() => _savingSystem.value.SaveFileExists(GetCurrentSaveName());

        private IEnumerable<string> ListSaves() => _savingSystem.value.ListSaves();

        private bool IsSaveProtectionOnStartActive() => _saveProtectionOnStartInSeconds > Time.time;
        #endregion



        #region --Methods-- (Custom PRIVATE) ~PlayerPrefs Saving~
        // **Current Save is mainly for Continue Game to work (so it knows what save file is currently used)**
        private void SetCurrentSaveName(string currentFileName)
        {
            PlayerPrefs.SetString(CurrentSaveKey, currentFileName);
        }

        private string GetCurrentSaveName()
        {
            return PlayerPrefs.GetString(CurrentSaveKey);
        }
        #endregion
    }
}