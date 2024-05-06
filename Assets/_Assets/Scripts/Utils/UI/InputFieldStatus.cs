using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WatKhaoWong.Utils.UI
{
    public class InputFieldStatus : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("General Stuffs")]
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Image _normalImage;
        [SerializeField] private RectTransform _placeHolderRT;
        [SerializeField] private RectTransform _textAreaRT;
        [Space]
        [Header("Error Status Stuffs")]
        [Tooltip("This is the sprite that will shows all the time, even though User is NOT select the input field.")]
        [SerializeField] private Sprite _errorNormalSprite;
        [Tooltip("This is the sprite that will shows only when User SELECT the input field.")]
        [SerializeField] private Sprite _errorSelectedSprite;
        [SerializeField] private GameObject _errorIcon;
        [SerializeField] private float _errorPlaceHolderRectRight = 100f;
        [SerializeField] private float _errorTextAreaRectRight = 100f;
        #endregion



        #region --Fields-- (In Class)
        private Sprite _defaultNormalSprite;
        private Sprite _defaultSelectedSprite;

        private Vector2 _defaultPlaceHolderOffsetMin;
        private Vector2 _defaultPlaceHolderOffsetMax;
        private Vector2 _defaultTextAreaOffsetMin;
        private Vector2 _defaultTextAreaOffsetMax;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _defaultNormalSprite = _normalImage.sprite;
            _defaultSelectedSprite = _inputField.spriteState.selectedSprite;

            _defaultPlaceHolderOffsetMin = _placeHolderRT.offsetMin;
            _defaultPlaceHolderOffsetMax = -_placeHolderRT.offsetMax;

            _defaultTextAreaOffsetMin = _textAreaRT.offsetMin;
            _defaultTextAreaOffsetMax = -_textAreaRT.offsetMax;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void SetNormal()
        {
            ChangeNormalSprite(_defaultNormalSprite);

            ChangeInputFieldSelectedSprite(_defaultSelectedSprite);

            _errorIcon.SetActive(false);

            ChangePlaceHolderRT(_defaultPlaceHolderOffsetMin.x, _defaultPlaceHolderOffsetMin.y, _defaultPlaceHolderOffsetMax.x, _defaultPlaceHolderOffsetMax.y);
            ChangeTextAreaRT(_defaultTextAreaOffsetMin.x, _defaultTextAreaOffsetMin.y, _defaultTextAreaOffsetMax.x, _defaultTextAreaOffsetMax.y);
        }

        public void SetError()
        {
            ChangeNormalSprite(_errorNormalSprite);

            ChangeInputFieldSelectedSprite(_errorSelectedSprite);

            _errorIcon.SetActive(true);

            ChangePlaceHolderRT(_defaultPlaceHolderOffsetMin.x, _defaultPlaceHolderOffsetMin.y, _errorPlaceHolderRectRight, _defaultPlaceHolderOffsetMax.y);
            ChangeTextAreaRT(_defaultTextAreaOffsetMin.x, _defaultTextAreaOffsetMin.y, _errorTextAreaRectRight, _defaultTextAreaOffsetMax.y);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ChangeNormalSprite(Sprite input) => _normalImage.sprite = input;

        private void ChangeInputFieldSelectedSprite(Sprite input)
        {
            SpriteState spriteState = _inputField.spriteState;
            spriteState.selectedSprite = input;

            _inputField.spriteState = spriteState;
        }

        private void ChangePlaceHolderRT(float left, float bottom, float right, float top)
        {
            _placeHolderRT.offsetMin = new Vector2(left, bottom);
            _placeHolderRT.offsetMax = new Vector2(-right, -top);
        }

        private void ChangeTextAreaRT(float left, float bottom, float right, float top)
        {
            _textAreaRT.offsetMin = new Vector2(left, bottom);
            _textAreaRT.offsetMax = new Vector2(-right, -top);
        }
        #endregion
    }
}