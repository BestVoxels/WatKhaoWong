using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Prays;
using WatKhaoWong.UI.Toggles;

namespace WatKhaoWong.UI.Prays
{
    public class ChallengePopupUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Popup Header UI Stuffs")]
        [SerializeField] private Button _closeButton;

        [Header("Challenge Popup UI Stuffs")]
        [SerializeField] private ToggleGroup _lengthTG;
        [SerializeField] private ToggleGroup _nowOrLaterTG;
        [SerializeField] private ToggleGroup _delayDurationTG;
        [Space]
        [SerializeField] private Toggle _nowToggle;
        [Space]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;
        [Space]
        [SerializeField] private GameObject[] _delayUIGameObjects;
        #endregion



        #region --Fields-- (In Class)
        private bool _isNowToggleTicked;
        private int _lengthTimeValue;
        private ENowOrLater _nowOrLaterValue;
        private int _delayTimeValue;

        private ChallengePopup _playerChallengePopup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerChallengePopup = GameObject.FindWithTag("Player").GetComponentInChildren<ChallengePopup>();

            _closeButton.onClick.AddListener(Close);

            _cancelButton.onClick.AddListener(Cancel);
            _confirmButton.onClick.AddListener(Confirm);

            _nowToggle.onValueChanged.AddListener(RefreshDelayUI);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool ValidateChallengePopup()
        {
            if (IsToggleGroupsEmpty()) return false;

            foreach (Toggle each in _lengthTG.ActiveToggles())
            {
                TimeLengthToggle toggle = each.GetComponentInChildren<TimeLengthToggle>();
                if (!toggle) return false;

                _lengthTimeValue = toggle.GetTimeValue();
            }

            foreach (Toggle each in _nowOrLaterTG.ActiveToggles())
            {
                NowOrLaterToggle toggle = each.GetComponentInChildren<NowOrLaterToggle>();
                if (!toggle) return false;

                _nowOrLaterValue = toggle.NowOrLater;
            }

            if (!_isNowToggleTicked)
            {
                foreach (Toggle each in _delayDurationTG.ActiveToggles())
                {
                    TimeLengthToggle toggle = each.GetComponentInChildren<TimeLengthToggle>();
                    if (!toggle) return false;

                    _delayTimeValue = toggle.GetTimeValue();
                }
            }

            return true;
        }

        private bool IsToggleGroupsEmpty()
        {
            bool isEmpty = false;

            if (!_lengthTG.AnyTogglesOn())
            {
                // TODO UPDATE Status Text by calling SingleTon METHOD Maybe?
                Debug.LogWarning(_playerChallengePopup.StatusMissingLengthTG);

                isEmpty = true;
            }
            if (!_nowOrLaterTG.AnyTogglesOn())
            {
                // TODO UPDATE Status Text by calling SingleTon METHOD Maybe?
                Debug.LogWarning(_playerChallengePopup.StatusMissingNowOrLaterTG);

                isEmpty = true;
            }
            if (!_delayDurationTG.AnyTogglesOn() && !_isNowToggleTicked)
            {
                // TODO UPDATE Status Text by calling SingleTon METHOD Maybe?
                Debug.LogWarning(_playerChallengePopup.StatusMissingDelayDurationTG);

                isEmpty = true;
            }

            return isEmpty;
        }

        private void UploadToServer()
        {
            bool isUploaded = false;
            switch (_nowOrLaterValue)
            {
                case ENowOrLater.Now:
                    // TODO UPLOAD to server NOW using '_lengthTimeValue'  |  ASSIGN 'isUploaded' to correct value
                    Debug.LogWarning($"START Challenge NOW, talk to server using {_lengthTimeValue}");
                    isUploaded = true;
                    break;

                case ENowOrLater.Later:
                    // TODO UPLOAD to server LATER using '_lengthTimeValue' & '_delayTimeValue'  |  ASSIGN 'isUploaded' to correct value
                    Debug.LogWarning($"START Challenge LATER, talk to server using {_lengthTimeValue} & {_delayTimeValue}");
                    isUploaded = true;
                    break;

                default:
                    Debug.LogError("Something Wrong! Active Toggle ISN'T either 'Now' or 'Later'!");
                    break;
            }

            if (isUploaded)
            {
                // TODO UPDATE Status Text by calling SingleTon METHOD Maybe?
                Debug.LogWarning(_playerChallengePopup.StatusUploadSucceed);
            }
            else
            {
                // TODO UPDATE Status Text by calling SingleTon METHOD Maybe?
                Debug.LogWarning(_playerChallengePopup.StatusUploadFail);
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~Popup Header UI~
        private void Close() => _playerChallengePopup.OnCloseButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Cancel()
        {
            _playerChallengePopup.OnCancelButtonClick();
        }

        private void Confirm()
        {
            if (ValidateChallengePopup())
            {
                UploadToServer();

                _playerChallengePopup.OnConfirmButtonClick();
            }
            else
                _playerChallengePopup.OnConfirmButtonCantClick();
        }

        private void RefreshDelayUI(bool tickedStatus)
        {
            _isNowToggleTicked = tickedStatus;

            foreach (GameObject each in _delayUIGameObjects)
                each.SetActive(!tickedStatus);
        }
        #endregion
    }
}