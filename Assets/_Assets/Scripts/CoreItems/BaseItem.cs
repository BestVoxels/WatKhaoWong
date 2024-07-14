using System.Collections.Generic;
using UnityEngine;

namespace WatKhaoWong.CoreItems
{
    /// <summary>
    /// A ScriptableObject that represents as a Base Item template for other class to inherit and use further.
    /// </summary>
    /// <remarks>
    /// In practice, you are likely to use a subclass such as `ProfileItem`
    /// </remarks>
    public abstract class BaseItem : ScriptableObject, ISerializationCallbackReceiver
    {
        #region --Properties-- (Inspector)
        [field: Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [field: SerializeField] public string ItemID { get; private set; }

        [field: Tooltip("(Optional, Don't have any use at the Moment) Item name to be displayed in UI.")]
        [field: SerializeField] public string ItemName { get; private set; }

        [field: TextArea]
        [field: Tooltip("(Optional, Don't have any use at the Moment) Item description to be displayed in UI.")]
        [field: SerializeField] public string ItemDescription { get; private set; }
        #endregion



        #region --Fields-- (In Class)
        private static Dictionary<string, BaseItem> s_itemLookupCache;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// Get the inventory item instance from its UUID.
        /// </summary>
        /// <param name="itemID">
        /// String UUID that persists between game instances.
        /// </param>
        /// <returns>
        /// Inventory item instance corresponding to the ID.
        /// </returns>
        public static BaseItem GetFromID(string itemID)
        {
            if (s_itemLookupCache == null)
            {
                s_itemLookupCache = new Dictionary<string, BaseItem>();
                BaseItem[] itemList = Resources.LoadAll<BaseItem>("");
                foreach (BaseItem item in itemList)
                {
                    if (s_itemLookupCache.ContainsKey(item.ItemID))
                    {
                        Debug.LogError($"Looks like there's a duplicate InventoryItem ID for objects: {s_itemLookupCache[item.ItemID]} and {item}");
                        continue;
                    }

                    s_itemLookupCache[item.ItemID] = item;
                }
                if (itemList.Length == 0) Debug.LogError($"Resources can't find any InventoryItem, so will always return null.");
            }
            if (itemID != null && !s_itemLookupCache.ContainsKey(itemID)) Debug.LogError($"Resources can't find an InventoryItem: {itemID}, so will return as null.");
            if (itemID == null || !s_itemLookupCache.ContainsKey(itemID)) return null;

            return s_itemLookupCache[itemID];
        }
        #endregion



        #region --Methods-- (Interface)
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(ItemID))
            {
                ItemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }
        #endregion
    }
}