using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Homes;

namespace WatKhaoWong.UI.Homes
{
    public class AbbotHistoryUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;

        //[Header("AbbotHistory UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private AbbotHistory _playerAbbotHistory;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerAbbotHistory = GameObject.FindWithTag("Player").GetComponentInChildren<AbbotHistory>();

            _backButton.onClick.AddListener(Back);
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerAbbotHistory.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void RefreshUI()
        {

        }
        #endregion
    }
}