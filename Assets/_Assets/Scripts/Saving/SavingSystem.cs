using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Firebase.Database;
using Firebase.Storage;
using WatKhaoWong.Utils.Core;

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
        private FirebaseStorage _storage;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            //DatabaseReference root = FirebaseDatabase.DefaultInstance.RootReference;
            _database = FirebaseDatabase.DefaultInstance;
            _storage = FirebaseStorage.DefaultInstance;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Firebase Storage~
        /// <summary>
        /// Upload Image to Firebase Storage. Compress to JPG (quality 90) first to reduce file size.
        /// </summary>
        public async Task<bool> UploadImage(string firebasePath, Texture2D textureImage, Dictionary<string, string> customMetadata, byte maxImageSizeInMB)
        {
            byte[] bytesImage = Utilities.CompressToJPG(textureImage);

            Debug.Log($"File size after compress to JPG : {Utilities.GetImageSizeMB(bytesImage)}");

            // Save Full Size is fine, system has MB limiter. No need to Resize to scale down.
            if (Utilities.GetImageSizeMB(bytesImage) > maxImageSizeInMB)
            {
                return false;
            }
            // Make File Name Unique
            //path = System.IO.Path.Combine(path, $"{System.Guid.NewGuid()}.jpg");

            // For Adding Metadata to File
            MetadataChange metadataChange = new MetadataChange()
            {
                ContentEncoding = "image/jpg",
                CustomMetadata = customMetadata
            };
            // For Monitoring Upload Progress
            StorageProgress<UploadState> progress = new StorageProgress<UploadState>(state =>
            {
                // called periodically during the upload
                Debug.Log($"Progress: {state.BytesTransferred} of {state.TotalByteCount} bytes transferred.");
            });

            
            // UPLOAD
            StorageMetadata metadata = null;
            try
            {
                metadata = await _storage.GetReference(firebasePath).PutBytesAsync(bytesImage, metadataChange, progress);

                return true;
            }
            catch (Firebase.FirebaseException e)
            {
                Debug.LogError($"UploadImage to storage encountered an Error : ({e.ErrorCode}) {e.Message}");

                return false;
            }

            //// GET URL to see data that just Uploaded
            //Uri url = null;
            //try
            //{
            //    url = await _storage.GetReference(firebasePath).GetDownloadUrlAsync();
            //    Debug.Log($"Can Download Image from {url}");
            //}
            //catch (Firebase.FirebaseException e)
            //{
            //    Debug.LogError($"Get Download Url encountered an Error : ({e.ErrorCode}) {e.Message}");
            //}
        }

        /// <summary>
        /// Download Image from Firebase Storage
        /// </summary>
        public async Task<Texture2D> DownloadImage(string firebasePath, byte maxImageSizeInMB)
        {
            // For Monitoring Download Progress
            StorageProgress<DownloadState> progress = new StorageProgress<DownloadState>(state =>
            {
                // called periodically during the download
                Debug.Log($"Progress: {state.BytesTransferred} of {state.TotalByteCount} bytes downloaded.");
            });

            byte[] bytes = null;
            try
            {
                bytes = await _storage.GetReference(firebasePath).GetBytesAsync(maxImageSizeInMB * 1024 * 1024, progress);

                Debug.Log("Downloaded");
            }
            catch (Firebase.FirebaseException e)
            {
                Debug.LogError($"UploadImage to storage encountered an Error : ({e.ErrorCode}) {e.Message}");
            }

            Texture2D tex = new(2, 2);

            tex.LoadImage(bytes);

            return tex;
        }

        /// <summary>
        /// Delete File from Firebase Storage
        /// </summary>
        public void DeleteFile(string firebasePath)
        {
            Debug.Log($"Called \"DeleteFile();\" with Firebase Path ({firebasePath})");

            _storage.GetReference(firebasePath).DeleteAsync();
        }

        /// <summary>
        /// Upload File to Firebase Storage
        /// </summary>
        public async void UploadFile(string firebasePath, string localFilePath)
        {
            await _storage.GetReference(firebasePath).PutFileAsync("file://" + localFilePath); // URI path file:// prefix works both iOS and Android.
        }
        // ------How to use UploadFile()------
        //// Pick any file
        //NativeFilePicker.PickFile((path) =>
        //{
        //    if (path == null)
        //        _statusText.text = "Operation cancelled";
        //    else
        //    {
        //        _statusText.text = $"Picked file: {path}";

        //        _savingWrapper.UploadFile($"Users/{FirebaseUtils.CurrentUserID}/Profile/filename{System.IO.Path.GetExtension(path)}", path);
        //    }
        //});
        // ------

        /// <summary>
        /// Download File from Firebase Storage
        /// </summary>
        public async void DownloadFile(string firebasePath, string localDownloadPath)
        {
            await _storage.GetReference(firebasePath).GetFileAsync("file://" + localDownloadPath); // URI path file:// prefix works both iOS and Android.
        }
        // ------How to use DownloadFile()------
        //_savingWrapper.DownloadFile($"Users/{FirebaseUtils.CurrentUserID}/Profile/filename", Application.temporaryCachePath);

        //// Export the file
        //NativeFilePicker.ExportFile(Application.temporaryCachePath, (success) => Debug.Log("File exported: " + success));
        // ------
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Firebase Database~
        /// <summary>
        /// Save Value to Firebase Database.
        /// </summary>
        public void Save(string path, object saveValue)
        {
            //Debug.Log($"Called \"Save();\" value of ({saveValue}) with path ({path})");
            
            _database.GetReference(path).SetValueAsync(saveValue);
        }

        /// <summary>
        /// Load Value from Firebase Database.
        /// </summary>
        public async Task<DataSnapshot> Load(string path)
        {
            //Debug.Log($"Called \"Load();\" with path ({path})");
            
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
            //Debug.Log($"Called \"LoadAndSortByChildValue();\" with path ({path}) and sort by child value of ({childNode})");

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
            //Debug.Log($"Called \"Delete();\" with path ({path})");

            _database.GetReference(path).RemoveValueAsync();
        }

        /// <summary>
        /// Check if Save Exists.
        /// Don't call this as checker for call 'Load()' because it has to waste downloads amount of data, right now Load() already check for .Exists within itself.
        /// </summary>
        public async Task<bool> IsSaveExists(string path)
        {
            //Debug.Log($"Called \"SaveExists(); + Load()\" with path ({path})");

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



        #region --Methods-- (Custom PUBLIC) ~Firebase Database~  ~JSON~
        /// <summary>
        /// Save JSON Value to Firebase Database.
        /// </summary>
        public async Task SaveJson(string path, object dataObject)
        {
            string jsonString = JsonConvert.SerializeObject(dataObject);

            await _database.GetReference(path).SetRawJsonValueAsync(jsonString);
        }

        /// <summary>
        /// Load JSON Value from Firebase Database.
        /// </summary>
        public async Task<T> LoadJson<T>(string path)
        {
            DataSnapshot dataSnapshot = await Load(path);

            string jsonString = dataSnapshot.GetRawJsonValue();

            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        // --DELETE is same method as above--
        // -> _database.GetReference("Users/UserID/GeneralInfo/MedicalCondition").SetRawJsonValueAsync(json);
        // -> _database.GetReference("Users/UserID/GeneralInfo/MedicalCondition").RemoveValueAsync();
        // Can nail down to specific path as well "Users/UserID/GeneralInfo/MedicalCondition"
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Firebase Database~  ~JSON with Unique Key~
        /// <summary>
        /// Save JSON with Unique Key to Firebase Database.
        /// </summary>
        public async Task<string> SaveDataWithKey(string path, object dataObject)
        {
            string jsonString = JsonConvert.SerializeObject(dataObject);

            DatabaseReference databaseReference = _database.GetReference(path).Push();

            await databaseReference.SetRawJsonValueAsync(jsonString);

            return databaseReference.Key;
        }

        /// <summary>
        /// Load JSON with Unique Key from Firebase Database.
        /// </summary>
        public async IAsyncEnumerable<DataSnapshot> LoadChildren(string path)
        {
            IEnumerable<DataSnapshot> dataSnapshots = (await Load(path)).Children;

            foreach (DataSnapshot child in dataSnapshots)
            {
                yield return child;
            }
        }

        public async IAsyncEnumerable<T> LoadChildrenJson<T>(string path)
        {
            await foreach (DataSnapshot child in LoadChildren(path))
            {
                string jsonString = child.GetRawJsonValue();

                yield return JsonConvert.DeserializeObject<T>(jsonString);
            }
        }

        // --DELETE is same method as above--
        // -> _database.GetReference("Users/UserID/PastAccommodation").Push().SetRawJsonValueAsync(json);
        // -> _database.GetReference("Users/UserID/PastAccommodation/pushedKey").RemoveValueAsync();
        // Can nail down to specific path as well "Users/UserID/PastAccommodation/pushedKey/something"
        #endregion
    }
}