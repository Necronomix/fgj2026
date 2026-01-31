using Masked.Player;
using UnityEngine;

namespace Masked.Inventory
{
    public class ChangeHPOnUse : MonoBehaviour, IUsed
    {
        [SerializeField] private int _changeOfHP = 1;

        void IUsed.ItemUsed(InventoryItemRepresentation item, InventoryManager stateManager, PlayerStateManager playerState)
        {
            playerState.ChangeHPBy(_changeOfHP);
        }
    }
}
