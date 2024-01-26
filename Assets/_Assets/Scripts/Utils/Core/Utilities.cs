using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WatKhaoWong.Utils.Core
{
    public static class Utilities
    {
        #region --Methods-- (Custom PUBLIC) ~For EventSystem Touching~
        public static bool IsPointerOverUIObject()
        {
            if (EventSystem.current == null) return false;

            // the ray cast appears to require only eventData.position
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Rotating~
        /// <summary>
        /// Snap Rotate Method, use by calling it Once, no need in multiple times like Smooth Rotate.
        /// </summary>
        /// <param name="transform">GameObject that want to be rotated</param>
        /// <param name="target">Target to rotate to</param>
        public static void SnapRotateTo(Transform transform, Transform target)
        {
            Vector3 direction = target.position - transform.position;
            transform.rotation = LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
        }

        public static void SnapRotateTo(Transform transform, Vector3 targetPosition)
        {
            transform.rotation = LookRotation(new Vector3(targetPosition.x, 0f, targetPosition.z), Vector3.up);
        }

        /// <summary>
        /// Smooth Rotate Method, use by calling multiple times so it can gradually rotate each time it get called.
        /// There is also the one that return bool indicate whether it is done rotating or not.
        /// For more details, check 'Rotate Code' section in 'Unity Doc' note.
        /// </summary>
        /// <param name="transform">GameObject that want to be rotated</param>
        /// <param name="target">Target to rotate to</param>
        /// <param name="rotateSpeed">DO NOT pass in Time.deltaTime Since this method already handle that</param>
        public static void SmoothRotateTo(Transform transform, Transform target, float rotateSpeed)
        {
            Vector3 direction = target.position - transform.position; // Getting Direction from vector3 by using formula 'targetPos - ourPos'
            Quaternion lookRotation = LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up); // Get Rotation that we want to rotate to
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime); // Gradually Rotate
        }

        public static void SmoothRotateTo(Transform transform, Vector3 targetPosition, float rotateSpeed)
        {
            Quaternion lookRotation = LookRotation(new Vector3(targetPosition.x, 0f, targetPosition.z), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }

        public static void SmoothRotateTo(Transform transform, Quaternion targetRotation, float rotateSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Smooth Rotate Method, use by calling multiple times so it can gradually rotate each time it get called.
        /// There is also the one that return bool indicate whether it is done rotating or not.
        /// For more details, check 'Rotate Code' section in 'Unity Doc' note.
        /// </summary>
        /// <param name="transform">GameObject that want to be rotated</param>
        /// <param name="target">Target to rotate to</param>
        /// <param name="rotateSpeed">DO NOT pass in Time.deltaTime Since this method already handle that</param>
        /// <param name="precisionRate">0f mean exact match / 1f mean far off. Typically use 0.01f or 0.001f or pretty close use 0.0000001f</param>
        /// <returns>Return False if it's close enough otherwise True.</returns>
        public static bool SmoothRotateTo(Transform transform, Vector3 targetPosition, float rotateSpeed, float precisionRate)
        {
            Vector3 direction = targetPosition - transform.position;
            Quaternion lookRotation = LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

            return !IsApproximate(transform.rotation, lookRotation, precisionRate);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Checking Approximation~
        /// <summary>
        /// Checking if Two Quaternions are close or not, using precision paramter to define how close.
        /// For more details, check 'Rotate Code' section in 'Unity Doc' note.
        /// </summary>
        /// <param name="q1">Quaternion 1st to compare</param>
        /// <param name="q2">Quaternion 2nd to compare</param>
        /// <param name="precision">0f mean exact match / 1f mean far off. Typically use 0.01f or 0.001f or pretty close use 0.0000001f</param>
        /// <returns>Return True if it's close enough otherwise False.</returns>
        public static bool IsApproximate(Quaternion q1, Quaternion q2, float precision)
        {
            return Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - precision;
        }

        /// <summary>
        /// Checking if Two Vector3 are close enough or not, using howCloseDistance paramter to define how close.
        /// </summary>
        /// <param name="v1">Vector3 1st to compare</param>
        /// <param name="v2">Vector3 2nd to compare</param>
        /// <param name="howCloseDistance">0f mean exact match (stay at same position) / [>0f - up till Infinity] mean how far off in distance.</param>
        /// <returns>Return True if it's close enough otherwise False.</returns>
        public static bool IsApproximate(Vector3 v1, Vector3 v2, float howCloseDistance)
        {
            return SqrDistance(v1, v2) <= 0f + (howCloseDistance * howCloseDistance);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Improvement~
        /// <summary>
        /// Fix 'Look rotation viewing vector is zero' message in console.
        /// Problem Come from Quaternion.LookRotation() bacause it is impossible to find any angle between two points when those points are the same as being zero.
        /// Use this method instead of calling Quaternion.LookRotation().
        /// For more details, check 'Rotate Code' section in 'Unity Doc' note.
        /// </summary>
        public static Quaternion LookRotation(Vector3 forward, Vector3 upwards)
        {
            return forward == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(forward, upwards);
        }

        /// <summary>
        /// Both Arguments 'position1' and 'position2' ARE swappable, this has no effect in distance result.
        /// This is faster than using .Distance() or .magnitude
        /// This return Square Distance, to compare with Actual Distance simply do (Actual Distance * Actual Distance) to make it comparable. This gives a better performance.
        /// For more details, check 'Vector3 Code' section in 'Unity Doc' note.
        /// </summary>
        public static float SqrDistance(Vector3 position1, Vector3 position2)
        {
            Vector3 offset = position1 - position2;
            return offset.sqrMagnitude;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Animation~
        /// <summary>
        /// To get Played Time Amount of an Animation Clip in Normalized Time. (When Completed the length is 1f)
        /// We can put the Animation Tag under each Animation Clip to group or differentiate between them.
        /// For more details, check 'Animation Code' section in 'Unity Doc' note.
        /// </summary>
        /// <param name="animator">Animator that we want to check</param>
        /// <param name="tag">Animation Tag that we put under Animation Clip, can be empty tag "" as well</param>
        /// <returns>NormalizedTime in range of [0f, Infinity]</returns>
        public static float GetNormalizedTime(Animator animator, string tag)
        {
            // We can't simply return currentState.normalizedTime while There is Transitioning between States
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);

            // While Transitioning we have to get normalizedTime from nextState
            if (animator.IsInTransition(0) && nextState.IsTag(tag))
            {
                return nextState.normalizedTime;
            }
            // While Transitioning is STOP we have to get normalizedTime from currentState
            else if (!animator.IsInTransition(0) && currentState.IsTag(tag))
            {
                return currentState.normalizedTime;
            }
            // Incase something is goes wrong
            else
            {
                return 0f;
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~For Detecting~
        /// <summary>
        /// ***IMPORTANT Make Sure 'Scene Window' is CLOSED, because the screen there is also count not just in 'Game Window'***
        /// To check if a specify target is on the Player Screen Display or not.
        /// For more details, check 'Camera Methods Tips' section in 'Unity Doc' note.
        /// </summary>
        public static bool IsOnScreen(GameObject target)
        {
            MeshRenderer targetMesh = target.GetComponentInChildren<MeshRenderer>();
            if (targetMesh == null)
            {
                Debug.LogError("No MeshRenderer Found on the 'target'.");
                return false;
            }

            return targetMesh.isVisible;

            //// There is Bug with this old code, when target is exactly behind the camera, it will return true.
            //Vector2 viewPosition = mainCamera.WorldToViewportPoint(target);
            //return viewPosition.x >= 0f && viewPosition.x <= 1f && viewPosition.y >= 0f && viewPosition.y <= 1f;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Vector3~
        /// <summary>
        /// Both Arguments 'toGoTo' and 'origin' ARE NOT swappable, since the direction will be the opposite side when swap.
        /// </summary>
        /// <param name="toGoTo">is the direction that we will be heading toward</param>
        /// <returns>Only Direction. As Vector3 NOT more than 1</returns>
        public static Vector3 Direction(Vector3 toGoTo, Vector3 origin)
        {
            return DirectionWithHowFarAway(toGoTo, origin).normalized;
        }

        /// <summary>
        /// Both Arguments 'toGoTo' and 'origin' ARE NOT swappable, since the direction will be the opposite side when swap.
        /// </summary>
        /// <param name="toGoTo">is the direction that we will be heading toward</param>
        /// <returns>Direction & How Far Away. As Vector3 more than 1</returns>
        public static Vector3 DirectionWithHowFarAway(Vector3 toGoTo, Vector3 origin)
        {
            return toGoTo - origin;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Numerical~
        /// <summary>
        /// Percentage CAN BE Over 100 or Negative Value.
        /// inputValue 500, percentage as 200, returns will be 1000.
        /// inputValue 500, percentage as -200, returns will be -1000.
        /// </summary>
        /// <param name="inputValue">Any Value</param>
        /// <param name="percentage">Any Percentage that we want to get from the InputValue</param>
        /// <returns>Any Value. Ex-inputValue=500, percentage=50, returns will be 250.</returns>
        public static float GetValueByPercentage(float inputValue, float percentage)
        {
            return (inputValue * percentage) / 100f;
        }

        /// <summary>
        /// Getting 0-1 values from the Range of Min & Max.
        /// (0, 2, 1) returns 0.5.
        /// (0, 2, 3) returns 1.
        /// (0, 2, -1) returns 0.
        /// For more details, check 'Mathf Code' section in 'Unity Doc' note.
        /// </summary>
        /// <param name="minValue">Minimum Range Value</param>
        /// <param name="maxValue">Maximum Range value</param>
        /// <param name="currentValue">Any Value between ‘min’ and ’max’. No Error if Below Min or Above Max.</param>
        /// <returns>0-1</returns>
        public static float Get01ValueFrom(float minValue, float maxValue, float currentValue)
        {
            return Mathf.InverseLerp(minValue, maxValue, currentValue);
        }
        #endregion
    }
}