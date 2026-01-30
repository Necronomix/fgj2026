using UnityEngine;

namespace Masked.Player
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Masked/Player Level config")]
    public class PlayerLevelConfigs : ScriptableObject
    {
        [SerializeField] private int[] _hpPerLevel;

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
    }
}
