using System;
using UnityEngine;

namespace Masked
{
    [Serializable]
    public class CardInDeck
    {
        public Card Card;
        public int Amount;
        public int LevelRequirement;
    }

    [CreateAssetMenu(fileName = "Deck", menuName = "Masked/Deck")]
    public class Deck : ScriptableObject
    {
        [SerializeField] private CardInDeck[] _cards;

        public CardInDeck[] Cards => _cards;
    }
}
