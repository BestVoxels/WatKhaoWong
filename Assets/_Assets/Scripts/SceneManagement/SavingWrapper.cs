using System.IO;
using UnityEngine;
using WatKhaoWong.Saving;
using WatKhaoWong.Utils;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;

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



        #region --Properties-- (Computed)
        // Using private Property to PREVENT getting 'null' value if it doesn't authenticated on Start(). This way it will gets value when it needs, no need to initialize on Start().
        private string CurrentUserID
        {
            get
            {
                if (IsAuthenticated()) return FirebaseAuth.DefaultInstance.CurrentUser.UserId;

                return null;
            }
        }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingSystem = new AutoInit<SavingSystem>(() => GetComponent<SavingSystem>()); // Use AutoInit so that when other classes use public methods in their Start() SavingSystem won't be null
        }

        //private async void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.S))
        //    {
        //        Save(ESaveName.Level, 2);
        //    }

        //    if (Input.GetKeyDown(KeyCode.L))
        //    {
        //        DataSnapshot result = await Load(ESaveName.Level);
        //        if (result == null) return;
        //        print($"Key : {result.Key} / Value : {result.Value}");
        //    }

        //    if (Input.GetKeyDown(KeyCode.D))
        //    {
        //        Delete(ESaveName.Level);
        //    }

        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        bool result = await SaveExists(ESaveName.Level);
        //        print($"Is Exist Result : {result}");
        //    }
        //}
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// BECAREFUL! using this method can cause some errors!
        /// 1. Override Save file on Start()
        /// 2. Can't create Path if Guest User or (Not Authenticated) save to database
        /// MAKE SURE you know the sequence of the code that use this method.
        /// </summary>
        public void ForceSave(ESaveName saveName, object saveValue)
        {
            _savingSystem.value.Save(GetCurrentUserPath(saveName), saveValue);
        }

        /// <summary>
        /// For most cases this method is perfect to use and also the safest.
        /// But there is a caveat:
        /// 1. It won't Save on Start()
        /// 2. It won't Save if User is not Authenticated
        /// </summary>
        public void Save(ESaveName saveName, object saveValue)
        {
            if (!IsAuthenticated()) return;
            if (IsSaveProtectionOnStartActive()) return; // Avoid Override Save file with Default Values of UI or Player Default State.

            _savingSystem.value.Save(GetCurrentUserPath(saveName), saveValue);
        }

        public async Task<DataSnapshot> Load(ESaveName saveName)
        {
            if (!IsAuthenticated()) return null;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            return await _savingSystem.value.Load(GetCurrentUserPath(saveName));
        }

        public void Delete(ESaveName saveName)
        {
            if (!IsAuthenticated()) return;

            _savingSystem.value.Delete(GetCurrentUserPath(saveName));
        }

        /// <summary>
        /// Don't call this as checker for call 'Load()' because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.
        /// </summary>
        public async Task<bool> SaveExists(ESaveName saveName)
        {
            if (!IsAuthenticated()) return false;

            return await _savingSystem.value.SaveExists(GetCurrentUserPath(saveName));
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool IsAuthenticated() => FirebaseAuth.DefaultInstance.CurrentUser != null;

        private bool IsSaveProtectionOnStartActive() => _saveProtectionOnStartInSeconds > Time.time;
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Path Builder as JSON Tree Structure Example Above~
        private string GetCurrentUserPath(ESaveName saveName)
        {
            EPath? path = GetEPath(saveName); // this will assigned with first element which is 'EPath.Progression'

            if (path == null) return string.Empty;
            if (CurrentUserID == null) Debug.LogError("Can't create Path. Current User ID is null! Maybe because User is not authenticated.");

            return Path.Combine("Users", CurrentUserID, path.ToString(), saveName.ToString());
        }

        //// EXAMPLE of getting Other User Path
        //private string GetOtherUserPath(string otherUserID, ESaveName saveName)
        //{
        //    EPath? path = GetEPath(saveName); // this will assigned with first element which is 'EPath.Progression'

        //    if (path == null) return string.Empty;

        //    return Path.Combine("Users", otherUserID, path.ToString(), saveName.ToString());
        //}

        private EPath? GetEPath(ESaveName saveName)
        {
            EPath? path = null;

            switch (saveName)
            {
                case ESaveName.FirstName:
                case ESaveName.LastName:
                case ESaveName.MemberSince:
                case ESaveName.ProfileIconID:
                case ESaveName.Role:
                    path = EPath.Stats;
                    break;

                case ESaveName.Level:
                case ESaveName.XP:
                    path = EPath.Progression;
                    break;

                case ESaveName.TodayTMPoint:
                case ESaveName.TotalTMPoint:
                case ESaveName.ChallengeWon:
                case ESaveName.FirstUploadTimeOfDay:
                    path = EPath.Points;
                    break;

                default:
                    path = null;
                    break;
            }

            if (path == null)
                Debug.LogError("Can't build Path because 'EPath' value is null. (Return Empty Path as Empty String)");

            return path;
        }
        #endregion
    }
}