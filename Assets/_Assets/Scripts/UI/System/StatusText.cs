using UnityEngine;

namespace WatKhaoWong.UI.System
{
    public class StatusText : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Status Text Stuffs")]
        [SerializeField] private StatusTextUI _statusTextPrefab;
        [SerializeField] private Transform _spawnPlace;

        [Header("Status Text Settings")]
        [SerializeField] private Color32 _defaultColor;
        [Min(0f)]
        [SerializeField] private float _spawnGap = 150f;
        #endregion



        #region --Fields-- (In Class)
        private static int _counter = 0;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Show(string text, Color32 color = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            
            if (color.r == 0 && color.g == 0 && color.b == 0 && color.a == 0)
                color = _defaultColor;

            StatusTextUI statusText = Instantiate<StatusTextUI>(_statusTextPrefab, _spawnPlace);
            statusText.text = $"{text}";

            statusText.color = color;

            statusText.rectTransfrom.anchoredPosition = new Vector2(0, CalculateGap());

            _counter++;
        }

        public void ReduceCounter()
        {
            if (_counter <= 0)
            {
                Debug.LogError($"Status Text Counter can't be reduced by 1, otherwise it will be negative");
                return;
            }

            _counter--;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private float CalculateGap() => -_spawnGap * _counter;
        #endregion
    }
}