using System;

namespace Masked.Player
{
    public interface IPlayerData : IData
    {
        string PlayerName { get; }

        void SetPlayerName(string value);
    }

    [Serializable]
    public class PlayerData : Data, IPlayerData
    {
        public MaskData EquippedMask;
        public string PlayerName;

        string IPlayerData.PlayerName => PlayerName;

        public void SetPlayerName(string value)
        {
            PlayerName = value;
            _changed = true;
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
