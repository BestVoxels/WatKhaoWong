using UnityEngine;
using WatKhaoWong.CoreItems;

namespace WatKhaoWong.Identities
{
    /// <summary>
    /// An inventory item that will be transfer to Coin Economy System of the collector. (Mechanism is at Coin.cs)
    /// </summary>
    /// <remarks>
    /// This class should be used as is. No need to have Subclasses OR itself class implementation.
    /// </remarks>
    [CreateAssetMenu(fileName = "Untitled", menuName = "WatKhaoWong/Item/New Profile Icon", order = 0)]
    public class ProfileIconItem : BaseItem
    {
        #region --Properties-- (Inspector)
        [field: Tooltip("Profile Icon UI GameObject.")]
        [field: SerializeField] public ProfileIconUI ProfileIconUI { get; private set; }
        #endregion
    }
}