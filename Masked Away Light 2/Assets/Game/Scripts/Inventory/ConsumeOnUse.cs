using Masked.Player;
using UnityEngine;

namespace Masked.Inventory
{
    public class ConsumeOnUse : MonoBehaviour, IUsed
    {
        void IUsed.ItemUsed(InventoryItemRepresentation item, PlayerStateManager stateManager)
        {
            stateManager.ConsumeItem(item.Slot);
        }
    }
}
