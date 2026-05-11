using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatKhaoWong.Utils.UI;

namespace WatKhaoWong.Retreats
{
    [System.Serializable]
    public class StayEntryInspector
    {
        public CustomDropdown activityDropdown;
        public TMP_Text activityResultText;
        [Space]
        public Button setTimeButton;
        public TMP_Text setTimeResultText;
        [Space]
        public CustomDropdown buildingDropdown;
        public TMP_Text buildingResultText;
        public GameObject buildingMenuGameObject;
        [Space]
        public TMP_InputField roomNumberInputField;
        public TMP_Text roomNumberResultText;
        public GameObject roomNumberMenuGameObject;
        public InputFieldStatus roomNumberInputFieldStatus;
        [Space]
        public SwitchManager hasCarSwitch;
        public TMP_Text hasCarResultText;
        [Space]
        public TMP_InputField plateNumberInputField;
        public TMP_Text plateNumberResultText;
        public GameObject plateNumberMenuGameObject;
        public InputFieldStatus plateNumberInputFieldStatus;
        [Space]
        public TMP_InputField notesInputField;
        public TMP_Text notesResultText;
        [Space]
        public CustomDropdown reputationDropdown;
        public TMP_Text reputationResultText;
        [Space]
        public Button confirmButton;
        public GameObject confirmPanelGameObject;
    }
}