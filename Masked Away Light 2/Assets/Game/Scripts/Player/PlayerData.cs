using Masked.Inventory;
using System;

namespace Masked.Player
{
    public interface IPlayerData : IData
    {
        string PlayerName { get; }
        int HP { get; }
        int Damage { get; }
        InventoryData InventoryData { get; }

        void SetPlayerName(string value);
        void SetPlayerHp(int value);
        bool PlaceInInventory(int slot, InventoryItemRepresentation item);
    }

    [Serializable]
    public class PlayerData : Data, IPlayerData
    {
        public MaskData EquippedMask;
        public InventoryData InventoryData;
        public string PlayerName;
        public int CurrentHP;

        string IPlayerData.PlayerName => PlayerName;
        InventoryData IPlayerData.InventoryData => InventoryData;

        int IPlayerData.HP => CurrentHP;

        int IPlayerData.Damage => 3;

        public void SetPlayerName(string value)
        {
            PlayerName = value;
            _changed = true;
        }

        void IPlayerData.SetPlayerHp(int value)
        {
            _changed = CurrentHP != value;
            CurrentHP = value;
        }

        public bool PlaceInInventory(int slot, InventoryItemRepresentation item)
        {
            if (InventoryData.Inventory.TryGetValue(slot, out var data) || slot >= InventoryData.MaximumSize)
            {
                return false;
            }

            InventoryData.Inventory[slot] = new InventoryItem(item);
            return true;
        }
    }

    [Serializable]
    public class MaskData
    {
        public string Name;
        public int Level;
        public int Experience;
    }
}
