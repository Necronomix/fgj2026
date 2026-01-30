using Masked.Player;
using UnityEngine;

namespace Masked.Inventory
{
    public class EquippableBehaviour: MonoBehaviour, IUsed
    {
        public string EquipmentType;

        void IUsed.ItemUsed(InventoryItemRepresentation representation, PlayerStateManager stateManager)
        {
            var others = stateManager.GetEquippedEquipmentsOfType(EquipmentType);
            foreach (var other in others)
            {
                stateManager.Unequip(other);
            }

            stateManager.EquipItem(representation);
        }
    }
}
