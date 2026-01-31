using System;
using System.Collections.Generic;

namespace Masked.Inventory
{

    [Serializable]
    public class InventoryItem
    {
        public string PrefabId;
        public string ItemId;
        public int Amount = 1;
        public int Slot;
        public bool Equipped = false;

        public InventoryItem(string prefabId, string itemId, int slot)
        {
            PrefabId = prefabId;
            ItemId = itemId;
            Amount = 1;
            Slot = slot;
        }

        public InventoryItem(InventoryItemRepresentation item)
        {
            Slot = item.Slot;
            PrefabId = item.PrefabId;
            Amount = item.Amount;
            Equipped = item.Equipped;
        }
    }

    [Serializable]
    public class InventoryData
    {
        public int MaximumSize = 20;
        public List<InventoryItem> Inventory = new();


    }
}
