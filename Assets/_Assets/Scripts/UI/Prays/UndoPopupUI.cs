using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Prays;
using WatKhaoWong.Utils.UI;

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
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private TMP_Text _infoText;
        #endregion



        #region --Fields-- (In Class)
        private UndoPopup _playerUndoPopup;
        private EventTriggerAnimator _undoEventTrigger;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerUndoPopup = GameObject.FindWithTag("Player").GetComponentInChildren<UndoPopup>();
            _undoEventTrigger = _undoButton.GetComponent<EventTriggerAnimator>();

            _closeButton.onClick.AddListener(Close);

            _undoButton.onClick.AddListener(Undo);
        }

        private void OnEnable()
        {
            RefreshUIDefault();

            _playerUndoPopup.OnUploading += UpdateStatus;
            _playerUndoPopup.OnUploadSucceeded += RefreshUIDone;
        }

        private void OnDisable()
        {
            _playerUndoPopup.OnUploading -= UpdateStatus;
            _playerUndoPopup.OnUploadSucceeded -= RefreshUIDone;
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

        private void RefreshUIDefault()
        {
            _undoButton.interactable = true;
            _undoEventTrigger.Interactable = true;

            _statusSlider.gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(false);

            _headerText.text = _playerUndoPopup.StatusHeaderTextDefault;
            _infoText.text = _playerUndoPopup.StatusInfoTextDefault;
        }

        private void RefreshUIDone()
        {
            _undoButton.interactable = false;
            _undoEventTrigger.Interactable = false;

            _statusSlider.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(true);

            _headerText.text = _playerUndoPopup.StatusHeaderTextDone;
            _infoText.text = _playerUndoPopup.StatusInfoTextDone;
        }
        #endregion
    }
}