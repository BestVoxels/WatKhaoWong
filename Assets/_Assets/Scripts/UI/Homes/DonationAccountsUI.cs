using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Homes;

namespace WatKhaoWong.UI.Homes
{
    public class DonationAccountsUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;

        //[Header("DonationAccounts UI Stuffs")]
        #endregion



        #region --Fields-- (In Class)
        private DonationAccounts _playerDonationAccounts;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerDonationAccounts = GameObject.FindWithTag("Player").GetComponentInChildren<DonationAccounts>();

            _backButton.onClick.AddListener(Back);
        }

        private void Start()
        {
            RefreshUI();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerDonationAccounts.OnBackButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void RefreshUI()
        {

        }
        #endregion
    }
}