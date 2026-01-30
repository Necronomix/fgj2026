using Masked.Elements;
using Masked.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Masked.Fights
{
    internal class FightController : MonoBehaviour
    {
        [SerializeField] private ElementalEffectivenessChart _chart;
        private int _atStartOfTurnCards = 3;

        private FightParty _player;
        private Card _playerCard;
        private FightParty _enemy;

        private FightParty _partyInTurn;
        private EnemyAI _enemyAI;
        private FightParty _partyOutOfTurn;

        public void InitializeFight(FightParty player, FightParty enemy)
        {
            _player = player;
            _enemy = enemy;

            _partyInTurn = player;
            _partyOutOfTurn = player;

            _enemyAI = new EnemyAI();

            _player.Deck = Shuffle(_player.Deck);
            _enemy.Deck = Shuffle(_enemy.Deck);

            DrawUntil(_player, _atStartOfTurnCards);
            DrawUntil(_enemy, _atStartOfTurnCards);
        }

        private List<Card> Shuffle(List<Card> deck)
        {
            return deck.Shuffle();
        }

        public bool PlayerSelectedCard(Card card)
        {
            if (_partyInTurn != _player)
            {
                return false;
            }
            if (!_player.Hand.Any(h => h == card))
            {
                return false;
            }

            _playerCard = card;
            return true;
        }

        private void Update()
        {
            if (_partyInTurn == _player && _playerCard != null)
            {
                ProcessPlayerTurn();
            }

            if (_partyInTurn == _enemy)
            {
                ProcessEnemyTurn();
            }
        }

        private void ProcessPlayerTurn()
        {
            if (!UseCardForTurn(_player, _enemy, _playerCard))
            {
                return;
            }

            //Reset state
            _playerCard = null;
            HandleEndOfTurn(_player, _enemy);
        }

        private void HandleEndOfTurn(FightParty current, FightParty other)
        {
            ClearHandEndOfTurn(current);
            DrawUntil(current, _atStartOfTurnCards);
            _partyInTurn = other;
            _partyOutOfTurn = current;
        }

        private void DrawUntil(FightParty party, int until)
        {
            var toDraw = until - party.Hand.Count;
            IEnumerable<Card> cards = DrawFromDeck(party, until);

            //Not enough cards in deck to draw all
            if (cards.Count() < until)
            {
                var shuffled = party.DiscardPile.Shuffle();
                party.Deck.AddRange(shuffled);

                var extraDraws = DrawFromDeck(party, cards.Count() - until);
                cards.Concat(extraDraws);
            }

            party.Hand.AddRange(cards);
        }

        private static IEnumerable<Card> DrawFromDeck(FightParty party, int until)
        {
            var cards = party.Deck.Take(until);

            for (var i = 0; i < cards.Count(); i++)
            {
                party.Deck.RemoveAt(0);
            }

            return cards;
        }

        private void ProcessEnemyTurn()
        {
            var card = _enemyAI.SelectCard(_enemy, _player, _chart);
            if (card == null)
            {
                throw new System.Exception("No card was selected by enemy");
            }

            var wasUsed = UseCardForTurn(_enemy, _player, card);
            if (!wasUsed)
            {
                return;
            }

            HandleEndOfTurn(_enemy, _player);
        }

        private void ClearHandEndOfTurn(FightParty party)
        {
            party.DiscardPile.AddRange(party.Hand);
            party.Hand.Clear();
        }

        public bool UseCardForTurn(FightParty party, FightParty defendant, Card cardPlayed)
        {
            //TODO: show damage
            //TODO: is reference check fine?
            if (!party.Hand.Remove(cardPlayed))
            {
                // No card in hard..
                return false;
            }
            party.DiscardPile.Add(cardPlayed);

            party.CurrentDefence = cardPlayed.DefencePairing;

            var damage = cardPlayed.AttackPairing.Effectiveness * party.Damage * Random.Range(0.8f, 1.2f);

            var currentDefenceEffect = defendant.CurrentDefence;

            var reducedDamage = damage * 1 - currentDefenceEffect.Effectiveness /
                _chart.GetMultiplier(cardPlayed.AttackPairing.Element, currentDefenceEffect.Element);
            defendant.HP -= Mathf.FloorToInt(reducedDamage);

            return true;
        }

    }
}
