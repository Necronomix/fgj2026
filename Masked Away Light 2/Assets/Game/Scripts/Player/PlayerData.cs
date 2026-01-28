using System;

namespace Masked.Player
{
    public interface IPlayerData : IData
    {
        void SetPlayerName(string value);
    }

    [Serializable]
    public class PlayerData : Data, IPlayerData
    {
        public MaskData EquippedMask;
        public string PlayerName;

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
