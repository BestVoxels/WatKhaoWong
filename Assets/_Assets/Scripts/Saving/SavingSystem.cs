using System.Threading.Tasks;
using UnityEngine;
using Firebase.Database;

namespace WatKhaoWong.Saving
{
    /// <summary>
    /// This component provides the interface to the saving system. It provides
    /// methods to save and restore a scene.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    ///
    /// Firebase Database Note - You can get an instance by calling 'DefaultInstance' . To access a location in the database and read or write data, use 'GetReference()'
    /// </summary>
    public class SavingSystem : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private FirebaseDatabase _database;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            //DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;
            _database = FirebaseDatabase.DefaultInstance;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// Save Value to Firebase Database.
        /// </summary>
        public void Save(string path, object saveValue)
        {
            Debug.Log($"Called \"Save();\" value of ({saveValue}) with path ({path})");
            
            _database.GetReference(path).SetValueAsync(saveValue);
        }

        /// <summary>
        /// Load Value from Firebase Database.
        /// </summary>
        public async Task<DataSnapshot> Load(string path)
        {
            Debug.Log($"Called \"Load();\" with path ({path})");

            DataSnapshot data = null;
            try
            {
                data = await _database.GetReference(path).GetValueAsync();
            }
            catch (Firebase.FirebaseException e)
            {
                Debug.LogError($"Load Save from database encountered an Error : ({e.ErrorCode}) {e.Message}");
            }

            if (data != null && data.Exists == false) return null; // Have to check if SaveExists() and return null, so other class can check using 'null', ex-AccountData.cs

            return data;
        }

        /// <summary>
        /// Load Bunches of Values from Firebase Database.
        /// Sort by Child Value, for more details check 'OrderByChild vs OrderByKey vs OrderByValue - ChatGPT link' under 'C# DOC' NOTE
        /// </summary>
        public async Task<DataSnapshot> LoadAndSortByChildValue(string path, string childNode, int limitNumber)
        {
            Debug.Log($"Called \"LoadAndSortByChildValue();\" with path ({path}) and sort by child value of ({childNode})");

            DataSnapshot data = null;
            try
            {
                data = await _database.GetReference(path).OrderByChild(childNode).LimitToLast(limitNumber).GetValueAsync();
            }
            catch (Firebase.FirebaseException e)
            {
                Debug.LogError($"Load Save from database encountered an Error : ({e.ErrorCode}) {e.Message}");
            }

            if (data != null && data.Exists == false) return null; // Have to check if SaveExists() and return null, so other class can check using 'null', ex-AccountData.cs

            return data;
        }

        /// <summary>
        /// Delete Value from Firebase Database.
        /// </summary>
        public void Delete(string path)
        {
            Debug.Log($"Called \"Delete();\" with path ({path})");

            _database.GetReference(path).RemoveValueAsync();
        }

        /// <summary>
        /// Check if Save Exists.
        /// Don't call this as checker for call 'Load()' because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.
        /// </summary>
        public async Task<bool> IsSaveExists(string path)
        {
            Debug.Log($"Called \"SaveExists(); + Load()\" with path ({path})");

            DataSnapshot data = null;
            try
            {
                data = await _database.GetReference(path).GetValueAsync();
            }
            catch (Firebase.FirebaseException e)
            {
                Debug.LogError($"Load Save from database encountered an Error : ({e.ErrorCode}) {e.Message}");
            }

            return data.Exists;
        }
        #endregion

        

        #region --Methods-- (Custom PRIVATE)
        #endregion
    }
}