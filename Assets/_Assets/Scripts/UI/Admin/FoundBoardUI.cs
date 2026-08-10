using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Admin;
using WatKhaoWong.Retreats;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.Localization;
using WatKhaoWong.Identities;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.UI.Admin
{
    [RequireComponent(typeof(FoundRowUIPool))]
    public class FoundBoardUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Board Stuffs")]
        [SerializeField] private ESearchPanelLocation _locationForSearchPanel;
        [Space]
        [SerializeField] private GameObject _noDataPanel;
        [Space]
        [SerializeField] private Transform _tabsTransform;
        [Space]
        [SerializeField] private Button _registerMemberButton;
        #endregion



        #region --Fields-- (In Class)
        private List<FoundRowUI> _activeRowUIs = new List<FoundRowUI>();

        private FoundBoard _board;
        private FoundRowUIPool _rowUIPool;
        private SearchPanel _searchPanel;
        private GameObject _player;
        private FoundRowUI.CacheData _foundRowUICacheData;
        #endregion



        #region --Fields-- (Constant)
        private const float WaitAsyncTimeOut = 10f;
        private const float WaitUIToTurnOffOnStartTime = 5f;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _board = _player.GetComponentInChildren<FoundBoard>();
            _rowUIPool = GetComponent<FoundRowUIPool>();
            _searchPanel = _player.GetComponentInChildren<SearchPanel>();

            InitFoundRowUICacheData();

            _registerMemberButton.onClick.AddListener(RegisterMember);

            _board.OnIsBoardExistsUpdated += ShowHideNoDataPanel;
        }

        private void Start()
        {
            InitialSetup();
        }

        private async void OnEnable()
        {
            if (Time.time < WaitUIToTurnOffOnStartTime) return; // Prevent OnEnable() on first Start when UI are seting itself which then it will hide itself. We only want OnEnable() when user open UI.
            if (!FirebaseUtils.IsAuthenticated()) return;
            if (!await MyUserData.IsAdmin()) return;

            // Use OnDisable()/OnEnable() because don't want UI to update on the background.
            UIRefresher.OnFoundBoardRefreshed += RefreshUI;
            _searchPanel.OnUIUpdated += RefreshUI;

            RefreshUI();
        }

        private async void OnDisable()
        {
            if (!await MyUserData.IsAdmin()) return;

            // Use OnDisable()/OnEnable() because don't want UI to update on the background.
            UIRefresher.OnFoundBoardRefreshed -= RefreshUI;
            _searchPanel.OnUIUpdated -= RefreshUI;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void InitialSetup()
        {
            SetupFilterButtonsUI();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Row~
        private async Task BuildRows()
        {
            ClearRows();

            //+Prevent some FoundBoardUI GameObject show Empty Data (No Rows), solve by make FoundBoardUI GameObject that comes after wait first then loads when Async is done.
            float timer = 0f;
            while (FoundBoard.IsAsyncRunning == true)
            {
                timer += Time.deltaTime;

                if (timer >= WaitAsyncTimeOut) return;

                await Task.Delay(100);
            }

            ClearRows(); //+Prevent duplicates Rows Bug.

            ushort rowCounter = 1;
            await foreach (IUserData userData in _board.GetRows(_locationForSearchPanel))
            {
                if (userData == null || rowCounter > _board.MaxRowNumber) continue;

                FoundRowUI createdPrefab = _rowUIPool.Pool.Get();

                createdPrefab.transform.SetSiblingIndex(rowCounter - 1); // -1 bcuz Index starts at 0.
                createdPrefab.Setup(userData, rowCounter, _board.Category, _foundRowUICacheData);

                _activeRowUIs.Add(createdPrefab);

                ++rowCounter;
            }
        }

        private void ClearRows()
        {
            foreach (FoundRowUI eachRow in _activeRowUIs)
                eachRow.Release();

            _activeRowUIs.Clear();
        }

        private void InitFoundRowUICacheData()
        {
            _foundRowUICacheData = new FoundRowUI.CacheData
            {
                Player = _player,
                FoundRow = _player.GetComponentInChildren<FoundRow>(),
                UserInfo = _player.GetComponentInChildren<UserInfo>(),
                Localizer = FindAnyObjectByType<Localizer>(),
                ServerTime = FindAnyObjectByType<ServerTime>()
            };
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~FilterButtons~
        private void SetupFilterButtonsUI()
        {
            foreach (FoundFilterUI button in _tabsTransform.GetComponentsInChildren<FoundFilterUI>())
            {
                button.Setup(_board);
            }
        }

        private void UpdateFilterButtonsUI()
        {
            foreach (FoundFilterUI button in _tabsTransform.GetComponentsInChildren<FoundFilterUI>())
            {
                button.RefreshUI();
            }
        }
        #endregion



        #region --Methods-- (Subscriber)
        private async void RefreshUI()
        {
            UpdateFilterButtonsUI();

            await BuildRows();
        }

        private void ShowHideNoDataPanel()
        {
            _noDataPanel.SetActive(_board.IsBoardExists() == false);
        }

        private void RegisterMember()
        {
            _board.OnRegisterMemberButtonClick();
        }
        #endregion
    }
}