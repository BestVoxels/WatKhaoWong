using UnityEngine.Pool;
using UnityEngine;

namespace WatKhaoWong.UI.Retreats
{
    public class StayEntryRowUIPool : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("StayEntryRowUIPool Stuffs")]
        [SerializeField] private StayEntryRowUI _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        [Space]
        [SerializeField] private bool _isClearExistPrefabs = true;
        [Range(10, 150)]
        [SerializeField] private int _poolSize = 100;
        [SerializeField] private bool _preWarmOnStart = false;
        #endregion



        #region --Properties-- (Auto)
        public IObjectPool<StayEntryRowUI> Pool { get; private set; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            Pool = new ObjectPool<StayEntryRowUI>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool, OnDestroyItem, collectionCheck: true, defaultCapacity: _poolSize, maxSize: _poolSize);
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
                    StayEntryRowUI rowUI = CreatePoolItem();

                    rowUI.Release();  // OR Pool.Release(rowUI);
                }
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~ObjectPool~
        private StayEntryRowUI CreatePoolItem()
        {
            StayEntryRowUI rowUI = Instantiate(_rowPrefab, _spawnParent);
            rowUI.OnCreatedByPool(Pool);

            return rowUI;
        }

        private void OnTakeFromPool(StayEntryRowUI rowUI)
        {
            rowUI.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(StayEntryRowUI rowUI)
        {
            rowUI.gameObject.SetActive(false);
        }

        // For Resources Saving Purpose, it needs memory for keeping items around. (If the pool capacity is reached then any items returned will be destroyed, but will create more if needed)
        private void OnDestroyItem(StayEntryRowUI rowUI)
        {
            Destroy(rowUI.gameObject);
        }
        #endregion
    }
}