using UnityEngine;

namespace Masked.Player
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Masked/Player Level config")]
    public class PlayerLevelConfigs : ScriptableObject
    {
        [SerializeField] private int[] _hpPerLevel;
        [SerializeField] private int[] _expPerLevel;
        [SerializeField] private int[] _expForMaskPerLevel;

        public int MaxLevel =>  Mathf.Min(_hpPerLevel.Length, _expPerLevel.Length);
        public int MaxMaskLevel => _expForMaskPerLevel.Length;

        public int GetHPForLevel(int level)
        {
            if (level <= 0)
            {
                return 1;
            }

            if (level > _hpPerLevel.Length)
            {
                return _hpPerLevel[_hpPerLevel.Length - 1];
            }

            return _hpPerLevel[level - 1];
        }

        internal int GetExpForNextLevel(int level)
        {
            if (level <= 0)
            {
                return 1000000;
            }

            if (level > _expPerLevel.Length)
            {
                return _expPerLevel[_expPerLevel.Length - 1];
            }

            return _expPerLevel[level - 1];
        }

        internal int GetExpForNextMaskLevel(int level)
        {
            if (level <= 0)
            {
                return 1000000;
            }

            if (level > _expForMaskPerLevel.Length)
            {
                return _expForMaskPerLevel[_expForMaskPerLevel.Length - 1];
            }

            return _expForMaskPerLevel[level - 1];
        }
    }
}
