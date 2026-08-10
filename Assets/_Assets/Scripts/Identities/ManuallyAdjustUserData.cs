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
            {
                //ManuallyAdjustUsersScore();

                ManuallyCountTotalScoreFromLeaderboardTMChallenge();

                //ManuallyCountTotalScoreFromUsers();
            }
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
            await foreach (DataSnapshot eachData in _savingWrapper.LoadAndSortByChildValueReverse(ECategoryNode.Users, EValueNode.TotalTMPoint, 1000))
            {
                ++index;

                OtherUserData otherUserData = new OtherUserData(eachData);

                //// -> Delete 'ChallengeTMPoint' node
                //if (otherUserData.GetChallengeTMPoints() == 0)
                //{
                //    _savingWrapper.ForceDeleteAnyUser(ECategoryNode.Users, eachData.Key, EValueNode.ChallengeTMPoint);

                //    Debug.LogWarning($"({index})");
                //}


                //if (otherUserData.GetTotalTMPoints() > 0 && otherUserData.GetChallengeTMPoints() != otherUserData.GetTotalTMPoints())
                //{
                //    // -> Get 'TotalTMPoint' and copy into 'ChallengeTMPoint'
                //    _savingWrapper.ForceSaveAnyUser(ECategoryNode.Users, eachData.Key, EValueNode.ChallengeTMPoint, otherUserData.GetTotalTMPoints());

                //    // -> Write 'ChallengeTMPoint' into 'LeaderboardTMChallenge'
                //    _savingWrapper.ForceSaveAnyUser(ECategoryNode.LeaderboardTMChallenge, eachData.Key, EValueNode.ChallengeTMPoint, otherUserData.GetTotalTMPoints());

                //    Debug.LogWarning($"({index}) / ({eachData.Key}) This User's ChallengeTMPoints is NOT equals with TotalTMPoints. So Update on 'LeaderboardTMChallenge' and 'ChallengeTMPoints' nodes");
                //}

                //// -> Set 'ChallengeTMPoint' to 0. (WHEN CHALLENGE IS ENDED)
                //if (otherUserData.GetTotalTMPoints() > 0 && otherUserData.GetChallengeTMPoints() != otherUserData.GetTotalTMPoints())
                //{
                //    _savingWrapper.ForceSaveAnyUser(ECategoryNode.Users, eachData.Key, EValueNode.ChallengeTMPoint, 0);

                //    Debug.LogWarning($"({index}) / ({eachData.Key}) Set 'ChallengeTMPoints' to 0");
                //}

                //// -> Check IF User TodayTMPoint is not assign in TodayLeaderboard, IF SO add 'TodayTMPoint' into 'LeaderboardTMToday'
                //if (otherUserData.GetFirstUploadTimeOfDayTM().Date == DateTime.Now.Date)
                //{
                //    if ((await _savingWrapper.ForceIsSaveExists(ECategoryNode.LeaderboardTMToday, eachData.Key, EValueNode.TodayTMPoint)) == false)
                //    {
                //        _savingWrapper.ForceSaveAnyUser(ECategoryNode.LeaderboardTMToday, eachData.Key, EValueNode.TodayTMPoint, otherUserData.GetTodayTMPoints());

                //        Debug.LogWarning($"({index}) / ({eachData.Key}) This User has NO data in 'LeaderboardTMToday'");
                //    }
                //}

                //_firstTime = true;
                //Debug.LogWarning("DONE");
            }
        }

        /// <summary>
        /// Sum Total Score from 'LeaderboardTMChallenge' leaderboard
        /// </summary>
        private async void ManuallyCountTotalScoreFromLeaderboardTMChallenge()
        {
            ushort totalPeople = 0;
            ushort totalPeopleWithoutScore = 0;
            int totalScore = 0;

            await foreach (DataSnapshot eachData in _savingWrapper.LoadAndSortByChildValueReverse(ECategoryNode.LeaderboardTMChallenge, EValueNode.ChallengeTMPoint, 1000))
            {
                ++totalPeople;
                DataSnapshot data = eachData.Child("ChallengeTMPoint");
                if (!data.Exists)
                {
                    ++totalPeopleWithoutScore;
                    continue;
                }

                int eachUserScore = int.Parse(data.Value.ToString());

                totalScore += eachUserScore;

                Debug.LogWarning($"{totalPeople} : data.Child(\"ChallengeTMPoint\").Value -> {eachUserScore}");
            }

            Debug.LogWarning($"Total People : {totalPeople}");

            Debug.LogWarning($"Total People Without Score: {totalPeopleWithoutScore}");
            Debug.LogWarning($"Total People WITH Score: {totalPeople - totalPeopleWithoutScore}");

            Debug.LogWarning($"Total Score : {totalScore}");

            _firstTime = true;
            Debug.LogWarning("DONE");
        }

        /// <summary>
        /// Sum Total Score from 'LeaderboardTMChallenge' leaderboard
        /// </summary>
        private async void ManuallyCountTotalScoreFromUsers()
        {
            ushort totalPeople = 0;
            ushort totalPeopleWithoutScore = 0;
            int totalScore = 0;

            await foreach (DataSnapshot eachData in _savingWrapper.LoadAndSortByChildValueReverse(ECategoryNode.Users, EValueNode.TotalTMPoint, 1000))
            {
                ++totalPeople;
                DataSnapshot data = eachData.Child("TMPoints").Child("TotalTMPoint");
                if (!data.Exists)
                {
                    ++totalPeopleWithoutScore;
                    continue;
                }

                int eachUserScore = int.Parse(data.Value.ToString());

                totalScore += eachUserScore;

                Debug.LogWarning($"{totalPeople} : data.Child(\"TotalTMPoint\").Value -> {eachUserScore}");
            }

            Debug.LogWarning($"Total People : {totalPeople}");

            Debug.LogWarning($"Total People Without Score: {totalPeopleWithoutScore}");
            Debug.LogWarning($"Total People WITH Score: {totalPeople - totalPeopleWithoutScore}");

            Debug.LogWarning($"Total Score : {totalScore}");

            _firstTime = true;
            Debug.LogWarning("DONE");
        }
        #endregion
    }
}