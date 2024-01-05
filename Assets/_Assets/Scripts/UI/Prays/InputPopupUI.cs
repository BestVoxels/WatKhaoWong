using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WatKhaoWong.Prays;

namespace WatKhaoWong.UI.Prays
{
    public class InputPopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Input Popup UI Stuffs")]
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _okButton;
        [SerializeField] private TMP_InputField _tMPointsInputField;
        #endregion



        #region --Fields-- (In Class)
        private bool _isValidated = false;
        private int _result = 0;

        private InputPopup _playerInputPopup;
        private ConfirmPopup _playerConfirmPopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerInputPopup = GameObject.FindWithTag("Player").GetComponentInChildren<InputPopup>();
            _playerConfirmPopup = GameObject.FindWithTag("Player").GetComponentInChildren<ConfirmPopup>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _okButton.onClick.AddListener(Ok);
            _tMPointsInputField.onEndEdit.AddListener(UpdateInputText);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UpdateStatusText(string newStatus) => _statusText.text = newStatus;
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerInputPopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Cancel() => _playerInputPopup.OnCancelButtonClick();

        private void Ok()
        {
            if (_isValidated)
            {
                _playerConfirmPopup.SaveToTempPlace(_result);

                _playerInputPopup.OnOkButtonClick();
            }
            else
                _playerInputPopup.OnOkButtonCantClick();
        }

        private void UpdateInputText(string TMPointsText)
        {
            if (string.IsNullOrWhiteSpace(TMPointsText))
            {
                _isValidated = false;
                UpdateStatusText(_playerInputPopup.StatusTextDefault);
                return;
            }

            if (int.TryParse(TMPointsText, out int result))
            {
                if (_playerInputPopup.Validate(result, out string validateStatus))
                {
                    _result = result;
                    _isValidated = true;
                }
                else
                    _isValidated = false;

                UpdateStatusText(validateStatus);
            }
            else
            {
                _isValidated = false;
                UpdateStatusText(_playerInputPopup.StatusTextCantParse);
            }
        }
        #endregion
    }
}