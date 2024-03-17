using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using WatKhaoWong.Utils.Core;

namespace WatKhaoWong.Utils.UI
{
    /// <summary>
    /// Place on the Parent GameObject of the Any UI.
    /// 
    /// The Parent GameObject MUST have a child GameObject that CONTAINS 'Canvas Group' component
    ///     - 'Blocks Raycasts' Boolean Field is set to 'False' or 'Untick'  -  so Pointer won't trigger with the UI and won't cause shaking, shink and expand infinitely.
    ///
    /// ONLY use 'Image' on the current attached GameObject as an Interactable Zone for Pointer to trigger animation, this won't cause shaking.
    /// This is why we need to avoid raycast on Child UI so that shaking won't occurs.
    /// </summary>
    public class EventTriggerAnimator : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        #region --Fields-- (Inspector)
        [Header("Animation Settings")]
        [Tooltip("GameObjects to be animated, the one that won't effect Interactable Zone, its scale will be changed. Ex incase of Button -> use Child Image (child of the button)")]
        [SerializeField] private Transform[] gameObjectsToBeAnimated;
        [Space]
        [SerializeField] private AnimationCurve _pressCurve = AnimationCurve.EaseInOut(0f, 1f, 0.25f, 0.9f);
        [SerializeField] private AnimationCurve _liftCurve = AnimationCurve.EaseInOut(0f, 1f, 0.25f, 1.1f);
        [Space]
        [Tooltip("Change Size Speed, 1f = nutual according to Curve, 2f = 2 times faster, 0.5f = 2 times slower.")]
        [SerializeField] private float _pressCurveSpeed = 1f;
        [Tooltip("Change Size Speed, 1f = nutual according to Curve, 2f = 2 times faster, 0.5f = 2 times slower.")]
        [SerializeField] private float _liftCurveSpeed = 1f;
        #endregion



        #region --Properties-- (Inspector)
        [field: Header("General Settings")]
        [field: SerializeField] public bool Interactable { get; set; } = true;
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

            // CHECK if 'gameObjectsToBeAnimated' is MISSING
            if (gameObjectsToBeAnimated.IsNullOrEmpty())   // Requires 'ArrayExtension.cs'
            {
                Debug.LogError($"Please assign 'GameObjectsToBeAnimated' field in the Inspector first before using this script. Under '{gameObject.name}' GameObject.");
                hasError = true;
            }

            // CHECK if 'current attached Image' is MISSING
            // CHECK if 'current attached Image' ALPHA color is ZERO
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

            // CHECK if 'canvas group' is MISSING
            // CHECK if 'canvas group' BLOCK RAYCASTS is TRUE
            CanvasGroup[] canvasGroups = GetComponentsInChildren<CanvasGroup>();
            if (canvasGroups.IsNullOrEmpty())   // Requires 'ArrayExtension.cs'
            {
                Debug.LogError($"No 'Canvas Group' component found in current attached GameObject's Children. Under '{gameObject.name}' GameObject.");
                hasError = true;
            }
            foreach (CanvasGroup each in canvasGroups)
            {
                if (each.blocksRaycasts == true)
                {
                    Debug.LogError($"'Blocks Raycasts' boolean field MUST set to 'False' under 'Canvas Group' component OTHERWISE it will cause Shaking! Under '{gameObject.name}/{each.name}' GameObject.");
                    hasError = true;
                }
            }

            // INCASE there is 'button' component, CHECK if 'button' TARGET GRAPHIC is ITSELF 'current attached Image', WHICH IS WRONG, MUST be CHILD IMAGE
            var button = GetComponent<UnityEngine.UI.Button>();
            if (button && image && button.targetGraphic.transform.Equals(image.transform))
            {
                Debug.LogError($"Wrong 'Target Graphic' field on 'Button' component, MUST be child image NOT itself image. USE 'GameObjectsToBeAnimated' instead. Under '{gameObject.name}' GameObject.");
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

            foreach (Transform each in gameObjectsToBeAnimated)
                each.localScale = new Vector3(1f, 1f, 1f);
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
                foreach (Transform each in gameObjectsToBeAnimated)
                    each.localScale = new Vector3(targetCurve.Evaluate(timer), targetCurve.Evaluate(timer));

                yield return null;
            }

            _previousCoroutine = null;
            yield break;
        }
        #endregion



        #region --Methods-- (Interface)
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!Interactable) return;

            if (_permission == false)
            {
                Shrink();
                _permission = true;
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (!Interactable) return;

            if (_permission)
            {
                DefaultSize();
                _permission = false;
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!Interactable) return;

            Shrink();
            _permission = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!Interactable) return;

            if (_permission)
                Expand();
            else
                DefaultSize();

            _permission = false;
        }
        #endregion
    }
}