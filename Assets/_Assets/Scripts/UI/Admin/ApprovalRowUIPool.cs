using UnityEngine.Pool;
using UnityEngine;

namespace WatKhaoWong.UI.Admin
{
    public class ApprovalRowUIPool : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("RowUIPool Stuffs")]
        [SerializeField] private ApprovalRowUI _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        [Space]
        [SerializeField] private bool _isClearExistPrefabs = true;
        [Range(10, 150)]
        [SerializeField] private int _poolSize = 100;
        [SerializeField] private bool _preWarmOnStart = false;
        #endregion



        #region --Properties-- (Auto)
        public IObjectPool<ApprovalRowUI> Pool { get; private set; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            Pool = new ObjectPool<ApprovalRowUI>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool, OnDestroyItem, collectionCheck: true, defaultCapacity: _poolSize, maxSize: _poolSize);
        }

        private void Start()
        {
            if (_isClearExistPrefabs)
            {
                foreach (Transform eachChild in _spawnParent)
                    Destroy(eachChild.gameObject);
            }

            if (_preWarmOnStart)
            {
                for (int i = 0; i < _poolSize; i++)
                {
                    ApprovalRowUI approvalRowUI = CreatePoolItem();

                    approvalRowUI.Release();  // OR Pool.Release(approvalRowUI);
                }
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~ObjectPool~
        private ApprovalRowUI CreatePoolItem()
        {
            ApprovalRowUI approvalRowUI = Instantiate(_rowPrefab, _spawnParent);
            approvalRowUI.OnCreatedByPool(Pool);

            return approvalRowUI;
        }

        private void OnTakeFromPool(ApprovalRowUI approvalRowUI)
        {
            approvalRowUI.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(ApprovalRowUI approvalRowUI)
        {
            approvalRowUI.gameObject.SetActive(false);
        }

        // For Resources Saving Purpose, it needs memory for keeping items around. (If the pool capacity is reached then any items returned will be destroyed, but will create more if needed)
        private void OnDestroyItem(ApprovalRowUI approvalRowUI)
        {
            Destroy(approvalRowUI.gameObject);
        }
        #endregion
    }
}