using UnityEngine;
using System.Collections;
using WatKhaoWong.Identity;

namespace WatKhaoWong.Core
{
    /// <summary>
    /// Place on any GameObject that itself won't get disabled so that it can Show/Hide another GameObject
    /// </summary>
    public class ShowHidePagePopupUI : MonoBehaviour
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
        private static byte s_popupDepth = 0;

        private IUserData _userData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _userData = GameObject.FindWithTag("Player").GetComponentInChildren<IUserData>();
        }

        private void Start()
        {
            if (IsPageExist())
                CloseOrOpenPage();
            else
                CloseOrOpenPopup();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void CloseOrOpenPage()
        {
            if (!IsPageExist()) return;

            _page.animator.Play(_page.isShowOnStart ? _pageIdle : _pageCloseInstant, -1, 0f);
        }

        private void CloseOrOpenPopup()
        {
            if (!IsDimExist()) return;

            _dim.animator.Play(_dim.isShowOnStart ? _popupIdle : _popupCloseInstant, -1, 0f);

            foreach (AnimatorType each in _popups)
            {
                each.animator.Play(each.isShowOnStart ? _popupIdle : _popupCloseInstant, -1, 0f);
            }
        }

        private IEnumerator DelayPageAnimation(int animationIndex, float delayAmount, bool toManagePopup)
        {
            yield return new WaitForSeconds(delayAmount);

            _page.animator.Play(animationIndex, -1, 0f); // Ex Case : play "PageUI Open" animation

            // Ex Case : current animation .IsName("PageUI Open") == false | .normalizedTime == HighNumber.  [false:Still on Old Animation | HighNumber:Played Time of Old Animation]
            yield return null; // Wait for 'Page Animation' to change (to enable its Panel GameObject first). Otherwise 'Popup Animations' won't be able to interact. // Must Have 'Panel GameObject' enabled in the beginning of the New Animation, so 'Popup Animations' can interact with them.
            // Ex Case : current animation .IsName("PageUI Open") == true | .normalizedTime == 0.  [true:Updated to New Animation | 0:Just Started Playing "PageUI Open"]

            if (toManagePopup)
                CloseOrOpenPopup(); // Can't Put in Start() because once Popup UI are disabled first, then Page UI is disabled. LATER WHEN Page UI is enabled, ALL Popup UI are enabled too!

            _previousCoroutine = null;
        }

        private bool IsPageExist() => _page.animator;
        private bool IsDimExist() => _dim.animator;
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void OpenPage(float delayBeforePlay)
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(DelayPageAnimation(_pageOpen, delayBeforePlay, true));

            s_popupDepth = 0;
        }

        public void ClosePage(float delayBeforePlay)
        {
            if (_previousCoroutine != null)
                StopCoroutine(_previousCoroutine);

            _previousCoroutine = StartCoroutine(DelayPageAnimation(_pageClose, delayBeforePlay, false));
        }

        public void OpenPopup(Animator popupAnimator)
        {
            if (s_popupDepth == 0)
                _dim.animator.Play(_popupOpen, -1, 0f);

            popupAnimator.Play(_popupOpen, -1, 0f);

            s_popupDepth++;
        }

        public void ClosePopup(Animator popupAnimator)
        {
            // Guard Check only "PopupUI Open" & "PopupUI Idle" are allow to proceed
            if (!popupAnimator.GetCurrentAnimatorStateInfo(0).IsName("PopupUI Open") && !popupAnimator.GetCurrentAnimatorStateInfo(0).IsName("PopupUI Idle"))
                return;

            if (s_popupDepth <= 1)
                _dim.animator.Play(_popupClose, -1, 0f);

            popupAnimator.Play(_popupClose, -1, 0f);

            if (s_popupDepth > 0)
                s_popupDepth--;
        }

        public void OpenPopupIfGuest(Animator popupAnimator)
        {
            if (_userData.GetRole() == EUserRole.Guest)
                OpenPopup(popupAnimator);
        }

        public void OpenPopupIfNotGuest(Animator popupAnimator)
        {
            if (_userData.GetRole() != EUserRole.Guest)
                OpenPopup(popupAnimator);
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