using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WatKhaoWong.Utils.UI
{
    public class InputFieldShowHidePassword : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("General Stuffs")]
        [SerializeField] private bool _showOnStart = false;
        [Space]
        [Header("Settings Stuffs")]
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Sprite _spriteToShow;
        [SerializeField] private Sprite _spriteToHide;
        [Space]
        [SerializeField] private EventTrigger _iconImageEventTrigger;
        #endregion



        #region --Fields-- (In Class)
        private bool _isShowing;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData data) => ShowHidePassword());
            _iconImageEventTrigger.triggers.Add(entry);
        }

        private void Start()
        {
            ShowHidePassword(_showOnStart);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ShowHidePassword()
        {
            _isShowing = !_isShowing;

            _iconImage.sprite = _isShowing ? _spriteToShow : _spriteToHide;

            _inputField.contentType = _isShowing ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            _inputField.ForceLabelUpdate(); // To make the InputField refresh when we change its ContentType
        }

        private void ShowHidePassword(bool toShow)
        {
            _isShowing = toShow;

            _iconImage.sprite = toShow ? _spriteToShow : _spriteToHide;

            _inputField.contentType = toShow ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            _inputField.ForceLabelUpdate(); // To make the InputField refresh when we change its ContentType
        }
        #endregion
    }
}