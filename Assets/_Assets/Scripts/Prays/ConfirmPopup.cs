using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace WatKhaoWong.Prays
{
    public class ConfirmPopup : Popup
    {
        #region --Fields-- (Inspector)
        [Header("Undo Popup Settings")]
        [Tooltip("This Numerical Value is represented as 'second' unit.")]
        [Range(0f, 10f)]
        [SerializeField] private float _uploadDelay = 3f;
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Confirm Popup UI Event")]
        [SerializeField] private UnityEvent _onCancelButtonClick;
        [SerializeField] private UnityEvent _onConfirmButtonClick;
        [Space]
        [Header("Undo Popup UI Event")]
        [SerializeField] private UnityEvent _onUndoButtonClick;
        [Space]
        [Header("Other Event")]
        [SerializeField] private UnityEvent _onUploadDelayCompleted;
        #endregion



        #region --Events-- (Delegate as Action)
        public event Action OnUploadSucceed;
        #endregion



        #region --Fields-- (In Class)
        private Coroutine _previousCoroutine;
        #endregion



        #region --Properties-- (Auto)
        [field: SerializeField] public int TMPoints { get; private set; }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SaveToTempPlace(int result)
        {
            if (result <= 0) return;

            Debug.LogWarning($"Save Points ({result}) to TMPoints property under ConfirmPopup.cs");

            TMPoints = result;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Popup UI Buttons~
        public void OnCancelButtonClick()
        {
            Debug.LogWarning("Click \"Cancel\" Button! on Popup");

            TMPoints = 0;

            _onCancelButtonClick?.Invoke();
        }

        public void OnConfirmButtonClick()
        {
            Debug.LogWarning("Click \"Confirm\" Button! on Popup");

            _previousCoroutine = StartCoroutine(UploadToServerDelay());

            _onConfirmButtonClick?.Invoke();
        }

        public void OnUndoButtonClick()
        {
            Debug.LogWarning("Click \"Undo\" Button! on Popup");

            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            TMPoints = 0;

            _onUndoButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator UploadToServerDelay()
        {
            float timer = 0f;
            while (timer < _uploadDelay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            UploadToServer();
            _onUploadDelayCompleted?.Invoke();

            _previousCoroutine = null;
            yield break;
        }

        private void UploadToServer()
        {
            // TODO Upload to Server HERE!!! Pass in 'TMPoints' to it!!!
            Debug.LogWarning($"Upload {TMPoints} Point to Server!!!");

            // TODO When Upload SUCCESSFUL, call OnUploadSucceed Event
            OnUploadSucceed?.Invoke();
        }
        #endregion



        #region --Methods-- (Override)
        public override void OnCloseButtonClick()
        {
            TMPoints = 0;

            base.OnCloseButtonClick();
        }
        #endregion
    }
}