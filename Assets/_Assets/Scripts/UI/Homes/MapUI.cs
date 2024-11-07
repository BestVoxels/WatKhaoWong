using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Homes;

namespace WatKhaoWong.UI.Homes
{
    public class MapUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;

        //[Header("Map UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private Map _playerMap;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerMap = GameObject.FindWithTag("Player").GetComponentInChildren<Map>();

            _backButton.onClick.AddListener(Back);
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerMap.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void RefreshUI()
        {

        }
        #endregion
    }
}