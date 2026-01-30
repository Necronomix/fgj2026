using Masked.Elements;
using System.Collections.Generic;

namespace Masked.Fights
{
    public class FightParty
    {
        public List<Card> Deck;
        public List<Card> DiscardPile;
        public List<Card> Hand;
        public ElementPairing CurrentDefence;
        public int HP;
        public int MaxHP;

        public float Damage { get; internal set; }
    }
}
