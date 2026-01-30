using Masked.Player;
using UnityEngine;

namespace Masked.Inventory
{
    public class InventoryItemRepresentation
    {
        public int Slot;
        public string PrefabId;
        public string ItemId;
        private InventoryItemBehaviour _behaviour;

        public Sprite Icon => _behaviour.Icon;
        public string Name => _behaviour.Name;

        public bool Equipped;

        public int Amount = 1;
        public InventoryItemBehaviour Behaviour => _behaviour;

        public InventoryItemRepresentation(int slot, InventoryItem item, InventoryItemBehaviour behaviour)
        {
            Slot = slot;
            ItemId = item.ItemId;
            Equipped = item.Equipped;
            PrefabId = item.PrefabId;
            _behaviour = behaviour;
        }

        public void TriggerUsed(PlayerStateManager manager)
        {
            var used = _behaviour.GetComponents<IUsed>();
            //TODO: do we need prio?

            foreach (var component in used)
            {
                component.ItemUsed(this, manager);
            }
        }
    }
}
