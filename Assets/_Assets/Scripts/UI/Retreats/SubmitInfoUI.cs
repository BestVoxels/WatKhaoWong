using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Retreats;

namespace WatKhaoWong.UI.Retreats
{
    public class SubmitInfoUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        [Header("SubmitInfo UI Stuffs")]
        [SerializeField] private TMP_InputField _phoneNumberInputField;
        [SerializeField] private TMP_InputField _medicalInputField;
        [Space]
        [SerializeField] private TMP_InputField _urgentPhoneNumberInputField;
        [SerializeField] private TMP_InputField _urgentPhoneRelateInputField;
        [Space]
        [SerializeField] private TMP_InputField _lineInputField;
        [SerializeField] private TMP_InputField _fbInputField;
        [SerializeField] private TMP_InputField _igInputField;
        [SerializeField] private TMP_InputField _tiktokInputField;
        [Space]
        [SerializeField] private Button _confirmButton;
        #endregion



        #region --Fields-- (In Class)
        private SubmitInfo _submitInfo;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _submitInfo = GameObject.FindWithTag("Player").GetComponentInChildren<SubmitInfo>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);
        }

        //private void Start()
        //{
        //    RefreshUI();
        //}
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back() => _submitInfo.OnBackButtonClick();
        private void ChangeLang() => _submitInfo.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        //private void RefreshUI()
        //{

        //}
        #endregion
    }
}