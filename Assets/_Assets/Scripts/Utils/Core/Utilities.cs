using System.IO;
using System.Collections;
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
        /// <param name="tag">Animation Tag that we put under Animation Clip, can be empty tag "" as well. (Optional Parameter)</param>
        /// <returns>NormalizedTime in range of [0f, Infinity]</returns>
        public static float GetNormalizedTime(Animator animator, string tag = "")
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

        /// <summary>
        /// Caller must be under IEnumerator type method.
        /// This Method will Pause code execution until specify 'Animation Clip' is finished playing. Caller have to use (1) way.
        /// Caller can choose to do 2 ways:
        /// (1) 'yield return WaitForClipFinished();'  OR  'yield return StartCoroutine(WaitForClipFinished());'  code after this line WON'T go on.
        /// (2) 'WaitForClipFinished();'  OR  'StartCoroutine(WaitForClipFinished());'  code after this line WILL go on.
        /// Another 'Return Type' that allows Caller to choose in 2 ways is 'Coroutine'.
        /// For more details & use case examples, check 'IEnumerator  ——Unity——' section in 'C# Doc' note.
        /// ------
        /// For more details about Animation, check 'Animation Code' section in 'Unity Doc' note.
        /// </summary>
        /// <param name="animator">Animator that contains Animation Clip</param>
        /// <param name="clipName">Name of the Animation State Node</param>
        /// <returns>IEnumerator that can pause the code execution if Caller use (1) way to call.</returns>
        public static IEnumerator WaitForClipFinished(Animator animator, string clipName)
        {
            // While Animation Clip is still the OLD one, wait for it to get update
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(clipName))
            {
                yield return null;
            }

            // Wait for New Animation Clip to play until it ends
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(clipName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
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



        #region --Methods-- (Custom PUBLIC) ~Color~
        /// <summary>
        /// Convert 'Color' or 'Color32' into 'string' Hexadecimal Color Code.
        /// </summary>
        /// <param name="color">Color or Color32 is fine, no need to cast.</param>
        /// <returns>Hexadecimal Color Code</returns>
        public static string ColorToHex(Color32 color) => $"{color.r:X2}{color.g:X2}{color.b:X2}";

        /// <summary>
        /// Convert 'string' Hexadecimal Color Code into 'Color' or 'Color32'.
        /// </summary>
        /// <param name="hex">Hexadecimal Color Code</param>
        /// <param name="a">Default Alpha is fully visible, but if Hexadecimal Color Code has Alpha value this will be overrided by Hexadecimal Color Code.</param>
        /// <returns>'Color' or 'Color32'</returns>
        public static Color32 HexToColor(string hex, byte a = 255)
        {
            hex = hex.Replace("0x", ""); // in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", ""); // in case the string is formatted #FFFFFF

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            // Only use alpha if the string has enough characters
            if (hex.Length == 8)
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32(r, g, b, a);
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Image~
        /// <summary>
        /// Compress Texture in to JPG. With optimized quality and save space.
        /// </summary>
        /// <param name="quality">JPG quality 90 is more than enough, save space up to 4-10 times compared to PNG.</param>
        public static byte[] CompressToJPG(Texture2D original, int quality = 90)
        {
            return original.EncodeToJPG(quality);
        }

        /// <summary>
        /// Get the file size in MB from a file path.
        /// </summary>
        public static float GetImageSizeMB(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("File not found at path: " + filePath);
                return 0f;
            }

            long bytes = new FileInfo(filePath).Length;
            return bytes / 1_000_000f; // Thi is 'decimal MB' format. For 'binary MB' (MiB) format use (1024f * 1024f) instead of (1,000,000f)
        }

        /// <summary>
        /// Get the size in MB from a byte array (e.g. imageBytes).
        /// </summary>
        public static float GetImageSizeMB(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                Debug.LogWarning("Byte array is null or empty.");
                return 0f;
            }

            return data.Length / 1_000_000f; // Thi is 'decimal MB' format. For 'binary MB' (MiB) format use (1024f * 1024f) instead of (1,000,000f)
        }

        /// <summary>
        /// Estimate the memory usage in MB of a loaded Texture2D.
        ///
        /// Why Estimate?
        /// Unlike the file size on disk (FileInfo.Length) or the byte array length (byte[].Length),
        /// the memory usage of a Texture2D in Unity is not directly exposed and varies based on...
        /// 
        /// - Texture Compression : DXT1, ETC2, ASTC, etc.
        /// - Mipmaps : If mipmaps are enabled, Unity stores smaller versions of the texture too
        /// - Runtime Conversion : Unity might convert the format internally when uploading to the GPU
        /// - Graphics API Differences : OpenGL, DirectX, Vulkan, and Metal all handle textures a bit differently
        /// </summary>
        public static float GetImageSizeMB(Texture2D texture)
        {
            if (texture == null)
            {
                Debug.LogWarning("Texture is null.");
                return 0f;
            }

            int bitsPerPixel = GetBitsPerPixel(texture.format);
            int mipCount = texture.mipmapCount > 1 ? 2 : 1; // Very rough multiplier for mipmaps
            float totalBits = texture.width * texture.height * bitsPerPixel * mipCount;
            float totalBytes = totalBits / 8f;
            return totalBytes / 1_000_000f; // Thi is 'decimal MB' format. For 'binary MB' (MiB) format use (1024f * 1024f) instead of (1,000,000f)
        }

        /// <summary>
        /// Get bits per pixel based on texture format.
        /// </summary>
        public static int GetBitsPerPixel(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Alpha8: return 8;
                case TextureFormat.ARGB4444: return 16;
                case TextureFormat.RGB24: return 24;
                case TextureFormat.RGBA32: return 32;
                case TextureFormat.ARGB32: return 32;
                case TextureFormat.RGB565: return 16;
                case TextureFormat.DXT1: return 4;
                case TextureFormat.DXT5: return 8;
                case TextureFormat.RGBA4444: return 16;
                case TextureFormat.RHalf: return 16;
                case TextureFormat.RGHalf: return 32;
                case TextureFormat.RGBAHalf: return 64;
                case TextureFormat.RFloat: return 32;
                case TextureFormat.RGFloat: return 64;
                case TextureFormat.RGBAFloat: return 128;
                default:
                    Debug.LogWarning($"Unknown bits-per-pixel for format: {format}, assuming 32.");
                    return 32;
            }
        }

        /// <summary>
        /// NOT IDEAL: Image looks blurry even set to same resolution as Original Image.
        /// </summary>
        public static Texture2D ResizePreserveRatio(Texture2D source, int maxSize)
        {
            int width = source.width;
            int height = source.height;

            float ratio = (float)width / height;

            int newWidth, newHeight;
            if (width > height)
            {
                newWidth = maxSize;
                newHeight = Mathf.RoundToInt(maxSize / ratio);
            }
            else
            {
                newHeight = maxSize;
                newWidth = Mathf.RoundToInt(maxSize * ratio);
            }

            // Create a RenderTexture with new dimensions
            RenderTexture rt = new RenderTexture(newWidth, newHeight, 24);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = rt;

            // Copy and scale the texture into the render texture
            source.filterMode = FilterMode.Point;

            Graphics.Blit(source, rt);

            // Create a new Texture2D with the target size
            Texture2D resized = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
            resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            resized.Apply();

            // Clean up
            RenderTexture.active = currentRT;
            rt.Release();

            return resized;
        }

        /// <summary>
        /// NOT IDEAL: Image looks Pixelate when lower size down.
        /// </summary>
        public static Texture2D ResizePreserveRatioUsingCPU(Texture2D source, int maxSize)
        {
            int width = source.width;
            int height = source.height;

            float ratio = (float)width / height;

            int newWidth, newHeight;
            if (width > height)
            {
                newWidth = maxSize;
                newHeight = Mathf.RoundToInt(maxSize / ratio);
            }
            else
            {
                newHeight = maxSize;
                newWidth = Mathf.RoundToInt(maxSize * ratio);
            }

            Texture2D result = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    float u = (float)x / (newWidth - 1);
                    float v = (float)y / (newHeight - 1);
                    result.SetPixel(x, y, source.GetPixelBilinear(u, v));
                }
            }

            result.Apply();
            return result;
        }
        #endregion
    }
}