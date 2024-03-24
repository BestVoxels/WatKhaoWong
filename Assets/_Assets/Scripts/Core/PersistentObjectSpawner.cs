using UnityEngine;

namespace WatKhaoWong.Core
{
    /// <summary>
    /// An alternative to using the singleton pattern. Will handle spawning a
    /// prefab only once across multiple scenes.
    ///
    /// Place this in a prefab that exists in every scene. Point to another
    /// prefab that contains all GameObjects that should be singletons. The
    /// class will spawn the prefab only once and set it to persist between
    /// scenes.
    ///
    /// ***** if assign with 'PersistentObjects' prefab to create, No Need to Create EventSystem cuz it contains EventSystem gameObject underneath. *****
    /// </summary>
    public class PersistentObjectSpawner : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Tooltip("This Prefab will only be spawned once and persisted between scenes.")]
        [SerializeField] private GameObject _persistentObjectPrefab;
        #endregion



        #region --Fields-- (In Class)
        private static bool _hasSpawned = false;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            if (_hasSpawned) return;

            SpawnPersistentObject();

            _hasSpawned = true;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void SpawnPersistentObject()
        {
            GameObject persistentObject = Instantiate(_persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
        #endregion
    }
}