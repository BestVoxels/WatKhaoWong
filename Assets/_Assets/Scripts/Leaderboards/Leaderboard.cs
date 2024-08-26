using System.Collections.Generic;
using System;
using UnityEngine;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Identity;
using Firebase.Database;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Leaderboards
{
    public class Leaderboard : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Leaderboard Settings")]
        [SerializeField] private ECategory _category;
        [Range(1, 200)]
        [SerializeField] private int _maxRowNumber = 100;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Leaderboard Filter Settings")]
        [field: SerializeField] public Color32 SelectedColor { get; private set; }
        [field: SerializeField] public Color32 UnselectedColor { get; private set; }

        [field: Space]

        [field: Header("Leaderboard Status Text")]
        [field: SerializeField] public string NoChallengeText { get; private set; } = "No active Challenge at the moment";
        [field: SerializeField] public string CountDownChallengeTextBegin { get; private set; } = $"Challenge ends in ";
        [field: SerializeField] public string CountDownChallengeTextEnd { get; private set; } = $" days!";
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnCategoryChanged;
        #endregion



        #region --Fields-- (In Class)
        private List<DataSnapshot> _allTimeRows = new List<DataSnapshot>();
        private ushort _myUserRank = 9999;
        private bool _isMeInLeaderboard = false;
        private bool _isAsyncRunning = false;

        private SavingWrapper _savingWrapper;
        #endregion



        #region --Properties-- (With Backing Fields)
        public ECategory Category
        {
            get => _category;

            set
            {
                _category = value;

                OnCategoryChanged?.Invoke();
            }
        }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public ushort GetMyUserRank()
        {
            if (_isMeInLeaderboard == false)
                _myUserRank = (ushort)_maxRowNumber;

            return _myUserRank;
        }

        public bool IsMeInLeaderboard() => _isMeInLeaderboard;

        public async IAsyncEnumerable<OtherUserData> GetRows()
        {
            if (_isAsyncRunning) yield break; // Prevent duplicates Rows Bug. Since we are dealing with 'await' so we only allow ONE instance of this method to run at a time.

            _isAsyncRunning = true;
            switch (Category)
            {
                case ECategory.AllTime:
                    await foreach (DataSnapshot eachData in GetAllTimeRows())
                    {
                        yield return new OtherUserData(eachData);
                    }
                    break;

                case ECategory.Today:
                    break;

                case ECategory.Challenge:
                    break;
            }
            _isAsyncRunning = false;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async IAsyncEnumerable<DataSnapshot> GetAllTimeRows()
        {
            // *** First Initialize List & also Return Asynchronous one by one when loaded from server. ***
            if (_allTimeRows.Count == 0)
            {
                ushort index = 0;
                await foreach (DataSnapshot each in _savingWrapper.LoadAndSortByChildValue(EValueNode.TotalTMPoint, _maxRowNumber))
                {
                    ++index;
                    if (each.Key.Equals(FirebaseUtils.CurrentUserID))
                    {
                        _myUserRank = index;
                        _isMeInLeaderboard = true;
                    }
                    
                    _allTimeRows.Add(each);

                    yield return each;
                }

                yield break; // Important to stop here because 'await' will resume call and if we don't end here it will run code below too.
            }
            
            // *** Return List as Synchronous ***
            foreach (DataSnapshot each in _allTimeRows)
                yield return each;
        }
        #endregion
    }
}