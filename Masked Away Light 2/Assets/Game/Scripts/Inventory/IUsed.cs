using Masked.Player;

namespace Masked.Inventory
{
    public interface IUsed
    {
         void ItemUsed(InventoryItemRepresentation representation, PlayerStateManager stateManager);
    }
}
