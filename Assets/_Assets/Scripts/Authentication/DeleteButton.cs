using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.Authentication
{
    public class DeleteButton : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        #endregion



        #region --Events-- (UnityEvent)
        [Header("Delete Button UI Event")]
        [SerializeField] private UnityEvent _onDeleteButtonClick;
        [SerializeField] private UnityEvent _onDeleteSucceeded;
        #endregion



        #region --Fields-- (In Class)
        private SavingWrapper _savingWrapper;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~UI Button~
        public void OnDeleteButtonClick()
        {
            _onDeleteButtonClick?.Invoke();
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void DeleteAccount()
        {
            FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

            if (user == null) return;

            _savingWrapper.ForceSave(ECategoryNode.Users, EValueNode.State, "Deleted by User");

            user.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                _onDeleteSucceeded?.Invoke();

                // Reload the Scene to make it reset back! reset value back
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
        #endregion
    }
}