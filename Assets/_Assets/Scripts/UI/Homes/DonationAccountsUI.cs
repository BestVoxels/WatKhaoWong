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
        [SerializeField] private Button _changeLangButton;

        [Header("DonationAccounts UI Stuffs")]
        [SerializeField] private Button _copyButton1;
        [SerializeField] private Button _copyButton2;
        [SerializeField] private Button _copyButton3;
        [SerializeField] private Button _copyButton4;
        [SerializeField] private Button _copyButton5;
        [SerializeField] private Button _copyButton6;
        [SerializeField] private Button _copyButton7;
        [SerializeField] private Button _copyButton8;
        [SerializeField] private Button _copyButton9;
        [SerializeField] private Button _copyButton10;
        [SerializeField] private Button _copyButton11;
        [SerializeField] private Button _copyButton12;
        #endregion



        #region --Fields-- (In Class)
        private DonationAccounts _playerDonationAccounts;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerDonationAccounts = GameObject.FindWithTag("Player").GetComponentInChildren<DonationAccounts>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);

            _copyButton1.onClick.AddListener(Copy1);
            _copyButton2.onClick.AddListener(Copy2);
            _copyButton3.onClick.AddListener(Copy3);
            _copyButton4.onClick.AddListener(Copy4);
            _copyButton5.onClick.AddListener(Copy5);
            _copyButton6.onClick.AddListener(Copy6);
            _copyButton7.onClick.AddListener(Copy7);
            _copyButton8.onClick.AddListener(Copy8);
            _copyButton9.onClick.AddListener(Copy9);
            _copyButton10.onClick.AddListener(Copy10);
            _copyButton11.onClick.AddListener(Copy11);
            _copyButton12.onClick.AddListener(Copy12);
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _playerDonationAccounts.OnBackButtonClick();
        private void ChangeLang() => _playerDonationAccounts.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void Copy1() => _playerDonationAccounts.OnCopyButton1Click();
        private void Copy2() => _playerDonationAccounts.OnCopyButton2Click();
        private void Copy3() => _playerDonationAccounts.OnCopyButton3Click();
        private void Copy4() => _playerDonationAccounts.OnCopyButton4Click();
        private void Copy5() => _playerDonationAccounts.OnCopyButton5Click();
        private void Copy6() => _playerDonationAccounts.OnCopyButton6Click();
        private void Copy7() => _playerDonationAccounts.OnCopyButton7Click();
        private void Copy8() => _playerDonationAccounts.OnCopyButton8Click();
        private void Copy9() => _playerDonationAccounts.OnCopyButton9Click();
        private void Copy10() => _playerDonationAccounts.OnCopyButton10Click();
        private void Copy11() => _playerDonationAccounts.OnCopyButton11Click();
        private void Copy12() => _playerDonationAccounts.OnCopyButton12Click();

        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}