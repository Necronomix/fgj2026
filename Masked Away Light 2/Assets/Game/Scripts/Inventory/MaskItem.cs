using System;

namespace Masked.Inventory
{
    [Serializable]
    public class MaskItem
    {
        public string ItemId;
        public int Level;
        public int Experience;

        public MaskItem(int level, int experience, string itemId)
        {
            ItemId = itemId;
            Level = level;
            Experience = experience;
        }
    }
}
