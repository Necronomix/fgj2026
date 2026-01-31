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
        [SerializeField] private Sprite[] _sprites;

        public Deck Deck => _deck;
        public int HP => _hp;
        public int Damage => _damage;
        public Sprite[] Sprites => _sprites;
        public string Name => _name;
    }
}
