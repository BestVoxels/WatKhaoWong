using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WatKhaoWong.Saving
{
    /// <summary>
    /// To be placed on any GameObject that has ISaveable components that
    /// require saving.
    ///
    /// This class gives the GameObject a unique ID in the scene file. The ID is
    /// used for saving and restoring the state related to this GameObject. This
    /// ID can be manually override to link GameObjects between scenes (such as
    /// recurring characters, the player or a score board). Take care not to set
    /// this in a prefab unless you want to link all instances between scenes.
    /// </summary>
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        // CONFIG DATA
        [Tooltip("The unique ID is automatically generated in a scene file if " +
        "left empty. Do not set in a prefab unless you want all instances to " +
        "be linked.")]
        [SerializeField] string uniqueIdentifier = "";

        // CACHED STATE
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        /// <summary>
        /// Will capture the state of all `ISaveables` on this component and
        /// return a `System.Serializable` object that can restore this state
        /// later.
        /// </summary>
        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();  // Currently 'saveable' for CompileTime Type is 'ISaveable' BUT on RunTime Type it is 'RPG.Movement.Mover', 'RPG.Core.Health' etc.
            }
            return state;
        }

        /// <summary>
        /// Will restore the state that was captured by `CaptureState`.
        /// </summary>
        /// <param name="state">
        /// The same object that was returned by `CaptureState`.
        /// </param>
        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString(); // Currently 'saveable' for CompileTime Type is 'ISaveable' BUT on RunTime Type it is 'RPG.Movement.Mover', 'RPG.Core.Health' etc.
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }

        // PRIVATE

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return; // For not generating when in Prefab Window Mode


            SerializedObject serializedObject = new SerializedObject(this); // Use SerializedObject to change the value serialized to a scene file or prefab we should instead of modify directly because this way we can make sure that it get saved to the scene file Instead of in Unity Scene Memory which is just cache and will be gone when close and open program.
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue)) // !IsUnique() is when duplicate gameObject in scene it should generate new one
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties(); // Notify Unity Scene these is a changes in scene, so we save the scene
            }

            globalLookup[property.stringValue] = this;
        }
#endif

        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;

            if (globalLookup[candidate] == this) return true;

            if (globalLookup[candidate] == null) // Check An object you have a reference to, has been destroyed by Unity
            {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}