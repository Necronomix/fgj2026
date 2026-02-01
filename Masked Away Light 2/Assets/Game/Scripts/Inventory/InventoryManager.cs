using Masked.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Masked.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private InventoryItemBehaviour[] _allItems;
        [SerializeField] private PlayerStateManager _playerStateManager;
        [SerializeField]
        private GameObject _inventoryUiPrefab;
        [SerializeField] private Deck _defaultDeck;

        private GameObject _inventoryUiObject;
        private IPlayerData _data;

        private void Start()
        {
            _data = _playerStateManager.GetPlayerData();
        }

        internal void OpenInventory()
        {
            _inventoryUiObject = Instantiate(_inventoryUiPrefab, Vector3.zero, Quaternion.identity);
            var uiDocument = _inventoryUiObject.GetComponent<UIDocument>();

            var result = uiDocument.rootVisualElement.Query<Button>(name: "Slot").ToList();
            var closeButton = uiDocument.rootVisualElement.Q<Button>(name: "CloseButton");
            closeButton.clicked += CloseInventory;

            var inventory = GetInventory();

            var columnLength = 5;
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < columnLength; col++)
                {
                    var slot = row * columnLength + col;
                    var item = result[slot];
                    var equipped = item.Q<VisualElement>("Equipped");
                    if (!inventory.TryGetValue(slot, out var representation))
                    {
                        equipped.enabledSelf = false;
                        continue;
                    }
                    item.iconImage = Background.FromSprite(representation.Icon);

                    item.clicked += () =>
                    {
                        UseItemInInventory(representation);
                    };

                    equipped.enabledSelf = representation.Equipped;
                }
            }
        }


        public (int level, Deck deck) GetDeckByMask()
        {
            foreach (var item in _data.InventoryData.Inventory)
            {
                if (item.ItemId == _data.EquippedMask)
                {
                    var maskEquipped = _allItems.FirstOrDefault(i => i.Id == item.PrefabId);
                    if (maskEquipped != null && maskEquipped.TryGetComponent<MaskBehaviour>(out var mask))
                    {
                        var level = _data.GetMaskLevel(item.ItemId);

                        return (level, mask.Deck);
                    }
                }
            }

            return (1, _defaultDeck);
        }

        public void EquipItem(InventoryItemRepresentation item, bool isMask)
        {
            _data.Equip(item.Slot);
            if (isMask)
            {
                _data.SetMaskUsed(item.ItemId);
            }
        }

        public Dictionary<int, InventoryItemRepresentation> GetInventory()
        {
            var inventory = new Dictionary<int, InventoryItemRepresentation>();
            foreach (var item in _data.InventoryData.Inventory)
            {
                var inventoryObject = _allItems.FirstOrDefault(i => i.Id == item.PrefabId);

                inventory[item.Slot] = new InventoryItemRepresentation(item.Slot, item, inventoryObject);
            }
            return inventory;
        }

        public void Unequip(InventoryItemRepresentation item)
        {
            _data.Unequip(item.Slot);
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

        public void UseItemInInventory(InventoryItemRepresentation selected)
        {
            selected.TriggerUsed(this, _playerStateManager);
        }

        public void ConsumeItem(string itemId)
        {
            var consumed = _data.InventoryData.Inventory.FirstOrDefault(f => f.ItemId == itemId);
            if (consumed != null)
            {
                _data.InventoryData.Inventory.Remove(consumed);
            }

            CloseInventory();
            OpenInventory();
        }

        public void CloseInventory()
        {
            if (_inventoryUiObject != null)
            {
                GameObject.Destroy(_inventoryUiObject);
            }
        }

        internal void GiveItems(List<InventoryItemBehaviour> rewards)
        {
            foreach (var item in rewards)
            {
                if (_data.TryGetFirstFreeSlot(out var slot))
                {
                    var id = Guid.NewGuid().ToString();
                    _data.PlaceInInventory(slot, new InventoryItem(item.Id, id, slot));
                    if (item.TryGetComponent<MaskBehaviour>(out var equipping))
                    {
                        if (equipping.EquipmentType == "Mask")
                        {
                            _playerStateManager.AddMask(id);
                        }
                    }
                }
            }
        }
    }
}
