using TMPro;
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
        [SerializeField] private Button _changeLangButton;

        //[Space]

        //[Header("AbbotHistory UI Stuffs")]
        //[SerializeField] private TMP_Text _thyAgeText;
        //[SerializeField] private TMP_Text _thyOrdinationAgeText;
        #endregion



        #region --Fields-- (In Class)
        private AbbotHistory _playerAbbotHistory;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerAbbotHistory = GameObject.FindWithTag("Player").GetComponentInChildren<AbbotHistory>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            //UIRefresher.OnAbbotHistoryRefreshed += RefreshUI; // Can't use OnDisable()/OnEnable() because UI won't get Updated when it disabled, we want this UI to update on the background.
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerAbbotHistory.OnBackButtonClick();
        private void ChangeLang() => _playerAbbotHistory.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}