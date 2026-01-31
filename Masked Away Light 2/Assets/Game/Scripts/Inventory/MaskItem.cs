using Masked.Player;
using System;

namespace Masked.Inventory
{
    [Serializable]
    public class MaskItem
    {
        public string ItemId;
        public LevelData LevelData;

        public MaskItem(int level, int experience, string itemId)
        {
            ItemId = itemId;
            LevelData = new LevelData
            {
                Level = level,
                Experience = experience
            };
        }
    }
}
