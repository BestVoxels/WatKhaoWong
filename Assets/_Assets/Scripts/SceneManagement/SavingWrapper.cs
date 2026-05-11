using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WatKhaoWong.Saving;
using WatKhaoWong.Utils;
using WatKhaoWong.Utils.Core;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Auth;
using System;
using Newtonsoft.Json;

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
        [Range(1f, 60f)]
        [SerializeField] private float _saveProtectionOnStartInSeconds = 3f;
        #endregion



        #region --Fields-- (In Class)
        private float _saveProtectionTimer = 0f;

        private AutoInit<SavingSystem> _savingSystem;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingSystem = new AutoInit<SavingSystem>(() => GetComponent<SavingSystem>()); // Use AutoInit so that when other classes use public methods in their Start() SavingSystem won't be null
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
        }

        private void Start()
        {
            StartCoroutine(StartProtectionTimer());
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }

        //// ---DEBUGGER PURPOSE---
        //private async void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.S))
        //    {
        //        Save(ECategoryNode.Users, EValueNode.Level, 2);
        //    }

        //    if (Input.GetKeyDown(KeyCode.L))
        //    {
        //        DataSnapshot result = await Load(ECategoryNode.Users, EValueNode.Level);
        //        if (result == null) return;
        //        print($"Key : {result.Key} / Value : {result.Value}");
        //    }

        //    if (Input.GetKeyDown(KeyCode.D))
        //    {
        //        Delete(ECategoryNode.Users, EValueNode.Level);
        //    }

        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        bool result = await IsSaveExists(ECategoryNode.Users, EValueNode.Level);
        //        print($"Is Exist Result : {result}");
        //    }
        //}
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Firebase Storage~
        public async Task<bool> UploadImage(string firebasePath, Texture2D image, Dictionary<string, string> customMetadata, byte maxImageSizeInMB)
        {
            return await _savingSystem.value.UploadImage(firebasePath, image, customMetadata, maxImageSizeInMB);
        }

        public async Task<Texture2D> DownloadImage(string firebasePath, byte maxImageSizeInMB)
        {
            return await _savingSystem.value.DownloadImage(firebasePath, maxImageSizeInMB);
        }

        public void DeleteFile(string firebasePath)
        {
            _savingSystem.value.DeleteFile(firebasePath);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Firebase Database~
        /// <summary>
        /// BECAREFUL! using this method can cause some errors!
        /// 1. Override Save file on Start()
        /// 2. Can't create Path if Guest User or (Not Authenticated) save to database
        /// MAKE SURE you know the sequence of the code that use this method.
        /// </summary>
        public void ForceSave(ECategoryNode categoryNode, EValueNode valueNode, object saveValue)
        {
            _ = _savingSystem.value.Save(GetPath(categoryNode, valueNode), saveValue);
        }

        /// <summary>
        /// For most cases this method is perfect to use and also the safest.
        /// But there is a caveat:
        /// 1. It won't Save on Start()
        /// 2. It won't Save if User is not Authenticated
        /// </summary>
        public void Save(ECategoryNode categoryNode, EValueNode valueNode, object saveValue)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;
            if (IsSaveProtectionActive()) return; // Avoid Override Save file with Default Values of UI or Player Default State.

            _ = _savingSystem.value.Save(GetPath(categoryNode, valueNode), saveValue);
        }

        public void ForceSaveAnyUser(ECategoryNode categoryNode, string userID, EValueNode valueNode, object saveValue)
        {
            _ = _savingSystem.value.Save(Path.Combine(categoryNode.ToString(), userID, GetValueNodePath(categoryNode, valueNode)), saveValue);
        }

        public void ForceSaveChallengeTMWinner(string challengeID, string userID, object saveValue)
        {
            _ = _savingSystem.value.Save(Path.Combine(ECategoryNode.LeaderboardTMChallengeWinner.ToString(), challengeID, userID, EValueNode.ChallengeTMPoint.ToString()), saveValue);
        }

        public async Task SaveToMyUser(EParentNode parentNode, string pathUnderParent, object saveValue)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;
            if (IsSaveProtectionActive()) return; // Avoid Override Save file with Default Values of UI or Player Default State.

            await _savingSystem.value.Save(Path.Combine(GetMyUserPath(parentNode), pathUnderParent), saveValue);
        }

        public async Task SaveData(ECategoryNode categoryNode, DataNode dataNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;
            if (IsSaveProtectionActive()) return; // Avoid Override Save file with Default Values of UI or Player Default State.

            await _savingSystem.value.SaveJson(Path.Combine(categoryNode.ToString()), dataNode);
        }

        public async Task SaveDataToMyUser(EParentNode parentNode, DataNode dataNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;
            if (IsSaveProtectionActive()) return; // Avoid Override Save file with Default Values of UI or Player Default State.

            await _savingSystem.value.SaveJson(GetMyUserPath(parentNode), dataNode);
        }

        public async Task<string> SaveDataWithKey(ECategoryNode categoryNode, DataNode dataNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return default;
            if (IsSaveProtectionActive()) return default; // Avoid Override Save file with Default Values of UI or Player Default State.

            return await _savingSystem.value.SaveDataWithKey(Path.Combine(categoryNode.ToString()), dataNode);
        }

        public async Task SaveDataToExistingKey(ECategoryNode categoryNode, string keyId, DataNode dataNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;
            if (IsSaveProtectionActive()) return; // Avoid Override Save file with Default Values of UI or Player Default State.

            await _savingSystem.value.SaveJson(Path.Combine(categoryNode.ToString(), keyId), dataNode);
        }

        public async Task<string> SaveDataWithKeyToMyUser(EParentNode parentNode, DataNode dataNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return default;
            if (IsSaveProtectionActive()) return default; // Avoid Override Save file with Default Values of UI or Player Default State.

            return await _savingSystem.value.SaveDataWithKey(GetMyUserPath(parentNode), dataNode);
        }

        public async Task SaveDataToExistingKeyToMyUser(EParentNode parentNode, string keyId, DataNode dataNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;
            if (IsSaveProtectionActive()) return; // Avoid Override Save file with Default Values of UI or Player Default State.

            await _savingSystem.value.SaveJson(Path.Combine(GetMyUserPath(parentNode), keyId), dataNode);
        }


        public async Task<DataSnapshot> Load(ECategoryNode categoryNode, EValueNode valueNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return null;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            return await _savingSystem.value.Load(GetPath(categoryNode, valueNode));
        }

        public async Task<DataSnapshot> ForceLoad(ECategoryNode categoryNode, EValueNode valueNode)
        {
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            return await _savingSystem.value.Load(GetPath(categoryNode, valueNode));
        }

        public async Task<T> LoadDataFromMyUser<T>(EParentNode parentNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return default;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            return await _savingSystem.value.LoadJson<T>(GetMyUserPath(parentNode));
        }

        public async Task<DataSnapshot> LoadOtherUser(string otherUserID)
        {
            if (!FirebaseUtils.IsAuthenticated()) return null;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            return await _savingSystem.value.Load(Path.Combine(ECategoryNode.Users.ToString(), otherUserID));
        }

        public async IAsyncEnumerable<DataSnapshot> LoadAndSortByChildValue(ECategoryNode categoryNode, EValueNode valueNode, int limitNumber)
        {
            if (!FirebaseUtils.IsAuthenticated()) yield break;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            DataSnapshot dataSnapshot = await _savingSystem.value.LoadAndSortByChildValue(categoryNode.ToString(), GetValueNodePath(categoryNode, valueNode), limitNumber);
            
            if (dataSnapshot == null)
            {
                Debug.LogWarning("Can't Load Child Value, maybe path is Wrong. 'dataSnapshot' is equals to 'null'.");
                yield break;
            }

            foreach (DataSnapshot each in dataSnapshot.Children.Reverse()) // Call 'Reverse()' to makes it Descending
            {
                yield return each;
            }
        }

        public async IAsyncEnumerable<(StayEntry, string)> LoadPastEntryFromMyUser()
        {
            if (!FirebaseUtils.IsAuthenticated()) yield break;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.
            
            await foreach (DataSnapshot each in _savingSystem.value.LoadChildren(GetMyUserPath(EParentNode.PastStay)))
            {
                string key = each.Key;

                string jsonString = each.GetRawJsonValue();
                StayEntry stayEntry = JsonConvert.DeserializeObject<StayEntry>(jsonString);

                yield return (stayEntry, key);
            }
        }

        public async Task<string> LoadActiveStayKeyIDFromMyUser()
        {
            DataSnapshot keyIdSnapshot = await _savingSystem.value.Load(Path.Combine(ECategoryNode.Users.ToString(), FirebaseUtils.CurrentUserID, EParentNode.ActiveStay.ToString(), EValueNode.KeyId.ToString()));

            if (keyIdSnapshot == null)
            {
                //Debug.LogWarning("Can't find 'KeyId' probably no 'ActiveStay' under my 'User' Category");
                return null;
            }

            return keyIdSnapshot.Value.ToString();
        }

        //public async Task<T> LoadDataWithKey<T>(ECategoryNode categoryNode, DataNode dataNode)
        //{
        //    // Loop first using "LoadChildrenJson()" method
        //    // Then find the one that match the key
        //    // Return that to user as a type based on what user want.
        //}

        /// <summary>
        /// This method should be used by 'MyUserData.cs' only.
        /// </summary>
        public async Task<StayEntry> LoadMyEntryFromStayRequests()
        {
            if (!FirebaseUtils.IsAuthenticated()) return default;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            string keyID = await LoadActiveStayKeyIDFromMyUser();
            if (keyID == null) return null;

            // Now directly fetch StayRequest by 'KeyId'
            StayEntry stayEntry = await _savingSystem.value.LoadJson<StayEntry>( Path.Combine(ECategoryNode.StayRequests.ToString(), keyID) );

            if (stayEntry == null)
            {
                Debug.LogWarning("Can't find 'StayEntry' under 'StayRequests' Category");
                return null;
            }

            return stayEntry;
        }

        /// <summary>
        /// This method should be used by 'MyUserData.cs' only.
        /// </summary>
        public async Task<StayEntry> LoadMyEntryFromScheduledStay()
        {
            if (!FirebaseUtils.IsAuthenticated()) return default;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            string keyID = await LoadActiveStayKeyIDFromMyUser();
            if (keyID == null) return null;

            // Now directly fetch ScheduledStay by 'KeyId'
            StayEntry stayEntry = await _savingSystem.value.LoadJson<StayEntry>(Path.Combine(ECategoryNode.ScheduledStay.ToString(), keyID));

            if (stayEntry == null)
            {
                Debug.LogWarning("Can't find 'StayEntry' under 'ScheduledStay' Category");
                return null;
            }

            return stayEntry;
        }

        /// <summary>
        /// This method should be used by 'MyUserData.cs' only.
        /// </summary>
        public async Task<StayEntry> LoadMyEntryFromActiveStay()
        {
            if (!FirebaseUtils.IsAuthenticated()) return default;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            string keyID = await LoadActiveStayKeyIDFromMyUser();
            if (keyID == null) return null;

            // Now directly fetch ActiveStay by 'KeyId'
            StayEntry stayEntry = await _savingSystem.value.LoadJson<StayEntry>(Path.Combine(ECategoryNode.ActiveStay.ToString(), keyID));

            if (stayEntry == null)
            {
                Debug.LogWarning("Can't find 'StayEntry' under 'ActiveStay' Category");
                return null;
            }

            return stayEntry;
        }

        



        public void Delete(ECategoryNode categoryNode, EValueNode valueNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;

            _savingSystem.value.Delete(GetPath(categoryNode, valueNode));
        }

        public void DeleteFromMyUser(EParentNode parentNode, string pathUnderParent = null)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;

            if (pathUnderParent == null)
            {
                _savingSystem.value.Delete(GetMyUserPath(parentNode));
                return;
            }

            _savingSystem.value.Delete(Path.Combine(GetMyUserPath(parentNode), pathUnderParent));
        }

        public void DeleteActiveStayEntry(string entryKeyId)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;

            _savingSystem.value.Delete(Path.Combine(ECategoryNode.ActiveStay.ToString(), entryKeyId));
        }
        public void DeleteScheduledStayEntry(string entryKeyId)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;

            _savingSystem.value.Delete(Path.Combine(ECategoryNode.ScheduledStay.ToString(), entryKeyId));
        }
        public void DeleteStayRequestsEntry(string entryKeyId)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;

            _savingSystem.value.Delete(Path.Combine(ECategoryNode.StayRequests.ToString(), entryKeyId));
        }

        public void ForceDeleteLeaderboardTMToday()
        {
            _savingSystem.value.Delete(ECategoryNode.LeaderboardTMToday.ToString());
        }

        public void ForceDeleteLeaderboardTMChallenge()
        {
            _savingSystem.value.Delete(ECategoryNode.LeaderboardTMChallenge.ToString());
        }

        /// <summary>
        /// Please update RULE on Firebase Console for this to works.
        /// </summary>
        public void ForceDeleteAnyUser(ECategoryNode categoryNode, string userID, EValueNode valueNode)
        {
            _savingSystem.value.Delete(Path.Combine(categoryNode.ToString(), userID, GetValueNodePath(categoryNode, valueNode)));
        }



        /// <summary>
        /// Don't call this as checker for call 'Load()' because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.
        /// </summary>
        public async Task<bool> IsSaveExists(ECategoryNode categoryNode, EValueNode valueNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return false;

            return await _savingSystem.value.IsSaveExists(GetPath(categoryNode, valueNode));
        }

        public async Task<bool> IsLeaderboardTMTodayExists()
        {
            if (!FirebaseUtils.IsAuthenticated()) return false;

            return await _savingSystem.value.IsSaveExists(ECategoryNode.LeaderboardTMToday.ToString());
        }

        public async Task<bool> IsLeaderboardTMChallengeExists()
        {
            if (!FirebaseUtils.IsAuthenticated()) return false;

            return await _savingSystem.value.IsSaveExists(ECategoryNode.LeaderboardTMChallenge.ToString());
        }

        public async Task<bool> ForceIsSaveExists(ECategoryNode categoryNode, string userID, EValueNode valueNode)
        {
            return await _savingSystem.value.IsSaveExists(Path.Combine(categoryNode.ToString(), userID, GetValueNodePath(categoryNode, valueNode)));
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Useful Utility~
        public static string GetValueNodePath(ECategoryNode categoryNode, EValueNode valueNode)
        {
            // ONLY 'Users' category that needs ParentNode
            if (categoryNode == ECategoryNode.Users)
            {
                EParentNode? parentNode = GetParentNode(valueNode);
                if (parentNode == null) return null;

                return Path.Combine(parentNode.ToString(), valueNode.ToString());
            }

            return Path.Combine(valueNode.ToString());
        }

        public static EParentNode? GetParentNode(EValueNode valueNode)
        {
            EParentNode? parentNode = null;

            switch (valueNode)
            {
                case EValueNode.FirstName:
                case EValueNode.LastName:
                case EValueNode.MemberSince:
                case EValueNode.ProfileIconID:
                case EValueNode.Role:
                case EValueNode.Title:
                    parentNode = EParentNode.Stats;
                    break;

                case EValueNode.TMPointCapRequest:
                case EValueNode.TMPointCap:
                case EValueNode.TMPointCapRound:
                case EValueNode.IsCustomTMPointCap:
                case EValueNode.TodayTMPoint:
                case EValueNode.TotalTMPoint:
                case EValueNode.ChallengeTMPoint:
                case EValueNode.ChallengeTMWon:
                case EValueNode.FirstUploadTimeOfDayTM:
                case EValueNode.FirstUploadTimeOfChallengeTM:
                    parentNode = EParentNode.TMPoints;
                    break;

                case EValueNode.TempleGuideConfirmed:
                case EValueNode.TempleGuideConfirmedAt:
                    parentNode = EParentNode.Agreement;
                    break;

                case EValueNode.Level:
                case EValueNode.XP:
                    parentNode = EParentNode.Progression;
                    break;

                case EValueNode.State:
                    parentNode = EParentNode.AccountStatus;
                    break;

                default:
                    parentNode = null;
                    break;
            }

            if (parentNode == null)
                Debug.LogError("Can't build Path because 'EParentNode' value is null. (Return Empty Path as Empty String)");

            return parentNode;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool IsSaveProtectionActive() => _saveProtectionTimer < _saveProtectionOnStartInSeconds;

        private IEnumerator StartProtectionTimer()
        {
            _saveProtectionTimer = 0f;

            while (IsSaveProtectionActive())
            {
                _saveProtectionTimer += Time.deltaTime;
                yield return null;
            }

            yield break;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Path Builder as JSON Tree Structure Example Above~
        private string GetPath(ECategoryNode categoryNode, EValueNode valueNode)
        {
            string valueNodePath = GetValueNodePath(categoryNode, valueNode);
            if (valueNodePath == null) Debug.LogError("Can't create Path. ValueNodePath is null! Maybe because Parent Node is null.");

            // ONLY 'LeaderboardStats' or 'ServerStats' or 'RemoteConfig' Category does NOT NEED userID in path.
            if (categoryNode == ECategoryNode.LeaderboardStats || categoryNode == ECategoryNode.ServerStats || categoryNode == ECategoryNode.RemoteConfig)
            {
                return Path.Combine(categoryNode.ToString(), valueNodePath);
            }

            if (FirebaseUtils.CurrentUserID == null) Debug.LogError("Can't create Path. Current User ID is null! Maybe because User is not authenticated.");
            return Path.Combine(categoryNode.ToString(), FirebaseUtils.CurrentUserID, valueNodePath);
        }

        private string GetMyUserPath(EParentNode parentNode)
        {
            return Path.Combine(ECategoryNode.Users.ToString(), FirebaseUtils.CurrentUserID, parentNode.ToString());
        }
        #endregion



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        /// </summary>
        private void HandleStateChanged(object obj, EventArgs args)
        {
            StartCoroutine(StartProtectionTimer()); // So it get reset and start again when User Log In or Log Out
        }
        #endregion
    }
}