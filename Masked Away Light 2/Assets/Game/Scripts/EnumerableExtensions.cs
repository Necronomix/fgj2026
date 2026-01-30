using System.Collections.Generic;

namespace Masked.Utils
{
    public static class EnumerableExtensions
    {
        public static List<T> Shuffle<T>(this IEnumerable<T> enumerable, int? seed = default)
        {
            var list = new List<T>(enumerable);
            var count = list.Count;

            var random = SeededRandom(seed);

            while (count > 1)
            {
                count--;
                var index = random.Next(count + 1);
                (list[index], list[count]) = (list[count], list[index]);
            }

            return list;
        }

        private static System.Random SeededRandom(int? seed)
        {
            return seed != null ?
                new System.Random(seed.Value) : new System.Random();
        }
    }
}
