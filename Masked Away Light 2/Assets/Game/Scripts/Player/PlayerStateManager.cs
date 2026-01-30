using Masked.Utils;
using System.IO;
using UnityEngine;

namespace Masked.Player
{
    internal class PlayerStateManager : MonoBehaviour
    {
        public static PlayerStateManager Instance;

        private IPlayerData _data;

        public string PlayerName => _data.PlayerName;
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


        public void SetPlayerName(string name)
        {
            _data.SetPlayerName(name);
            JsonAccess.UpdateData(_data, PlayerPath);
        }
    }
}
