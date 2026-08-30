using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WatKhaoWong.Utils
{
    public class A4DocumentGenerator : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("A4 Rendering")]
        [SerializeField] private GameObject _a4DocumentGameObject;
        [SerializeField] private Camera _a4Camera;
        [SerializeField] private RenderTexture _a4RenderTexture;

        [Header("National ID")]
        [SerializeField] private Image _nationalIdImage;

        [Header("User Information")]
        [SerializeField] private TMP_Text _nameValue;
        [SerializeField] private TMP_Text _cardTypeValue;
        [SerializeField] private TMP_Text _idNumberValue;
        [SerializeField] private TMP_Text _expireDateValue;
        [SerializeField] private TMP_Text _birthDateValue;
        [SerializeField] private TMP_Text _ageValue;
        [SerializeField] private TMP_Text _addressValue;
        [SerializeField] private TMP_Text _phoneNumberValue;
        [SerializeField] private TMP_Text _medicalConditionValue;
        [SerializeField] private TMP_Text _urgentPhoneNumberValue;
        [SerializeField] private TMP_Text _urgentRelationValue;
        [SerializeField] private TMP_Text _lineIdValue;
        [SerializeField] private TMP_Text _facebookValue;
        [SerializeField] private TMP_Text _instagramValue;
        [SerializeField] private TMP_Text _tiktokValue;

        [Header("Staff Information")]
        [SerializeField] private TMP_Text _buildingValue;
        [SerializeField] private TMP_Text _roomNumberValue;
        [SerializeField] private TMP_Text _stayDaysValue;
        [SerializeField] private TMP_Text _plateNumberValue;
        #endregion

        
        
        #region --Fields-- (In Class)
        private Texture2D _generatedTexture;
        private string _filePath;
        #endregion



        #region --Methods-- (Built In)
        private void OnDestroy()
        {
            if (_generatedTexture != null)
            {
                Destroy(_generatedTexture);
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SetUserData(string fullName, string cardType, string idNumber, string expireDate, string birthDate, string age, string address, string phoneNumber, string medicalCondition, string urgentPhoneNumber, string urgentRelation, string line, string fb, string ig, string tt, string building, string roomNumber, string stayDays, string plateNumber)
        {
            _nameValue.text = fullName;
            _cardTypeValue.text = cardType;
            _idNumberValue.text = idNumber;
            _expireDateValue.text = expireDate;
            _birthDateValue.text = birthDate;
            _ageValue.text = age;
            _addressValue.text = address;
            _phoneNumberValue.text = phoneNumber;
            _medicalConditionValue.text = medicalCondition;
            _urgentPhoneNumberValue.text = urgentPhoneNumber;
            _urgentRelationValue.text = urgentRelation;
            _lineIdValue.text = line;
            _facebookValue.text = fb;
            _instagramValue.text = ig;
            _tiktokValue.text = tt;

            _buildingValue.text = building;
            _roomNumberValue.text = roomNumber;
            _stayDaysValue.text = stayDays;
            _plateNumberValue.text = plateNumber;
        }
        
        public void SetNationalIdSprite(Sprite sprite)
        {
            if (_nationalIdImage == null)
            {
                Debug.LogError(
                    "A4 Document Generator: National ID Image is not assigned."
                );

                return;
            }

            _nationalIdImage.sprite = sprite;
            _nationalIdImage.preserveAspect = true;
        }

        /// <summary>
        /// Generates the A4 document and saves it as PNG.
        /// </summary>
        public void GenerateA4()
        {
            StartCoroutine(GenerateA4Coroutine());
        }
        
        public void ClearNationalIdImage()
        {
            if (_nationalIdImage != null)
            {
                _nationalIdImage.sprite = null;
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator GenerateA4Coroutine()
        {
            if (_a4Camera == null)
            {
                Debug.LogError("A4 Document Generator: A4 Camera is not assigned.");
                yield break;
            }

            if (_a4RenderTexture == null)
            {
                Debug.LogError("A4 Document Generator: A4 RenderTexture is not assigned.");
                yield break;
            }

            if (_a4RenderTexture.width != 2480 || _a4RenderTexture.height != 3508)
            {
                Debug.LogWarning(
                    $"A4 RenderTexture is {_a4RenderTexture.width}x{_a4RenderTexture.height}. " +
                    "Expected 2480x3508."
                );
            }

            // Make sure UI layout has finished updating.
            Canvas.ForceUpdateCanvases();

            _a4DocumentGameObject.gameObject.SetActive(true);
            _a4Camera.gameObject.SetActive(true);

            yield return new WaitForEndOfFrame();

            // Save current RenderTexture.
            RenderTexture previousActive = RenderTexture.active;

            // Render A4 camera into the A4 RenderTexture.
            RenderTexture.active = _a4RenderTexture;

            _a4Camera.targetTexture = _a4RenderTexture;
            _a4Camera.Render();

            // Create CPU-readable Texture2D.
            _generatedTexture = new Texture2D(
                _a4RenderTexture.width,
                _a4RenderTexture.height,
                TextureFormat.RGB24,
                false
            );

            _generatedTexture.ReadPixels(
                new Rect(
                    0,
                    0,
                    _a4RenderTexture.width,
                    _a4RenderTexture.height
                ),
                0,
                0
            );

            _generatedTexture.Apply();

            // Restore previous RenderTexture.
            RenderTexture.active = previousActive;

            // Encode PNG.
            byte[] pngBytes = _generatedTexture.EncodeToPNG();

            // Save to path.
            _filePath = Path.Combine(
                Application.persistentDataPath,
                "RegistrationForm.png"
            );

            File.WriteAllBytes(_filePath, pngBytes);

            // Debug.Log(
            //     $"A4 PNG ready:\n" +
            //     $"Path: {_filePath}\n" +
            //     $"Exists: {File.Exists(_filePath)}\n" +
            //     $"Size: {new FileInfo(_filePath).Length} bytes\n" +
            //     $"Resolution: {_a4RenderTexture.width}x{_a4RenderTexture.height}"
            // );

            // Share Image
            new NativeShare()
                .AddFile(_filePath)
                .SetTitle("Share Registration Form")
                .SetCallback((result, target) =>
                {
                    // Debug.Log(
                    //     $"Native Share finished.\n" +
                    //     $"Result: {result}\n" +
                    //     $"Target: {target}"
                    // );

                    _a4Camera.gameObject.SetActive(false);
                    _a4DocumentGameObject.SetActive(false);
                })
                .Share();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        #endregion
    }
}