using Masked.Inventory;
using Masked.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Masked.Player
{
    public enum ExperienceGivingResult
    {
        GaveExp,
        LevelUp
    }

    public class PlayerStateManager : MonoBehaviour
    {
        [SerializeField] private PlayerLevelConfigs _levelConfigs;

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
                CurrentLevel = new LevelData
                {
                    Level = 1
                },
                InventoryData = new InventoryData
                {
                    MaximumSize = 20,
                    Inventory = new List<InventoryItem>()
                },
                MaskData = new List<MaskItem>(),
                CurrentHP = _levelConfigs.GetHPForLevel(1)
            }, PlayerPath);

            _data = data;
        }



        public int GetMaxHP()
        {
            return _levelConfigs.GetHPForLevel(_data.Level);
        }

        public void ChangeHPBy(int hpChange)
        {
             var maxed = Math.Min(_data.HP + hpChange, _levelConfigs.GetHPForLevel(_data.Level));
            _data.SetPlayerHp(maxed);
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

        public ExperienceGivingResult GiveExperience(int newExperience)
        {
            var neededForNextLevel = _levelConfigs.GetExpForNextLevel(_data.Level);

            var diff = newExperience + _data.CurrentExperience - neededForNextLevel;
            if (diff > 0 && _data.Level < _levelConfigs.MaxLevel)
            {
                _data.IncreasePlayerLevel();
                _data.SetExperience(diff);
                return ExperienceGivingResult.LevelUp;
            }

            _data.SetExperience(newExperience + _data.CurrentExperience);
            return ExperienceGivingResult.GaveExp;
        }

        internal void GiveMaskExperience(int expGained)
        {
            if (string.IsNullOrEmpty(_data.EquippedMask))
            {
                return;
            }

            var mask = _data.Masks.FirstOrDefault(m => m.ItemId == _data.EquippedMask);
            if (mask == null)
            {
                return;
            }

            var neededForNextLevel = _levelConfigs.GetExpForNextMaskLevel(mask.LevelData.Level);
            var diff = mask.LevelData.Experience + expGained - neededForNextLevel;
            if (diff > 0 && mask.LevelData.Level < _levelConfigs.MaxMaskLevel)
            {
                _data.IncreaseMaskLevel(mask.ItemId);
                _data.SetMaskExperience(mask.ItemId, diff);
                return;
            }

            _data.SetMaskExperience(mask.ItemId, expGained + mask.LevelData.Experience);
        }

        internal IPlayerData GetPlayerData()
        {
            return _data;
        }
    }
}
