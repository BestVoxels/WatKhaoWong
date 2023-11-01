using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

namespace WatKhaoWong.Utils.UI
{
    /// <summary>
    /// Place on the Parent GameObject of the Button UI (on a GameObject that has 'Button' component attached).
    /// The Parent GameObject MUST have a child GameObject that CONTAINS 'Canvas Group' component, and its 'Blocks Raycasts' Boolean Field is set to 'False' or 'Untick'.
    /// </summary>
    public class EventTriggerAnimator : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        #region --Fields-- (Inspector)
        [Header("Animation Settings")]
        [Tooltip("Image to be animated (child of the button), the one that won't effect InteractableZone, its scale will be changed. Recommend to use Child Image.")]
        [SerializeField] private Transform _targetImage;
        [Space]
        [SerializeField] private AnimationCurve _pressCurve = AnimationCurve.EaseInOut(0f, 1f, 0.25f, 0.9f);
        [SerializeField] private AnimationCurve _liftCurve = AnimationCurve.EaseInOut(0f, 1f, 0.25f, 1.1f);
        [Space]
        [Tooltip("Change Size Speed, 1f = nutual according to Curve, 2f = 2 times faster, 0.5f = 2 times slower.")]
        [SerializeField] private float _pressCurveSpeed = 1f;
        [Tooltip("Change Size Speed, 1f = nutual according to Curve, 2f = 2 times faster, 0.5f = 2 times slower.")]
        [SerializeField] private float _liftCurveSpeed = 1f;
        #endregion



        #region --Fields-- (In Class)
        private Coroutine _previousCoroutine;
        private bool _permission = false;
        #endregion


#if UNITY_EDITOR
        #region --Methods-- (Built In)
        private void Awake()
        {
            bool hasError = false;

            if (!_targetImage)
            {
                Debug.LogError($"Please assign 'Target Image' field in the Inspector first before using this script. Under '{gameObject.name}' GameObject.");
                hasError = true;
            }

            var image = GetComponent<UnityEngine.UI.Image>();
            if (!image)
            {
                Debug.LogError($"No 'Image' component found in current attached GameObject. Under '{gameObject.name}' GameObject.");
                hasError = true;
            }
            if (image && image.color.a > Mathf.Epsilon)
            {
                Debug.LogError($"'Alpha' of Color field MUST be '0', because this is just a template for Pointer to trigger animation. Under '{gameObject.name}' GameObject.");
                hasError = true;
            }

            UnityEngine.UI.Image childImage = null;
            foreach (Transform child in gameObject.transform)
            {
                childImage = child.GetComponent<UnityEngine.UI.Image>();
                if (childImage) break;
            }

            if (!childImage)
            {
                Debug.LogError($"No 'Image' component found in current attached GameObject's Children. Under '{gameObject.name}' GameObject.");
                hasError = true;
            }

            CanvasGroup canvasGroup = GetComponentInChildren<CanvasGroup>();
            if (!canvasGroup)
            {
                Debug.LogError($"No 'Canvas Group' component found in current attached GameObject's Children. Under '{gameObject.name}' GameObject.");
                hasError = true;
            }

            if (canvasGroup && canvasGroup.blocksRaycasts == true)
            {
                Debug.LogError($"'Blocks Raycasts' boolean field MUST set to 'False' under 'Canvas Group' component. Under '{gameObject.name}/{canvasGroup.name}' GameObject.");
                hasError = true;
            }

            var button = GetComponent<UnityEngine.UI.Button>();
            if (!button)
            {
                Debug.LogError($"No 'Button' component found in current attached GameObject. Under '{gameObject.name}' GameObject.");
                hasError = true;
            }
            if (button && image && button.targetGraphic.transform.Equals(image.transform))
            {
                Debug.LogError($"Wrong 'Target Graphic' field on 'Button' component, MUST be child image NOT itself image. USE '{_targetImage.name}' GameObject instead. Under '{gameObject.name}' GameObject.");
                hasError = true;
            }

            if (hasError)
                UnityEditor.EditorApplication.isPlaying = false;
        }
        #endregion
#endif



        #region --Methods-- (Custom PRIVATE) ~Animating Size~
        private void DefaultSize()
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _targetImage.localScale = new Vector3(1f, 1f, 1f);
        }

        private void Expand()
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(ChangeSizeTo(_liftCurve, _liftCurveSpeed));
        }

        private void Shrink()
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(ChangeSizeTo(_pressCurve, _pressCurveSpeed));
        }

        private IEnumerator ChangeSizeTo(AnimationCurve targetCurve, float speed)
        {
            float timer = 0;
            while (timer <= 1f)
            {
                timer += Time.deltaTime * speed;
                _targetImage.localScale = new Vector3(targetCurve.Evaluate(timer), targetCurve.Evaluate(timer));

                yield return null;
            }

            yield break;
        }
        #endregion



        #region --Methods-- (Interface)
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (_permission == false)
            {
                Shrink();
                _permission = true;
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            DefaultSize();
            _permission = false;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            Shrink();
            _permission = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (_permission)
                Expand();
            else
                DefaultSize();

            _permission = false;
        }
        #endregion
    }
}