using UnityEngine;
using Firebase.Auth;

namespace WatKhaoWong.Utils.Core
{
    public static class FirebaseUtils
    {
        #region --Properties-- (Computed)
        // Doing this way to PREVENT Null Error from accessing CurrentUser when there is no user. This way it will gets value when it needs, no need to initialize on Start().
        public static string CurrentUserID
        {
            get
            {
                if (IsAuthenticated()) return FirebaseAuth.DefaultInstance.CurrentUser.UserId;

                Debug.LogWarning("Some Class is trying to get 'CurrentUserID' but User is NOT Authenticate Yet, so will get 'null' instead");
                return null;
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public static bool IsAuthenticated() => FirebaseAuth.DefaultInstance.CurrentUser != null;
        #endregion
    }
}