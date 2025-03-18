using Firebase.Database;
using System;
using UnityEngine;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.Identities
{
    public class ManuallyAdjustUserData : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private SavingWrapper _savingWrapper;

        private static bool _firstTime = false; // Make sure this script run only once
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void Start()
        {
            if (_firstTime == false)
                ManuallyAdjustUsersScore();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        /// <summary>
        /// FOR THESE CODE TO WORKS PLEASE UPDATE FIREBASE RULE!!!
        ///
        /// update Firebase Rule so that User can WRITE to other User.
        /// </summary>
        private async void ManuallyAdjustUsersScore()
        {
            ushort index = 0;
            await foreach (DataSnapshot eachData in _savingWrapper.LoadAndSortByChildValue(ECategoryNode.Users, EValueNode.TotalTMPoint, 1000))
            {
                ++index;

                OtherUserData otherUserData = new OtherUserData(eachData);

                //// -> Delete 'ChallengeTMPoint' node
                //if (otherUserData.GetChallengeTMPoints() == 0)
                //{
                //    _savingWrapper.ForceDeleteAnyUser(ECategoryNode.Users, eachData.Key, EValueNode.ChallengeTMPoint);

                //    Debug.LogWarning($"({index})");
                //}

                // -> Get 'TotalTMPoint' and copy into 'ChallengeTMPoint'
                if (otherUserData.GetTotalTMPoints() > 0)
                {
                    _savingWrapper.ForceSaveAnyUser(ECategoryNode.Users, eachData.Key, EValueNode.ChallengeTMPoint, otherUserData.GetTotalTMPoints());

                    Debug.LogWarning($"({index}) GetTotalTMPoints: {otherUserData.GetTotalTMPoints()}");
                }

                // -> Write 'ChallengeTMPoint' into 'LeaderboardTMChallenge'
                if (otherUserData.GetTotalTMPoints() > 0)
                {
                    _savingWrapper.ForceSaveAnyUser(ECategoryNode.LeaderboardTMChallenge, eachData.Key, EValueNode.ChallengeTMPoint, otherUserData.GetTotalTMPoints());

                    Debug.LogWarning($"({index}) GetChallengeTMPoints: {otherUserData.GetChallengeTMPoints()}");
                }

                // -> Check 'FirstUploadTimeOfDayTM' is TODAY, if so add 'TodayTMPoint' into 'LeaderboardTMToday'
                if (otherUserData.GetFirstUploadTimeOfDayTM().Date == DateTime.Now.Date)
                {
                    _savingWrapper.ForceSaveAnyUser(ECategoryNode.LeaderboardTMToday, eachData.Key, EValueNode.TodayTMPoint, otherUserData.GetTodayTMPoints());

                    Debug.LogWarning($"({index}) IS TODAY & GetTodayTMPoints: {otherUserData.GetTodayTMPoints()}");
                }
            }

            _firstTime = true;
        }
        #endregion
    }
}