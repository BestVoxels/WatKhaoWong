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



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// BECAREFUL! using this method can cause some errors!
        /// 1. Override Save file on Start()
        /// 2. Can't create Path if Guest User or (Not Authenticated) save to database
        /// MAKE SURE you know the sequence of the code that use this method.
        /// </summary>
        public void ForceSave(ECategoryNode categoryNode, EValueNode valueNode, object saveValue)
        {
            _savingSystem.value.Save(GetPath(categoryNode, valueNode), saveValue);
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

            _savingSystem.value.Save(GetPath(categoryNode, valueNode), saveValue);
        }

        public void ForceSaveAnyUser(ECategoryNode categoryNode, string userID, EValueNode valueNode, object saveValue)
        {
            _savingSystem.value.Save(Path.Combine(categoryNode.ToString(), userID, GetValueNodePath(categoryNode, valueNode)), saveValue);
        }

        public void ForceSaveChallengeTMWinner(string challengeID, string userID, object saveValue)
        {
            _savingSystem.value.Save(Path.Combine(ECategoryNode.LeaderboardTMChallengeWinner.ToString(), challengeID, userID, EValueNode.ChallengeTMPoint.ToString()), saveValue);
        }

        public async Task<DataSnapshot> Load(ECategoryNode categoryNode, EValueNode valueNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return null;
            // Don't call 'SaveExists()' to check because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.

            return await _savingSystem.value.Load(GetPath(categoryNode, valueNode));
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

        public void Delete(ECategoryNode categoryNode, EValueNode valueNode)
        {
            if (!FirebaseUtils.IsAuthenticated()) return;

            _savingSystem.value.Delete(GetPath(categoryNode, valueNode));
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

                case EValueNode.Level:
                case EValueNode.XP:
                    parentNode = EParentNode.Progression;
                    break;

                case EValueNode.TodayTMPoint:
                case EValueNode.TotalTMPoint:
                case EValueNode.ChallengeTMPoint:
                case EValueNode.ChallengeTMWon:
                case EValueNode.FirstUploadTimeOfDayTM:
                case EValueNode.FirstUploadTimeOfChallengeTM:
                    parentNode = EParentNode.TMPoints;
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

            // ONLY 'LeaderboardStats' or 'ServerStats' Category does NOT NEED userID in path.
            if (categoryNode == ECategoryNode.LeaderboardStats || categoryNode == ECategoryNode.ServerStats)
            {
                return Path.Combine(categoryNode.ToString(), valueNodePath);
            }

            if (FirebaseUtils.CurrentUserID == null) Debug.LogError("Can't create Path. Current User ID is null! Maybe because User is not authenticated.");
            return Path.Combine(categoryNode.ToString(), FirebaseUtils.CurrentUserID, valueNodePath);
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