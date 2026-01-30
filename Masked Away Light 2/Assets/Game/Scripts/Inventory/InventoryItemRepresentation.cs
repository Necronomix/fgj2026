using UnityEngine;

namespace Masked.Inventory
{
    public class InventoryItemRepresentation
    {
        public string Id;
        private InventoryItemBehaviour _behaviour;

        public Sprite Icon => _behaviour.Icon;
        public string Name => _behaviour.Name;
        public int Amount = 1;

        public InventoryItemRepresentation(InventoryItem item, InventoryItemBehaviour behaviour)
        {
            Id = item.Id;
            _behaviour = behaviour;
        }
    }
}
