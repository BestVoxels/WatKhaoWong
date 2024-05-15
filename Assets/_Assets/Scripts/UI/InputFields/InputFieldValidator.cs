using UnityEngine;
using WatKhaoWong.Utils.UI;
using WatKhaoWong.UI.System;

namespace WatKhaoWong.UI.InputFields
{
    public class InputFieldValidator : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private StatusText _statusText;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _statusText = FindAnyObjectByType<StatusText>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~STATIC~ ~Custom Extension~
        public bool ValidateFirstName(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minLength, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsLessThanMinimumLength(inputText, fieldStatus, out resultText, minLength, (status[1].Msg, status[1].Color)))
                return false;

            // TODO CHECK Name is not relates to something bad or pornography
            // TODO CHECK Name is not too short
            // Facebook Example : 
            // 1. Usernames can only contain alphanumeric characters (A-Z, 0-9) and full stops ("."). They can't contain generic terms or domain extensions (e.g.,.com, net), including country extensions
            // 2. Usernames must be at least 5 characters long.

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }

        public bool ValidateLastName(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minLength, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsLessThanMinimumLength(inputText, fieldStatus, out resultText, minLength, (status[1].Msg, status[1].Color)))
                return false;

            // TODO CHECK Name is not relates to something bad or pornography
            // TODO CHECK Name is not too short
            // Facebook Example : 
            // 1. Usernames can only contain alphanumeric characters (A-Z, 0-9) and full stops ("."). They can't contain generic terms or domain extensions (e.g.,.com, net), including country extensions
            // 2. Usernames must be at least 5 characters long.

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }

        public bool ValidateUserName(string inputText, InputFieldStatus fieldStatus, out string resultText, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            // TODO CHECK IF Email or Phone Number is valid.
            // Facebook check if email Domain is Valid,
            // Example: wfek.com is valid website, WhateverNames@wfek.com is consider valid.
            // BUT asdfakljs.com is NOT valid website, WhateverNames@asdfakljs.com is NOT valid.

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }

        public bool ValidatePassword(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minLength, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsLessThanMinimumLength(inputText, fieldStatus, out resultText, minLength, (status[1].Msg, status[1].Color)))
                return false;

            // TODO TooEasy Password

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }

        public bool ValidateConfirmPassword(string inputText, InputFieldStatus fieldStatus, out string resultText, string compareText, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsNotMatch(inputText, fieldStatus, out resultText, compareText, (status[1].Msg, status[1].Color)))
                return false;

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }

        public bool ValidateCode(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minLength, string compareText, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsLessThanMinimumLength(inputText, fieldStatus, out resultText, minLength, (status[1].Msg, status[1].Color)))
                return false;

            if (IsNotMatch(inputText, fieldStatus, out resultText, compareText, (status[2].Msg, status[2].Color)))
                return false;

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        private bool IsNullOrWhiteSpace(string inputText, InputFieldStatus fieldStatus, out string resultText, (string Msg, Color32 Color) status)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                _statusText.Show(status.Msg, status.Color);

                fieldStatus.SetError();
                resultText = string.Empty;
                return true;
            }

            resultText = string.Empty;
            return false;
        }

        private bool IsLessThanMinimumLength(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minLength, (string Msg, Color32 Color) status)
        {
            if (inputText.Length < minLength)
            {
                _statusText.Show(status.Msg, status.Color);

                fieldStatus.SetError();
                resultText = string.Empty;
                return true;
            }

            resultText = string.Empty;
            return false;
        }

        private bool IsNotMatch(string inputText, InputFieldStatus fieldStatus, out string resultText, string compareText, (string Msg, Color32 Color) status)
        {
            if (!compareText.Equals(inputText))
            {
                _statusText.Show(status.Msg, status.Color);

                fieldStatus.SetError();
                resultText = string.Empty;
                return true;
            }

            resultText = string.Empty;
            return false;
        }
        #endregion
    }
}