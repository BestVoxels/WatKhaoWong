using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Prays;

namespace WatKhaoWong.UI.Prays
{
    public class UndoPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Undo Popup UI Stuffs")]
        [SerializeField] private Button _undoButton;
        [SerializeField] private Slider _statusSlider;
        #endregion



        #region --Fields-- (In Class)
        private UndoPopup _playerUndoPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerUndoPopup = GameObject.FindWithTag("Player").GetComponentInChildren<UndoPopup>();

            _closeButton.onClick.AddListener(Close);

            _undoButton.onClick.AddListener(Undo);
        }

        private void OnEnable()
        {
            _playerUndoPopup.OnUploading += UpdateStatus;
        }

        private void OnDisable()
        {
            _playerUndoPopup.OnUploading -= UpdateStatus;
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerUndoPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Undo()
        {
            _playerUndoPopup.OnUndoButtonClick();
        }

        private void UpdateStatus(float progressValue)
        {
            progressValue = Mathf.Clamp01(progressValue);

            _statusSlider.value = progressValue;
        }
        #endregion
    }
}