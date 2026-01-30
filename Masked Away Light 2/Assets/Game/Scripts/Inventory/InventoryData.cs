using System;
using System.Collections.Generic;

namespace Masked.Inventory
{
    [Serializable]
    public class InventoryItem
    {
        public string Id;
        public int Amount = 1;

        public InventoryItem(InventoryItemRepresentation item)
        {
            Id = item.Id;
            Amount = item.Amount;
        }
    }

    [Serializable]
    public class InventoryData
    {
        public int MaximumSize;
        public Dictionary<int, InventoryItem> Inventory = new();

        
    }
}
