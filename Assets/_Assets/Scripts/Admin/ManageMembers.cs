using UnityEngine.Localization;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Attributes;

// TODO ---REMOVE when don't have to Test Upload/Download/SaveImage Features!!!---
// TODO ---REMOVE Assembly Dependencies as well!!!---
using WatKhaoWong.SceneManagement;
using WatKhaoWong.Utils.Core;
using UnityEngine.UI;
using TMPro;
// TODO ---REMOVE when don't have to Test Upload/Download/SaveImage Features!!!---

namespace WatKhaoWong.Admin
{
    public class ManageMembers : Page
    {
        #region --Fields-- (Inspector)
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Manage Members Text")]
        [field: SerializeField] public LocalizedString TotalUsersText { get; private set; }
        [field: SerializeField] public LocalizedString ActiveStayText { get; private set; }

        [field: Space]

        [field: Header("Manage Members - Settings")]
        [field: SerializeField] public string ValueTextFormatBegin { get; private set; } = "<space=25><b><cspace=-3>";
        [field: SerializeField] public string ValueTextFormatEnd { get; private set; } = "</cspace></b>";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("ManageMembers UI Event")]
        [SerializeField] private UnityEvent _onUserProfileClick;
        [SerializeField] private UnityEvent _onUserStatsClick;
        [Space]
        [SerializeField] private UnityEvent _onSearchEditMemberButtonClick;
        [SerializeField] private UnityEvent _onStayApprovalButtonClick;
        [SerializeField] private UnityEvent _onActivityManagementButtonClick;
        [SerializeField] private UnityEvent _onRegisterMemberButtonClick;
        #endregion
        


        #region --Methods-- (Custom PUBLIC) ~Page UI Buttons~
        public void OnUserProfileClick()
        {
            _onUserProfileClick?.Invoke();
        }

        public void OnUserStatsClick()
        {
            _onUserStatsClick?.Invoke();
        }

        public void OnSearchEditMemberButtonClick()
        {
            _onSearchEditMemberButtonClick?.Invoke();
        }

        public void OnStayApprovalButtonClick()
        {
            _onStayApprovalButtonClick?.Invoke();
        }

        public void OnActivityManagementButtonClick()
        {
            _onActivityManagementButtonClick?.Invoke();
        }

        public void OnRegisterMemberButtonClick()
        {
            _onRegisterMemberButtonClick?.Invoke();
        }
        #endregion



        // TODO ---REMOVE when don't have to Test Upload/Download/SaveImage Features!!!---
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Image _image;

        private SavingWrapper _savingWrapper;
        private Texture2D _downloadedTexture;

        private void Awake() => _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        
        public void TestUploadImage()
        {
            NativeGallery.GetImageFromGallery( async path =>
            {
                _statusText.text = $"Path : {path}";

                if (path == null) return;


                // Create Texture from selected image
                Texture2D texture = await NativeGallery.LoadImageAtPathAsync(path, 2048, false);
                if (texture == null)
                {
                    _statusText.text = $"Couldn't load texture from path : {path}";
                    return;
                }

                // Upload to Server
                System.Collections.Generic.Dictionary<string, string> customMetadata = new System.Collections.Generic.Dictionary<string, string>()
                {
                    { "Data 1", "..." },
                    { "Data 2", "..." }
                };
 
                bool result = await _savingWrapper.UploadImage($"Users/{FirebaseUtils.CurrentUserID}/Profile/FromPhone.jpg", texture, customMetadata, 5);

                _statusText.text = $"{_statusText.text}\nUpload Result : {result}";

                // To avoid memory leaks
                Destroy(texture);
            });
        }
        
        public async void TestDownloadImageNAssign()
        {
            // Download from Server
            _downloadedTexture = await _savingWrapper.DownloadImage($"Users/{FirebaseUtils.CurrentUserID}/Profile/FromPhone.jpg", 5);

            // Assign to Image
            Sprite sprite = Sprite.Create(
                    _downloadedTexture,
                    new Rect(0, 0, _downloadedTexture.width, _downloadedTexture.height),
                    new Vector2(0.5f, 0.5f) // center pivot
                    );

            AspectRatioFitter aspectFitter = _image.GetComponent<AspectRatioFitter>();
            float aspect = (float)_downloadedTexture.width / _downloadedTexture.height;
            aspectFitter.aspectRatio = aspect;

            _image.sprite = sprite;
        }

        public void TestSaveImageToGallery()
        {
            // Save the screenshot to Gallery/Photos
            NativeGallery.SaveImageToGallery(_downloadedTexture, "Firebase Storage", "FromServer.jpg", (success, path) => _statusText.text = $"Media save result: {success} {path}");
        }
        // TODO ---REMOVE when don't have to Test Upload/Download/SaveImage Features!!!---
    }
}