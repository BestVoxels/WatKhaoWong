using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using WatKhaoWong.Utils.Core;
using WatKhaoWong.Attributes;
using WatKhaoWong.Identity;

namespace WatKhaoWong.Prays
{
    public class UndoPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Undo Popup Settings")]
        [Tooltip("This Numerical Value is represented as 'second' unit.")]
        [Range(0f, 10f)]
        [SerializeField] private float _uploadDelay = 3f;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("Undo Popup Status Text")]
        [field: SerializeField] public string StatusHeaderTextDefault { get; private set; } = "Uploading...";
        [field: SerializeField] public string StatusInfoTextDefault { get; private set; } = "Press Undo if you uplooad by mistake";
        [field: SerializeField] public string StatusHeaderTextDone { get; private set; } = "Uploaded!";
        [field: SerializeField] public string StatusInfoTextDoneBegin { get; private set; } = $"You have successfully upload (";
        [field: SerializeField] public string StatusInfoTextDoneEnd { get; private set; } = $") to the system. Awesome!";
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Undo Popup UI Event")]
        [SerializeField] private UnityEvent _onUndoButtonClick;
        [Space]
        [Header("Other Event")]
        [SerializeField] private UnityEvent _onUploadSucceed;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnUploadSucceed;
        public event Action<float> OnUploading;
        #endregion



        #region --Fields-- (In Class)
        private int _tmPoints;

        private Coroutine _previousCoroutine;
        private MyUserData _myUserData;
        #endregion



        #region --Properties-- (Computed)
        public string StatusInfoTextDone => StatusInfoTextDoneBegin + _tmPoints + StatusInfoTextDoneEnd;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void StartUploadToServer(int tmPoints)
        {
            if (tmPoints <= 0) return;

            _tmPoints = tmPoints;

            _previousCoroutine = StartCoroutine(UploadToServerDelay());
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnUndoButtonClick()
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _tmPoints = 0;

            _onUndoButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator UploadToServerDelay()
        {
            float timer = 0f;
            float uploadProgress01;
            while (timer < _uploadDelay)
            {
                timer += Time.deltaTime;

                uploadProgress01 = Utilities.Get01ValueFrom(0f, _uploadDelay, timer);
                OnUploading?.Invoke(uploadProgress01);

                yield return null;
            }
            yield return null; // Wait for next frame, because when While() loop condition is false, it Invoke _onUploadCompleted() right away, but we don't want that.

            UploadToServer();

            _previousCoroutine = null;
            yield break;
        }

        private void UploadToServer()
        {
            _myUserData.AddTotalTMPoints(_tmPoints);
            _myUserData.AddTodayTMPoints(_tmPoints);

            _onUploadSucceed?.Invoke();
            OnUploadSucceed?.Invoke();
        }
        #endregion



        #region --Methods-- (Override)
        public override void OnCloseButtonClick()
        {
            _tmPoints = 0;

            base.OnCloseButtonClick();
        }
        #endregion
    }
}