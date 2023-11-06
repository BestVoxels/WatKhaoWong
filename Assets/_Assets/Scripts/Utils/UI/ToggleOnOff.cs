using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WatKhaoWong.Utils.UI
{
    public class ToggleOnOff : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Tooltip("IMPORTANT : First Element in 'OnOffObjects' field MUST have 'Canvas Group' component, but the Rest are optional.")]
        [SerializeField] private GameObject[] _onOffObjects;
        [Space]
        [SerializeField] private float _fadeSpeed = 2f;
        [SerializeField] private AnimationCurve _onCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve _offCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        #endregion



        #region --Fields-- (In Class)
        private Coroutine _previousCoroutine;

        private Toggle _toggle;
        private CanvasGroup _canvasGroup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _toggle = GetComponentInChildren<Toggle>();

            _toggle.onValueChanged.AddListener(UpdateUI);
        }

        private void Start()
        {
            if (_toggle.isOn)
                UpdateUI(true);
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void UpdateCanvasGroup()
        {
            if (_onOffObjects[0].TryGetComponent<CanvasGroup>(out var result))
                _canvasGroup = result;
            else
                Debug.LogError("First Element in 'OnOffObjects' field MUST have 'Canvas Group' component, but the Rest are optional.");
        }

        private bool HasCanvasGroup() => _canvasGroup != null;

        private IEnumerator FadeCanvasGroupTo(AnimationCurve targetCurve)
        {
            float timer = 0;
            while (timer <= 1f)
            {
                timer += Time.deltaTime * _fadeSpeed;
                _canvasGroup.alpha = targetCurve.Evaluate(timer);

                yield return null;
            }

            _previousCoroutine = null;
            yield break;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateUI(bool status)
        {
            UpdateCanvasGroup();
            if (!HasCanvasGroup()) return;

            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            if (status)
                _previousCoroutine = StartCoroutine(FadeCanvasGroupTo(_onCurve));
            else
                _previousCoroutine = StartCoroutine(FadeCanvasGroupTo(_offCurve));
        }
        #endregion
    }
}