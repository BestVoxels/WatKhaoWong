using UnityEngine;

namespace WatKhaoWong.Identity
{
    public class AccountRule : MonoBehaviour
    {
        #region --Properties-- (Inspector)
        [field: Header("Debugger Stuffs")]
        [field: SerializeField] public EAccountRole Role { get; private set; } = EAccountRole.Member;
        #endregion
    }
}