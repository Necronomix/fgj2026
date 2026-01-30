using Masked.Inventory;
using Masked.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Masked.Player
{
    public class PlayerStateManager : MonoBehaviour
    {
        [SerializeField] private InventoryItemBehaviour[] _allItems;
        [SerializeField] private PlayerLevelConfigs _levelConfigs;
        [SerializeField] private Deck _defaultDeck;

        public static PlayerStateManager Instance;

        private IPlayerData _data;

        public string PlayerName => _data.PlayerName;
        public IPlayerData Player => _data;
        public string PlayerPath => Path.Combine(Application.persistentDataPath, "player.json");
        private void Awake()
        {
            Instance = this;

            var data = JsonAccess.FetchOrCreateJson(new PlayerData
            {
                PlayerName = "Unown",
                CurrentLevel = 1,
                CurrentHP = _levelConfigs.GetHPForLevel(1)
            }, PlayerPath);

            _data = data;
        }

        //TODO: call this from inventory
        public void UseItemInInventory(InventoryItemRepresentation selected)
        {
            selected.TriggerUsed(this);
        }

        public void EquipItem(InventoryItemRepresentation item)
        {
            _data.Equip(item.Slot);
        }


        public void Unequip(InventoryItemRepresentation item)
        {
            _data.Unequip(item.Slot);
        }

        public Deck GetDeckByMask()
        {
            foreach (var (key, item) in _data.InventoryData.Inventory)
            {
                if (item.ItemId == _data.EquippedMask)
                {
                    var maskEquipped = _allItems.FirstOrDefault(i => i.Id == item.PrefabId);
                    if (maskEquipped != null && maskEquipped.TryGetComponent<MaskBehaviour>(out var mask))
                    {
                        return mask.Deck;
                    }
                }
            }

            return _defaultDeck;
        }

        public List<InventoryItemRepresentation> GetEquippedEquipmentsOfType(string equipmentType)
        {
            var list = new List<InventoryItemRepresentation>();
            var inventory = GetInventory();
            foreach (var (key, item) in inventory)
            {
                if (item.Equipped)
                {
                    var equipment = item.Behaviour.GetComponent<EquippableBehaviour>();
                    if (equipment.EquipmentType == equipmentType)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public Dictionary<int, InventoryItemRepresentation> GetInventory()
        {
            var inventory = new Dictionary<int, InventoryItemRepresentation>();
            foreach (var (key, item) in _data.InventoryData.Inventory)
            {
                var inventoryObject = _allItems.FirstOrDefault(i => i.Id == item.PrefabId);

                inventory[key] = new InventoryItemRepresentation(key, item, inventoryObject);
            }
            return inventory;
        }

        public void ChangeHPBy(int hpChange)
        {
             var maxed = Math.Min(_data.HP + hpChange, _levelConfigs.GetHPForLevel(_data.Level));
            _data.SetPlayerHp(maxed);
        }

        public void ConsumeItem(int slot)
        {
            _data.InventoryData.Inventory.Remove(slot);
        }

        public void SetPlayerName(string name)
        {
            _data.SetPlayerName(name);
            JsonAccess.UpdateData(_data, PlayerPath);
        }

        internal void SetPlayerHp(int hpAfter)
        {
            _data.SetPlayerHp(hpAfter);
        }

        private void Update()
        {
            // Is there risk of out of sync world and other data?
            JsonAccess.UpdateData(_data, PlayerPath);
        }
    }
}
