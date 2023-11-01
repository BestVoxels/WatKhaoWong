using UnityEngine;

namespace WatKhaoWong.Utils.UI
{
    /// <summary>
    /// Place on any GameObject that itself won't get disabled so that it can Show/Hide another GameObject
    /// </summary>
    public class ShowHideUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private KeyCode _toggleKey = KeyCode.Escape;
        [SerializeField] private GameObject _uiContainer = null;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _uiContainer.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                ShowHideUIContainer();
            }
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void ShowHideUIContainer()
        {
            _uiContainer.SetActive(!_uiContainer.activeSelf);
        }
        #endregion
    }
}