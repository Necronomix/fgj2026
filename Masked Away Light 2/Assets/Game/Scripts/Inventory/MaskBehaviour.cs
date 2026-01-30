using UnityEngine;

namespace Masked.Inventory
{
    public class MaskBehaviour : EquippableBehaviour
    {
        [SerializeField] private Deck _deck;

        public Deck Deck => _deck;
    }
}
