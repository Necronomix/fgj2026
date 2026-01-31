using Masked.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Masked.Player
{
    [Serializable]
    public class LevelData
    {
        public int Level;
        public int Experience;
    }

    public interface IPlayerData : IData
    {
        string PlayerName { get; }
        int HP { get; }
        int Level { get; }
        int Damage { get; }
        InventoryData InventoryData { get; }
        string EquippedMask { get; }
        List<MaskItem> Masks { get; }
        int CurrentExperience { get; }

        void SetPlayerName(string value);
        void SetPlayerHp(int value);
        bool PlaceInInventory(int slot, InventoryItem item);
        void Unequip(int slot);
        void Equip(int slot);
        int GetMaskLevel(string id);
        void IncreasePlayerLevel();
        void SetExperience(int diff);
        void IncreaseMaskLevel(string id);
        void SetMaskExperience(string id, int experience);
        void SetMaskUsed(string itemId);
        bool TryGetFirstFreeSlot(out int slot);
    }

    [Serializable]
    public class PlayerData : Data, IPlayerData
    {
        public string EquippedMaskId;
        public InventoryData InventoryData;
        public List<MaskItem> MaskData;
        public string PlayerName;
        public int CurrentHP;
        public LevelData CurrentLevel;

        string IPlayerData.EquippedMask => EquippedMaskId;
        string IPlayerData.PlayerName => PlayerName;
        InventoryData IPlayerData.InventoryData => InventoryData;

        int IPlayerData.HP => CurrentHP;

        int IPlayerData.Damage => 3;

        int IPlayerData.Level => CurrentLevel.Level;

        int IPlayerData.CurrentExperience => CurrentLevel.Experience;

        List<MaskItem> IPlayerData.Masks => MaskData;

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

        public bool PlaceInInventory(int slot, InventoryItem item)
        {
            if (InventoryData.Inventory.Any((i) => i.Slot == slot) || slot >= InventoryData.MaximumSize)
            {
                return false;
            }

            InventoryData.Inventory[slot] = item;
            return true;
        }

        void IPlayerData.Unequip(int slot)
        {
            _changed = true;
            var inventoryItem = InventoryData.Inventory.FirstOrDefault(i => i.Slot == slot);

            if (inventoryItem != null)
            {
                inventoryItem.Equipped = false;
            }
        }

        void IPlayerData.Equip(int slot)
        {
            _changed = true;
            var inventoryItem = InventoryData.Inventory.FirstOrDefault(i => i.Slot == slot);

            if (inventoryItem != null)
            {
                inventoryItem.Equipped = true;
            }
        }

        int IPlayerData.GetMaskLevel(string id)
        {
            var mask = MaskData.FirstOrDefault(m => m.ItemId == id);
            return mask == null ? 0 : mask.LevelData.Level;
        }

        void IPlayerData.IncreaseMaskLevel(string id)
        {
            var mask = MaskData.FirstOrDefault(m => m.ItemId == id);
            if (mask == null)
            {
                return;
            }

            mask.LevelData.Level++;
            _changed = true;
        }

        void IPlayerData.SetMaskExperience(string id, int experience)
        {
            var mask = MaskData.FirstOrDefault(m => m.ItemId == id);
            if (mask == null)
            {
                return;
            }
            mask.LevelData.Experience = experience;
            _changed = true;
        }

        void IPlayerData.IncreasePlayerLevel()
        {
            _changed = true;
            CurrentLevel.Level++;
        }

        void IPlayerData.SetExperience(int newAmount)
        {
            _changed = true;
            CurrentLevel.Experience = newAmount;
        }

        void IPlayerData.SetMaskUsed(string itemId)
        {
            EquippedMaskId = itemId;
        }

        bool IPlayerData.TryGetFirstFreeSlot(out int slot)
        {
            for (int i = 0; i < InventoryData.MaximumSize; i++)
            {
                var inventoryItem = InventoryData.Inventory.FirstOrDefault(item => item.Slot == i);

                if (inventoryItem == null)
                {
                    slot = i;
                    return true;
                }
            }
            slot = -1;
            return false;
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
