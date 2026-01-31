using Cysharp.Threading.Tasks;
using Masked.Elements;
using Masked.GameState;
using Masked.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Masked.Fights
{
    internal class FightController : MonoBehaviour
    {
        [SerializeField] private ElementType[] _elements;
        [SerializeField] private ElementalEffectivenessChart _chart;
        [SerializeField] private Card[] _cards;
        [SerializeField] private GameObject _fightUIPrefab;
        [SerializeField] private FighterVisualRepresentation _playerRepresentation;
        [SerializeField] private FighterVisualRepresentation _enemyRepresentation;
        private int _atStartOfTurnCards = 3;

        private FightParty _player;
        private CardRepresentation _playerCard;
        private FightParty _enemy;

        private FightParty _partyInTurn;
        private EnemyAI _enemyAI;
        private FightParty _partyOutOfTurn;

        private bool _inFight = false;
        private bool _processingTurn = false;
        private FightParty _winningParty;
        private GameStateManager _gameStateManager;
        private UIDocument _uiDocument;

        public void InitializeFight(FightParty player, FightParty enemy, GameState.GameStateManager gameStateManager)
        {
            player.Visuals = _playerRepresentation;
            enemy.Visuals = _enemyRepresentation;
            player.HPBarName = "PlayerHP";
            enemy.HPBarName = "EnemyHP";

            _winningParty = null;
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

            InitializeFightUI();

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Fight initialized between {_player.Name} and {_enemy.Name}");
#endif
        }

        private void InitializeFightUI()
        {
            var ui = GameObject.Instantiate(_fightUIPrefab, Vector3.zero, Quaternion.identity);
            _uiDocument = ui.GetComponent<UIDocument>();

            Button cardUI, cardUI2, cardUI3;
            UpdateCardsUI(_uiDocument, out cardUI, out cardUI2, out cardUI3);

            cardUI.clicked += () =>
            {
                PlayerSelectedCard(0);
            };
            cardUI2.clicked += () =>
            {
                PlayerSelectedCard(1);
            };
            cardUI3.clicked += () =>
            {
                PlayerSelectedCard(2);
            };

            UpdateHP(_player);
            UpdateHP(_enemy);
        }

        private void UpdateHP(FightParty player)
        {
            var hpBar = _uiDocument.rootVisualElement.Q<ProgressBar>(player.HPBarName);
            hpBar.value = player.HP / (float)player.MaxHP * 100;
            hpBar.title = $"{player.HP.ToString()}/{player.MaxHP.ToString()}";
        }

        private void UpdateCardsUI(UIDocument uiDocument, out Button cardUI, out Button cardUI2, out Button cardUI3)
        {
            var firstCard = _player.Hand.FirstOrDefault();

            cardUI = uiDocument.rootVisualElement.Q<Button>("1stCard");
            SetupCardUI(firstCard, cardUI);
            var secondCard = _player.Hand.Skip(1).FirstOrDefault();

            cardUI2 = uiDocument.rootVisualElement.Q<Button>("2ndCard");
            SetupCardUI(secondCard, cardUI2);


            var thirdCard = _player.Hand.Skip(2).FirstOrDefault();

            cardUI3 = uiDocument.rootVisualElement.Q<Button>("3rdCard");
            SetupCardUI(thirdCard, cardUI3);
        }

        private void SetupCardUI(CardRepresentation card, Button cardUI)
        {
            if (card == null)
            {
                cardUI.style.display = DisplayStyle.None;
                return;
            }


            var cardLabel = cardUI.Q<Label>("Name");
            cardLabel.text = card.CardName;

            var atkIcons = new VisualElement[4];
            atkIcons[0] = cardUI.Q<VisualElement>("AtkElement1");
            atkIcons[1] = cardUI.Q<VisualElement>("AtkElement2");
            atkIcons[2] = cardUI.Q<VisualElement>("AtkElement3");
            atkIcons[3] = cardUI.Q<VisualElement>("AtkElement4");
            for (int i = 0; i < atkIcons.Length; i++)
            {
                bool shouldShow = i < (int)card.AttackPairing.Effectiveness;
                var icon = atkIcons[i];

                icon.style.display = shouldShow ? DisplayStyle.Flex : DisplayStyle.None;
                icon.style.backgroundImage = Background.FromSprite(card.AttackPairing.Element.Icon);
            }

            var defIcons = new VisualElement[4];
            defIcons[0] = cardUI.Q<VisualElement>("DefElement1");
            defIcons[1] = cardUI.Q<VisualElement>("DefElement2");
            defIcons[2] = cardUI.Q<VisualElement>("DefElement3");
            defIcons[3] = cardUI.Q<VisualElement>("DefElement4");
            for (int i = 0; i < defIcons.Length; i++)
            {
                bool shouldShow = i < (int)card.DefencePairing.Effectiveness;
                var icon = defIcons[i];

                icon.style.display = shouldShow ? DisplayStyle.Flex : DisplayStyle.None;
                icon.style.backgroundImage = Background.FromSprite(card.DefencePairing.Element.Icon);
            }
        }

        public async UniTask<(bool playerWon, int playerHP)> AwaitFight()
        {
            _inFight = true;

            await UniTask.WaitUntil(() => _winningParty != null);

            _inFight = false;

            return (_winningParty == _player, _player.HP);
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

        public bool PlayerSelectedCard(int index)
        {
            if (index >= _player.Hand.Count )
            {
                return false;
            }

            var card = _player.Hand[index];
            if (_partyInTurn != _player)
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
            if (!_inFight)
            {
                return;
            }

            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.digit1Key.wasPressedThisFrame)
            {
                PlayerSelectedCard(0);
            }
            if (kb.digit2Key.wasPressedThisFrame)
            {
                PlayerSelectedCard(1);
            }
            if (kb.digit3Key.wasPressedThisFrame)
            {
                PlayerSelectedCard(2);
            }

            if (_player.HP <= 0 || _enemy.HP <= 0)
            {
                _winningParty = _player.HP > 0 ? _player : _enemy;
                return;
            }

            if (_processingTurn)
            {
                return;
            }

            if (_partyInTurn == _player && _playerCard != null)
            {
                _processingTurn = true;
                ProcessPlayerTurn(destroyCancellationToken).Forget();
            } else if (_partyInTurn == _enemy)
            {
                _processingTurn = true;
                ProcessEnemyTurn(destroyCancellationToken).Forget();
            }
        }

        private async UniTask ProcessPlayerTurn(CancellationToken ct)
        {
            if (!await UseCardForTurn(_player, _enemy, _playerCard, ct))
            {
                _processingTurn = false;
                return;
            }

            //Reset state
            _playerCard = null;
            HandleEndOfTurn(_player, _enemy);

            Button cardUI, cardUI2, cardUI3;
            UpdateCardsUI(_uiDocument, out cardUI, out cardUI2, out cardUI3);
            _processingTurn = false;
        }

        private void HandleEndOfTurn(FightParty current, FightParty other)
        {
            ClearHandEndOfTurn(current);
            DrawUntil(current, _atStartOfTurnCards);
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"{current.Name} has {current.Deck.Count} in deck, {current.Hand.Count} in hand and {current.DiscardPile.Count} in Discard");
            UnityEngine.Debug.Log($"{current.Name} has {current.HP} HP, {other.Name} has {other.HP} HP");
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

        private async UniTask ProcessEnemyTurn(CancellationToken ct)
        {
            var card = _enemyAI.SelectCard(_enemy, _player, _chart);
            if (card == null)
            {
                throw new System.Exception("No card was selected by enemy");
            }

            var wasUsed = await UseCardForTurn(_enemy, _player, card, ct);
            if (!wasUsed)
            {
                _processingTurn = false;
                return;
            }

            HandleEndOfTurn(_enemy, _player);
            _processingTurn = false;
        }

        private void ClearHandEndOfTurn(FightParty party)
        {
            party.DiscardPile.AddRange(party.Hand);
            party.Hand.Clear();
        }

        public async UniTask<bool> UseCardForTurn(FightParty party, FightParty defendant, CardRepresentation cardPlayed, CancellationToken ct)
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

            await defendant.Visuals.TakeDamage(flooredDamage);

            defendant.HP = Mathf.Max(0, defendant.HP - flooredDamage);
            UpdateHP(defendant);

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"{party.Name} did {flooredDamage} dmg with {cardPlayed.CardName}, leaving them with {defendant.HP} HP");
#endif

            return true;
        }

    }
}
