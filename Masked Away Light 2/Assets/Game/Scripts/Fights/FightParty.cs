using Masked.Elements;
using System.Collections.Generic;

namespace Masked.Fights
{
    public class FightParty
    {
        public string Name;
        public List<CardRepresentation> Deck;
        public List<CardRepresentation> DiscardPile;
        public List<CardRepresentation> Hand;
        public ElementPairing CurrentDefence;
        public int HP;
        public int MaxHP;
        public FighterVisualRepresentation Visuals;

        public FightParty(string name, int damage, int hp)
        {
            Name = name;
            Damage = damage;
            HP = hp;

            Deck = new List<CardRepresentation>();
            DiscardPile = new List<CardRepresentation>();
            Hand = new List<CardRepresentation>();
        }

        public float Damage { get; internal set; }
    }
}
