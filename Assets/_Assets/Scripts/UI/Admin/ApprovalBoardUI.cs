using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Admin;
using WatKhaoWong.Retreats;
using WatKhaoWong.Attributes;
using WatKhaoWong.Utils.Localization;
using Firebase.Database;

namespace WatKhaoWong.UI.Admin
{
    [RequireComponent(typeof(ApprovalRowUIPool))]
    public class ApprovalBoardUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Board Stuffs")]
        [SerializeField] private TMP_Text _dataIndicatorText;
        [Space]
        [SerializeField] private Transform _tabsTransform;
        #endregion



        #region --Fields-- (In Class)
        private List<ApprovalRowUI> _activeRowUIs = new List<ApprovalRowUI>();

        private ApprovalBoard _board;
        private ApprovalRowUIPool _rowUIPool;
        private GameObject _player;
        private ApprovalRowUI.CacheData _approvalRowUICacheData;
        #endregion



        #region --Fields-- (Constant)
        private const float WaitAsyncTimeOut = 10f;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");

            _board = _player.GetComponentInChildren<ApprovalBoard>();
            _rowUIPool = GetComponent<ApprovalRowUIPool>();
            InitApprovalRowUICacheData();

            _board.OnIsBoardExistsUpdated += ShowHideDataIndicatorText;
            UIRefresher.OnApprovalBoardRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
        }

        private void Start()
        {
            InitialSetup();

            RefreshUI();
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

            //+Prevent some ApprovalBoardUI GameObject show Empty Data (No Rows), solve by make ApprovalBoardUI GameObject that comes after wait first then loads when Async is done.
            float timer = 0f;
            while (ApprovalBoard.IsAsyncRunning == true)
            {
                timer += Time.deltaTime;

                if (timer >= WaitAsyncTimeOut) return;

                await Task.Delay(100);
            }

            ClearRows(); //+Prevent duplicates Rows Bug.

            ushort rowCounter = 1;
            await foreach ((StayEntry stayEntry, string keyId, DataSnapshot dataSnapShot) rowData in _board.GetRows())
            {
                if (rowData.stayEntry == null) continue;

                ApprovalRowUI createdPrefab = _rowUIPool.Pool.Get();

                createdPrefab.transform.SetSiblingIndex(rowCounter - 1); // -1 bcuz Index starts at 0.
                createdPrefab.Setup(rowData, rowCounter, _board.Category, _approvalRowUICacheData);

                _activeRowUIs.Add(createdPrefab);

                ++rowCounter;
            }
        }

        private void ClearRows()
        {
            foreach (ApprovalRowUI eachRow in _activeRowUIs)
                eachRow.Release();

            _activeRowUIs.Clear();
        }

        private void InitApprovalRowUICacheData()
        {
            _approvalRowUICacheData = new ApprovalRowUI.CacheData
            {
                Player = _player,
                ApprovalRow = _player.GetComponentInChildren<ApprovalRow>(),
                SetTimePopup = _player.GetComponentInChildren<AccommodationSetTimePopup>(),
                UserInfo = _player.GetComponentInChildren<UserInfo>(),
                Localizer = FindAnyObjectByType<Localizer>(),
                ServerTime = FindAnyObjectByType<ServerTime>(),
                ApprovalNoPopupUI = FindAnyObjectByType<ApprovalNoPopupUI>(FindObjectsInactive.Include),
                ApprovalYesPopupUI = FindAnyObjectByType<ApprovalYesPopupUI>(FindObjectsInactive.Include)
            };
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~FilterButtons~
        private void SetupFilterButtonsUI()
        {
            foreach (ApprovalFilterUI button in _tabsTransform.GetComponentsInChildren<ApprovalFilterUI>())
            {
                button.Setup(_board);
            }
        }

        private void UpdateFilterButtonsUI()
        {
            foreach (ApprovalFilterUI button in _tabsTransform.GetComponentsInChildren<ApprovalFilterUI>())
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

        private void ShowHideDataIndicatorText()
        {
            _dataIndicatorText.gameObject.SetActive(_board.IsBoardExists() == false);
        }
        #endregion
    }
}