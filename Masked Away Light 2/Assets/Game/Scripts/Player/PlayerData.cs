using System;

namespace Masked.Player
{
    public interface IPlayerData : IData
    {
        string PlayerName { get; }
        int HP { get; }
        int Damage { get; }

        void SetPlayerName(string value);
        void SetPlayerHp(int value);
    }

    [Serializable]
    public class PlayerData : Data, IPlayerData
    {
        public MaskData EquippedMask;
        public string PlayerName;
        public int CurrentHP;

        string IPlayerData.PlayerName => PlayerName;

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
    }

    [Serializable]
    public class MaskData
    {
        public string Name;
        public int Level;
        public int Experience;
    }
}
