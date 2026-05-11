using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WatKhaoWong.Retreats
{
    [System.Serializable]
    public class StatusSetterInspector
    {
        public CustomDropdown statusDropdown;
        public Button setTimeButton;
        public TMP_Text infoText;
        public TMP_InputField notesInputField;
        public Button confirmButton;
        public GameObject[] gameOjectsToShowHide;
    }
}