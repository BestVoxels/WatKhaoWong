using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Homes;
using WatKhaoWong.Identities;

namespace WatKhaoWong.UI.Homes
{
    public class TempleGuideUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Page Header UI Stuffs")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _changeLangButton;

        [Header("TempleGuide UI Stuffs")]
        [SerializeField] private Toggle _consentToggle;
        [SerializeField] private Button _submitInfoButton;
        [Space]
        [SerializeField] private GameObject _consentGameObject;
        [SerializeField] private GameObject _submitInfoGameObject;
        #endregion



        #region --Fields-- (In Class)
        private TempleGuide _templeGuide;
        private MyUserData _myUserData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _templeGuide = player.GetComponentInChildren<TempleGuide>();
            _myUserData = player.GetComponentInChildren<MyUserData>();

            _backButton.onClick.AddListener(Back);
            _changeLangButton.onClick.AddListener(ChangeLang);
            _consentToggle.onValueChanged.AddListener(ConsentToggle);
            _submitInfoButton.onClick.AddListener(SubmitInfo);
        }

        private void OnEnable()
        {
            RefreshUI();
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            TempleGuide.ShowConsent = false;

            RefreshUI();
        }
        #endregion



        #region --Methods-- (Subscriber) ~Page Header UI~
        private void Back()
        {
            if (TempleGuide.ShowConsent)
                _templeGuide.OnBackButtonClickWithConsent();
            else
                _templeGuide.OnBackButtonClick();
        }

        private void ChangeLang() => _templeGuide.OnChangeLangButtonClick();
        #endregion



        #region --Methods-- (Subscriber)
        private void ConsentToggle(bool toggleStatus)
        {
            // Make it so that once ticked it can't be unchecked
            _consentToggle.isOn = true;
            _consentToggle.interactable = false;

            if (_submitInfoGameObject.activeSelf) return; // Make it Trigger only once.

            _submitInfoGameObject.SetActive(true);
            _myUserData.SetTempleGuideConfirmedToTrue(); // Save to Server.
        }

        private void SubmitInfo()
        {
            _templeGuide.OnSubmitInfoButtonClick();
        }

        private void RefreshUI()
        {
            _consentGameObject.SetActive(TempleGuide.ShowConsent);
            _submitInfoGameObject.SetActive(false);
        }
        #endregion
    }
}