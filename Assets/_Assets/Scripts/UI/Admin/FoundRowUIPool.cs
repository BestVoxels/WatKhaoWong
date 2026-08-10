using UnityEngine.Pool;
using UnityEngine;

namespace WatKhaoWong.UI.Admin
{
    public class FoundRowUIPool : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("RowUIPool Stuffs")]
        [SerializeField] private FoundRowUI _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        [Space]
        [SerializeField] private bool _isClearExistPrefabs = true;
        [Range(10, 150)]
        [SerializeField] private int _poolSize = 100;
        [SerializeField] private bool _preWarmOnStart = false;
        #endregion



        #region --Properties-- (Auto)
        public IObjectPool<FoundRowUI> Pool { get; private set; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            Pool = new ObjectPool<FoundRowUI>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool, OnDestroyItem, collectionCheck: true, defaultCapacity: _poolSize, maxSize: _poolSize);
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
                    FoundRowUI foundRowUI = CreatePoolItem();

                    foundRowUI.Release();  // OR Pool.Release(foundRowUI);
                }
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~ObjectPool~
        private FoundRowUI CreatePoolItem()
        {
            FoundRowUI foundRowUI = Instantiate(_rowPrefab, _spawnParent);
            foundRowUI.OnCreatedByPool(Pool);

            return foundRowUI;
        }

        private void OnTakeFromPool(FoundRowUI foundRowUI)
        {
            foundRowUI.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(FoundRowUI foundRowUI)
        {
            foundRowUI.gameObject.SetActive(false);
        }

        // For Resources Saving Purpose, it needs memory for keeping items around. (If the pool capacity is reached then any items returned will be destroyed, but will create more if needed)
        private void OnDestroyItem(FoundRowUI foundRowUI)
        {
            Destroy(foundRowUI.gameObject);
        }
        #endregion
    }
}