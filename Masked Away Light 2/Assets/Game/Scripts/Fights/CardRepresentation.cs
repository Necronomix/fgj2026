using Masked.Elements;

namespace Masked.Fights
{
    public class CardRepresentation
    {
        private Card _card;

        public CardRepresentation(Card card)
        {
            this._card = card;
        }

        public ElementPairing DefencePairing => _card.DefencePairing;
        public ElementPairing AttackPairing => _card.AttackPairing;
        public string CardName => _card.CardName;
    }
}
