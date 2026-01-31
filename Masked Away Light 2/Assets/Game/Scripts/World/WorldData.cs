using System;

namespace Masked.World
{
    [Serializable]
    public class WorldData : Data
    {
        public string Location;
        public int[] Coordinates;

        internal void FlagChanged()
        {
            _changed = true;
        }
    }
}
