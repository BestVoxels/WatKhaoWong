using UnityEngine;
using WatKhaoWong.Admin;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.UI.Admin
{
    public class SearchPanelUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Settings")]
        [SerializeField] private SearchPanelInspector _ui;
        #endregion



        #region --Fields-- (In Class)
        private byte _criteriaIndex;
        private string _searchData;

        private SearchPanel _searchPanel;
        private InputFieldValidator _inputFieldValidator;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _searchPanel = player.GetComponentInChildren<SearchPanel>();

            _inputFieldValidator = FindAnyObjectByType<InputFieldValidator>();

            _ui.criteriaDropdown.onValueChanged.AddListener(CriteriaDropdownValue);
            _ui.searchButton.onClick.AddListener(SearchConfirm);
            _ui.searchDataInputField.onEndEdit.AddListener(inputText => { if (string.IsNullOrWhiteSpace(inputText)) _searchPanel.RemoveSearchFilter(); });
        }

        private void Start()
        {
            _criteriaIndex = (byte)_ui.criteriaDropdown.index;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool Validate()
        {
            bool status = true;

            if (!IsSearchDataValidated()) status = false;

            return status;
        }
        #endregion



        #region --Methods-- (Subscriber)
        private bool IsSearchDataValidated() => _inputFieldValidator.ValidateNotNull(
            _ui.searchDataInputField.text, _ui.searchDataInputFieldStatus, out _searchData,
            (_searchPanel.StatusMustBeFilled.GetLocalizedString(), _searchPanel.StatusMustBeFilledColor));

        private void CriteriaDropdownValue(int index)
        {
            _criteriaIndex = (byte)index;
        }

        private void SearchConfirm()
        {
            if (Validate())
            {
                _searchPanel.StartSearchFilter(_criteriaIndex, _searchData);
            }
            else
            {
                _searchPanel.OnValidateFailed();
            }
        }
        #endregion
    }
}