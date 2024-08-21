using System;
using UnityEngine;

namespace WatKhaoWong.Leaderboards
{
    public class Leaderboard : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Leaderboard Settings")]
        [SerializeField] private ECategory _category;
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
    }
}