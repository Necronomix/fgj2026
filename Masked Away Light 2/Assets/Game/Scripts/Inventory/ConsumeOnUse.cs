using Masked.Player;
using UnityEngine;

namespace Masked.Inventory
{
    public class ConsumeOnUse : MonoBehaviour, IUsed
    {
        void IUsed.ItemUsed(InventoryItemRepresentation item, InventoryManager stateManager, PlayerStateManager playerManager)
        {
            stateManager.ConsumeItem(item.ItemId);
        }
    }
}
