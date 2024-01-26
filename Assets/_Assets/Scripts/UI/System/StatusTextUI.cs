using UnityEngine;
using TMPro;

namespace WatKhaoWong.UI.System
{
    public class StatusTextUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _text;
        #endregion



        #region --Properties-- (With Backing Fields)
        public string text { get => _text.text;  set => _text.text = value; }
        public Color32 color { get => _text.color; set => _text.color = value; }
        public RectTransform rectTransfrom { get => _text.rectTransform; }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void ReduceCounterOnStatusText() => FindAnyObjectByType<StatusText>().ReduceCounter();
        #endregion
    }
}