using Masked.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Masked
{
    [Serializable]
    public class InventoryItemWithChance
    {
        public InventoryItemBehaviour Target;
        public float Chance = 0.1f;
    }

    [CreateAssetMenu(fileName = "Loot Pool", menuName = "Masked/Loot Pool")]
    public class LootPool : ScriptableObject
    {
        [SerializeField] private InventoryItemWithChance[] _loots;

        public List<InventoryItemBehaviour> LootsWithChance()
        {
            var results = new List<InventoryItemBehaviour>();

            foreach (var loot in _loots)
            {
                if (loot.Chance >= UnityEngine.Random.Range(0, 1f))
                {
                    results.Add(loot.Target);
                }
            }
            return results;
        }
    }
}
