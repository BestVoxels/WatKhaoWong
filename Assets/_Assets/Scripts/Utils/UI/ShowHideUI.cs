using UnityEngine;
using System.Collections;

namespace WatKhaoWong.Utils.UI
{
    /// <summary>
    /// Place on any GameObject that itself won't get disabled so that it can Show/Hide another GameObject
    /// </summary>
    public class ShowHideUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Animation Stuffs")]
        [SerializeField] private AnimatorType _page;

        [Header("Popup Animation Stuffs")]
        [SerializeField] private AnimatorType _dim;
        [SerializeField] private AnimatorType[] _popups;
        #endregion



        #region --Fields-- (In Class)
        private readonly int _pageIdle = Animator.StringToHash("PageUI Idle");
        private readonly int _pageOpen = Animator.StringToHash("PageUI Open");
        private readonly int _pageClose = Animator.StringToHash("PageUI Close");
        private readonly int _pageCloseInstant = Animator.StringToHash("PageUI Close Instant");

        private readonly int _popupIdle = Animator.StringToHash("PopupUI Idle");
        private readonly int _popupOpen = Animator.StringToHash("PopupUI Open");
        private readonly int _popupClose = Animator.StringToHash("PopupUI Close");
        private readonly int _popupCloseInstant = Animator.StringToHash("PopupUI Close Instant");

        private Coroutine _previousCoroutine;
        private byte _popupDepth = 0;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            CloseOrOpenPage();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void CloseOrOpenPage()
        {
            _page.animator.Play(_page.isShowOnStart ? _pageIdle : _pageCloseInstant, -1, 0f);
        }

        private void CloseOrOpenPopup()
        {
            if (!_dim.animator) return;

            _dim.animator.Play(_dim.isShowOnStart ? _popupIdle : _popupCloseInstant, -1, 0f);

            foreach (AnimatorType each in _popups)
            {
                each.animator.Play(each.isShowOnStart ? _popupIdle : _popupCloseInstant, -1, 0f);
            }
        }

        private IEnumerator DelayPageAnimation(int animationIndex, float delayAmount)
        {
            yield return new WaitForSeconds(delayAmount);

            _page.animator.Play(animationIndex, -1, 0f);

            _previousCoroutine = null;
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void OpenPage(float delayBeforePlay)
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(DelayPageAnimation(_pageOpen, delayBeforePlay));

            CloseOrOpenPopup();
            _popupDepth = 0;
        }

        public void ClosePage(float delayBeforePlay)
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(DelayPageAnimation(_pageClose, delayBeforePlay));
        }

        public void OpenPopup(Animator popupAnimator)
        {
            if (_popupDepth == 0)
                _dim.animator.Play(_popupOpen, -1, 0f);

            popupAnimator.Play(_popupOpen, -1, 0f);

            _popupDepth++;
        }

        public void ClosePopup(Animator popupAnimator)
        {
            if (!popupAnimator.GetCurrentAnimatorStateInfo(0).IsName("PopupUI Open")) return;

            if (_popupDepth == 1)
                _dim.animator.Play(_popupClose, -1, 0f);

            popupAnimator.Play(_popupClose, -1, 0f);

            _popupDepth--;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class AnimatorType
        {
            public Animator animator;

            public bool isShowOnStart;
        }
        #endregion
    }
}