using Masked.Elements;
using Masked.GameState;
using Masked.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Masked.Fights
{
    internal class FightController : MonoBehaviour
    {
        [SerializeField] private ElementalEffectivenessChart _chart;
        [SerializeField] private Card[] _cards;
        private int _atStartOfTurnCards = 3;

        private FightParty _player;
        private CardRepresentation _playerCard;
        private FightParty _enemy;

        private FightParty _partyInTurn;
        private EnemyAI _enemyAI;
        private FightParty _partyOutOfTurn;

        private GameStateManager _gameStateManager;

        public void InitializeFight(FightParty player, FightParty enemy, GameState.GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
            //TODO: player and enemy come with their own decks to fight
            var cards = new List<CardRepresentation>();
            for (int i = 0; i < _cards.Length; i++)
            {
                for (int card = 0; card < 10; card++)
                {
                    cards.Add(new CardRepresentation(_cards[i]));
                }
            }

            cards = FillCards(player, cards);
            FillCards(enemy, cards);

            _player = player;
            _enemy = enemy;

            _partyInTurn = player;
            _partyOutOfTurn = player;

            _enemyAI = new EnemyAI();

            _player.Deck = Shuffle(_player.Deck);
            _enemy.Deck = Shuffle(_enemy.Deck);

            DrawUntil(_player, _atStartOfTurnCards);
            DrawUntil(_enemy, _atStartOfTurnCards);

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Fight initialized between {_player.Name} and {_enemy.Name}");
#endif
        }

        private static List<CardRepresentation> FillCards(FightParty player, List<CardRepresentation> cards)
        {
            for (int i = 0; i < 10; i++)
            {
                cards = cards.Shuffle();
                var taken = cards.First();
                cards.RemoveAt(0);
                player.Deck.Add(taken);
            }

            return cards;
        }

        private List<CardRepresentation> Shuffle(List<CardRepresentation> deck)
        {
            return deck.Shuffle();
        }

        public bool PlayerSelectedCard(CardRepresentation card)
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
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Player selected {card.CardName} from {string.Join(", ", _player.Hand)}");
#endif
            return true;
        }

        private void Update()
        {
            if (_player == null || _enemy == null)
            {
                return;
            }

            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.digit1Key.wasPressedThisFrame)
            {
                PlayerSelectedCard(_player.Hand.First());
            }
            if (kb.digit2Key.wasPressedThisFrame)
            {
                PlayerSelectedCard(_player.Hand.Skip(1).First());
            }
            if (kb.digit3Key.wasPressedThisFrame)
            {
                PlayerSelectedCard(_player.Hand.Skip(2).First());
            }

            if (_player.HP <= 0 || _enemy.HP <= 0)
            {
                _enemy = null;
                _gameStateManager.FromFightToWorld();
                return;
            }

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
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"{current.Name} has {current.Deck.Count} in deck, {current.Hand.Count} in hand and {current.DiscardPile.Count} in Discard");
            UnityEngine.Debug.Log($"{current.Name} has {current.HP} HP, {other.Name} has {current.HP} HP");
#endif

            _partyInTurn = other;
            _partyOutOfTurn = current;
        }

        private void DrawUntil(FightParty party, int until)
        {
            var toDraw = until - party.Hand.Count;
            IEnumerable<CardRepresentation> cards = DrawFromDeck(party, until);

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

        private static IEnumerable<CardRepresentation> DrawFromDeck(FightParty party, int until)
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

        public bool UseCardForTurn(FightParty party, FightParty defendant, CardRepresentation cardPlayed)
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

            var currentDefenceEffect = defendant.CurrentDefence == null ? 1 : defendant.CurrentDefence.Effectiveness /
                _chart.GetMultiplier(cardPlayed.AttackPairing.Element, defendant.CurrentDefence.Element);

            var reducedDamage = damage * 1 - currentDefenceEffect;
            var flooredDamage = Mathf.FloorToInt(reducedDamage);
            defendant.HP -= flooredDamage;

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"{party.Name} did {flooredDamage} with {cardPlayed.CardName}, leaving them with {party.HP} HP");
#endif

            return true;
        }

    }
}
