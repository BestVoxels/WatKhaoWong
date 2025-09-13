using System.Threading.Tasks;
using UnityEngine;
using WatKhaoWong.Identities;
//using WatKhaoWong.Utils.Localization;

namespace WatKhaoWong.Core
{
    public class AccountStatusSetter : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private MyUserData _myUserData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _myUserData = GameObject.FindWithTag("Player").GetComponentInChildren<MyUserData>();
        }

        private async void Start()
        {
            await SetAccountStatusIfNoDataInServer(); // This must comes first!

            UpdateCheckinAt();
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                UpdateCheckinAt();
            }
        }

        //private async void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        await _myUserData.SetDataAccountStatus(updateCheckinAt: true);
        //    }

        //    if (Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        await _myUserData.SetDataAccountStatus(false, EAccountStatus.Normal);
        //    }

        //    if (Input.GetKeyDown(KeyCode.Alpha3))
        //    {
        //        await _myUserData.SetDataAccountStatus(false, EAccountStatus.BanTemporary, System.DateTime.Now.AddDays(5), "Reason 1 .....\nReason 2 ......", "#FF7575");
        //    }

        //    if (Input.GetKeyDown(KeyCode.Alpha4))
        //    {
        //        await _myUserData.SetDataAccountStatus(false, EAccountStatus.BanPermanent, null, "Reason 1 .....\nReason 2 ......\nReason 3 ......", "#C30000");
        //    }

        //    if (Input.GetKeyDown(KeyCode.Alpha5))
        //    {
        //        await _myUserData.SetDataAccountStatus(false, EAccountStatus.VIP);
        //    }
        //}
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private async Task SetAccountStatusIfNoDataInServer()
        {
            bool isMyUserDataSaveLoaded = await _myUserData.LoadCompletionSource.Task;

            if (isMyUserDataSaveLoaded == false)
            {
                Debug.LogError("Could not continue SetAccountStatusIfNoDataInServer() on AccountStatusSetter.cs because MyUserData.cs LoadSave() is not completed.");
                return;
            }

            if (!_myUserData.IsAccountStatusExists())
            {
                await _myUserData.SetAccountStatusDefault();
            }
        }

        private async void UpdateCheckinAt()
        {
            await _myUserData.SetDataAccountStatus(updateCheckinAt: true);
        }
        #endregion
    }
}