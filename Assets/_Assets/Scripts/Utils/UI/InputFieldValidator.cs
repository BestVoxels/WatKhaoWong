using System;
using System.Linq;
using UnityEngine;

namespace WatKhaoWong.Utils.UI
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



        #region --Methods-- (Custom PUBLIC)
        public void CheckAuthTypeCallback(string input, Action<EAuthType> callback)
        {
            if (IsPhoneNumber(input))
            {
                callback?.Invoke(EAuthType.PhoneNumber);
                
            }
            else if (IsEmail(input))
            {
                callback?.Invoke(EAuthType.EmailPassword);
            }
            else
            {
                callback?.Invoke(EAuthType.Unknown);
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Signup~
        public bool ValidateFirstName(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minimum, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsLessThanTargetLength(inputText, fieldStatus, out resultText, minimum, (status[1].Msg, status[1].Color)))
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

        public bool ValidateLastName(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minimum, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsLessThanTargetLength(inputText, fieldStatus, out resultText, minimum, (status[1].Msg, status[1].Color)))
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

        public bool ValidateSignupPhoneNumber(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minimum, byte maximum, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsNotPhoneNumber(inputText, fieldStatus, out resultText, (status[1].Msg, status[1].Color)))
                return false;

            if (IsLessThanTargetLength(inputText, fieldStatus, out resultText, minimum, (status[2].Msg, status[2].Color)))
                return false;

            if (IsMoreThanTargetLength(inputText, fieldStatus, out resultText, maximum, (status[3].Msg, status[3].Color)))
                return false;

            inputText = FormatToThaiCodeIfPossible(inputText); // Add +66 if User's input starts with 0

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }

        public bool ValidateSignupEmail(string inputText, InputFieldStatus fieldStatus, out string resultText, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsNotEmail(inputText, fieldStatus, out resultText, (status[1].Msg, status[1].Color)))
                return false;

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }

        public bool ValidateSignupPassword(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minimum, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsLessThanTargetLength(inputText, fieldStatus, out resultText, minimum, (status[1].Msg, status[1].Color)))
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
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Verification~
        public bool ValidateCode(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minimum, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsLessThanTargetLength(inputText, fieldStatus, out resultText, minimum, (status[1].Msg, status[1].Color)))
                return false;

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~Login~
        public bool ValidateLoginPhoneNumber(string inputText, InputFieldStatus fieldStatus, out string resultText, byte minimum, byte maximum, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsNotPhoneNumber(inputText, fieldStatus, out resultText, (status[1].Msg, status[1].Color)))
                return false;

            if (IsLessThanTargetLength(inputText, fieldStatus, out resultText, minimum, (status[2].Msg, status[2].Color)))
                return false;

            if (IsMoreThanTargetLength(inputText, fieldStatus, out resultText, maximum, (status[3].Msg, status[3].Color)))
                return false;

            inputText = FormatToThaiCodeIfPossible(inputText); // Add +66 if User's input starts with 0

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }

        public bool ValidateLoginEmail(string inputText, InputFieldStatus fieldStatus, out string resultText, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            if (IsNotEmail(inputText, fieldStatus, out resultText, (status[1].Msg, status[1].Color)))
                return false;

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }

        public bool ValidateLoginPassword(string inputText, InputFieldStatus fieldStatus, out string resultText, params (string Msg, Color32 Color)[] status)
        {
            if (IsNullOrWhiteSpace(inputText, fieldStatus, out resultText, (status[0].Msg, status[0].Color)))
                return false;

            fieldStatus.SetNormal();
            resultText = inputText;
            return true;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~For Input Fields~
        /// <summary>
        /// Use as components for ValidateMethod().
        /// BUT it won't work if ValidateMethod() put '!' in front, For Example - outsider calls !IsPhoneNumber() it works on that level
        /// but inside method itself there is another if-block that check again regardless of how the outsider calls with '!' or without.
        /// </summary>
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

        /// <summary>
        /// Use as components for ValidateMethod().
        /// BUT it won't work if ValidateMethod() put '!' in front, For Example - outsider calls !IsPhoneNumber() it works on that level
        /// but inside method itself there is another if-block that check again regardless of how the outsider calls with '!' or without.
        /// </summary>
        private bool IsLessThanTargetLength(string inputText, InputFieldStatus fieldStatus, out string resultText, byte targetLength, (string Msg, Color32 Color) status)
        {
            if (inputText.Length < targetLength)
            {
                _statusText.Show(status.Msg, status.Color);

                fieldStatus.SetError();
                resultText = string.Empty;
                return true;
            }

            resultText = string.Empty;
            return false;
        }

        /// <summary>
        /// Use as components for ValidateMethod().
        /// BUT it won't work if ValidateMethod() put '!' in front, For Example - outsider calls !IsPhoneNumber() it works on that level
        /// but inside method itself there is another if-block that check again regardless of how the outsider calls with '!' or without.
        /// </summary>
        private bool IsMoreThanTargetLength(string inputText, InputFieldStatus fieldStatus, out string resultText, byte targetLength, (string Msg, Color32 Color) status)
        {
            if (inputText.Length > targetLength)
            {
                _statusText.Show(status.Msg, status.Color);

                fieldStatus.SetError();
                resultText = string.Empty;
                return true;
            }

            resultText = string.Empty;
            return false;
        }

        /// <summary>
        /// Use as components for ValidateMethod().
        /// BUT it won't work if ValidateMethod() put '!' in front, For Example - outsider calls !IsPhoneNumber() it works on that level
        /// but inside method itself there is another if-block that check again regardless of how the outsider calls with '!' or without.
        /// </summary>
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

        /// <summary>
        /// Use as components for ValidateMethod().
        /// BUT it won't work if ValidateMethod() put '!' in front, For Example - outsider calls !IsPhoneNumber() it works on that level
        /// but inside method itself there is another if-block that check again regardless of how the outsider calls with '!' or without.
        /// </summary>
        private bool IsNotPhoneNumber(string inputText, InputFieldStatus fieldStatus, out string resultText, (string Msg, Color32 Color) status)
        {
            if (!IsPhoneNumber(inputText))
            {
                _statusText.Show(status.Msg, status.Color);

                fieldStatus.SetError();
                resultText = string.Empty;
                return true;
            }

            resultText = string.Empty;
            return false;
        }

        /// <summary>
        /// Use as components for ValidateMethod().
        /// BUT it won't work if ValidateMethod() put '!' in front, For Example - outsider calls !IsPhoneNumber() it works on that level
        /// but inside method itself there is another if-block that check again regardless of how the outsider calls with '!' or without.
        /// </summary>
        private bool IsNotEmail(string inputText, InputFieldStatus fieldStatus, out string resultText, (string Msg, Color32 Color) status)
        {
            if (!IsEmail(inputText))
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



        #region --Methods-- (Custom PRIVATE)
        private string FormatToThaiCodeIfPossible(string input)
        {
            if (input != string.Empty && input[0] == '0') // Incase 0959504457
            {
                input = input.Substring(1);
                input = input.Insert(0, "+66");
            }
            else if (input != string.Empty && char.IsDigit(input[0])) // Incase 959504457
            {
                input = input.Insert(0, "+66");
            }

            return input;
        }

        private bool IsPhoneNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            if (input != string.Empty && input[0] == '+')
                input = input.Substring(1);
            
            return input.All(char.IsDigit);
        }

        // TODO OPTIONAL CHECK IF Email's Domain is Valid
        // Facebook check if email Domain is Valid,
        // Example: wfek.com is valid website, WhateverNames@wfek.com is consider valid.
        // BUT asdfakljs.com is NOT valid website, WhateverNames@asdfakljs.com is NOT valid.
        private bool IsEmail(string input) => input.Contains('@') && input.Contains('.');
        #endregion
    }
}