using Michsky.MUIP;
using TMPro;
using UnityEngine.UI;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Admin
{
    [System.Serializable]
    public class SearchPanelInspector
    {
        public ESearchPanelLocation location;
        public CustomDropdown criteriaDropdown;
        public TMP_InputField searchDataInputField;
        public InputFieldStatus searchDataInputFieldStatus;
        public Button searchButton;
    }
}