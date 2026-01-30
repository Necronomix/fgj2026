using Masked.Inventory;
using Masked.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Masked.Player
{
    internal class PlayerStateManager : MonoBehaviour
    {
        [SerializeField] private InventoryItemBehaviour[] _allItems;

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
                EquippedMask = new MaskData
                {
                    Name = "starter"
                }
            }, PlayerPath);

            _data = data;
        }


        public Dictionary<int, InventoryItemRepresentation> GetInventory()
        {
            var inventory = new Dictionary<int, InventoryItemRepresentation>();
            foreach (var (key, item) in _data.InventoryData.Inventory)
            {
                var inventoryObject = _allItems.FirstOrDefault(i => i.Id == item.Id);

                inventory[key] = new InventoryItemRepresentation(item, inventoryObject);
            }
            return inventory;
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
