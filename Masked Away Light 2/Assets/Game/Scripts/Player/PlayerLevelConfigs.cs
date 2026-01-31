using UnityEngine;

namespace Masked.Player
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Masked/Player Level config")]
    public class PlayerLevelConfigs : ScriptableObject
    {
        [SerializeField] private int[] _hpPerLevel;
        [SerializeField] private int[] _expPerLevel;

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
    }
}
