using System;
using System.Collections.Generic;

namespace Masked.Inventory
{
    [Serializable]
    public class MaskItem : InventoryItem
    {
        public int Level;
        public int Experience;

        public MaskItem(int level, int experience, InventoryItemRepresentation item) : base(item)
        {
            Level = level;
            Experience = experience;
        }
    }

    [Serializable]
    public class InventoryItem
    {
        public string PrefabId;
        public string ItemId;
        public int Amount = 1;
        public bool Equipped = false;

        public InventoryItem(InventoryItemRepresentation item)
        {
            PrefabId = item.PrefabId;
            Amount = item.Amount;
            Equipped = item.Equipped;
        }
    }

    [Serializable]
    public class InventoryData
    {
        public int MaximumSize;
        public Dictionary<int, InventoryItem> Inventory = new();


    }
}
