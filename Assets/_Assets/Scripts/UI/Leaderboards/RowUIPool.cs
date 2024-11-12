using UnityEngine.Pool;
using UnityEngine;

namespace WatKhaoWong.UI.Leaderboards
{
    public class RowUIPool : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("RowUIPool Stuffs")]
        [SerializeField] private RowUI _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        [Space]
        [SerializeField] private bool _isClearExistPrefabs = true;
        [Range(10, 150)]
        [SerializeField] private int _poolSize = 100;
        [SerializeField] private bool _preWarmOnStart = false;
        #endregion



        #region --Properties-- (Auto)
        public IObjectPool<RowUI> Pool { get; private set; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            Pool = new ObjectPool<RowUI>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool, OnDestroyItem, collectionCheck: true, defaultCapacity: _poolSize, maxSize: _poolSize);
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
                    RowUI rowUI = CreatePoolItem();

                    rowUI.Release();  // OR Pool.Release(rowUI);
                }
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~ObjectPool~
        private RowUI CreatePoolItem()
        {
            RowUI rowUI = Instantiate(_rowPrefab, _spawnParent);
            rowUI.Setup(Pool);

            return rowUI;
        }

        private void OnTakeFromPool(RowUI rowUI)
        {
            rowUI.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(RowUI rowUI)
        {
            rowUI.gameObject.SetActive(false);
        }

        // For Resources Saving Purpose, it needs memory for keeping items around. (If the pool capacity is reached then any items returned will be destroyed, but will create more if needed)
        private void OnDestroyItem(RowUI rowUI)
        {
            Destroy(rowUI.gameObject);
        }
        #endregion
    }
}