using Masked.Player;

namespace Masked.Inventory
{
    public interface IUsed
    {
         void ItemUsed(InventoryItemRepresentation representation, InventoryManager stateManager, PlayerStateManager playerState);
    }
}
