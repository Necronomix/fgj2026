using UnityEngine;

namespace Masked.Monsters
{
    [CreateAssetMenu(fileName = "Monster", menuName = "Masked/Monster config")]
    public class MonsterConfig : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private Deck _deck;
        [SerializeField] private int _hp;
        [SerializeField] private int _damage;
        [SerializeField] private int _experience;
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private LootPool _lootPool;

        public Deck Deck => _deck;
        public int HP => _hp;
        public int Damage => _damage;
        public int Experience => _experience;
        public Sprite[] Sprites => _sprites;
        public string Name => _name;
        public LootPool LootPool => _lootPool;
    }
}
