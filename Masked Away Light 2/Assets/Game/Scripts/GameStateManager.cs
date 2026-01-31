using Cysharp.Threading.Tasks;
using Masked.Fights;
using Masked.Inventory;
using Masked.Monsters;
using Masked.Player;
using Masked.World;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Masked.GameState
{
    public enum State
    {
        Loading,
        MainMenu,
        OverWorld,
        InFight
    }

    public class GameStateManager : MonoBehaviour
    {
        [SerializeField] private WorldManager _worldManager;
        [SerializeField] private PlayerStateManager _playerManager;
        [SerializeField] private InventoryManager _inventoryManager;
        [SerializeField] private string _menuScene = "MainMenu";
        [SerializeField] private string _fightScene = "FightScene";
        [SerializeField] private MonsterConfig[] _monsters;


        public static GameStateManager Instance;

        public State State;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Instance = this;
            State = State.Loading;

            var loading = SceneManager.LoadSceneAsync(_menuScene, LoadSceneMode.Additive);

            loading.completed += MenuLoaded;
        }

        private void MenuLoaded(AsyncOperation operation)
        {
            State = State.MainMenu;

            //TODO: UI

            FromMenuToWorld().Forget();
        }

        public async UniTask FromMenuToWorld()
        {
            if (State != State.MainMenu)
            {
                return;
            }

            await SceneManager.UnloadSceneAsync(_menuScene);

            await _worldManager.LoadWorld();
            State = State.OverWorld;

            _playerManager.SetPlayerName("NecSimo");
        }

        public async UniTask FromWorldToFight()
        {
            if (State != State.OverWorld)
            {
                return;
            }

            await _worldManager.UnloadWorld();

            var handle = SceneManager.LoadSceneAsync(_fightScene, LoadSceneMode.Additive);
            await handle;
            State = State.InFight;
            var fight = SceneManager.GetSceneAt(SceneManager.sceneCount -1);
            SceneManager.SetActiveScene(fight);
            var roots = fight.GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.TryGetComponent<FightController>(out var controller))
                {
                    var enemyMonster = InitializeFightController(controller);
                    FightTheFight(controller, enemyMonster, destroyCancellationToken).Forget();
                }
            }
        }

        private async UniTask FightTheFight(FightController controller, MonsterConfig monster, CancellationToken ct)
        {
            var (playerWon, hpAfter) = await controller.AwaitFight(ct);
            _playerManager.SetPlayerHp(hpAfter);

            //TODO: if player hp == 0, return to town

            var expGained = 5;
            if (playerWon)
            {
                var rewards = monster.LootPool.LootsWithChance();
                _inventoryManager.GiveItems(rewards);
            }
            var experienceGained = _playerManager.GiveExperience(expGained);
            _playerManager.GiveMaskExperience(expGained);
            //TODO: level up animation
            if (experienceGained == ExperienceGivingResult.LevelUp)
            {
                _playerManager.SetPlayerHp(_playerManager.GetMaxHP());
            }

            await FromFightToWorld();
        }

        private MonsterConfig InitializeFightController(FightController controller)
        {
            var maxHP = _playerManager.GetMaxHP();
            var player = new FightParty(
                _playerManager.PlayerName,
                damage: _playerManager.Player.Damage,
                hp: Mathf.Min(maxHP, _playerManager.Player.HP),
                maxHP: maxHP);

            var enemyMonster = _monsters[UnityEngine.Random.Range(0, _monsters.Length)];

            var enemy = new FightParty(enemyMonster.Name, damage: enemyMonster.Damage, hp: enemyMonster.HP, maxHP: enemyMonster.HP);

            player.Deck = new();
            var (level, deck) = _inventoryManager.GetDeckByMask();

            foreach (var cardInDeck in deck.Cards)
            {
                if (cardInDeck.LevelRequirement > level)
                {
                    continue;
                }

                for (var i = 0; i < cardInDeck.Amount; i++)
                {
                    player.Deck.Add(new CardRepresentation(cardInDeck.Card));
                }
            }

            controller.InitializeFight(player, enemy, this);
            enemy.Visuals.SetSprite(enemyMonster.Sprites);
            return enemyMonster;
        }

        public async UniTask FromFightToWorld()
        {
            if (State != State.InFight)
            {
                return;
            }

            await SceneManager.UnloadSceneAsync(_fightScene);
            
            await _worldManager.LoadWorld();

            State = State.OverWorld;
        }

        public bool TransitionTo(State state)
        {
            if (State == State.Loading)
            {
                return false;
            }
            switch (state)
            {
                case State.Loading:
                    return false;
                case State.MainMenu:
                    break;
                case State.OverWorld:
                    break;
                case State.InFight:
                    break;
                default:
                    break;
            }
            return true;
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;
            
            if (kb.fKey.wasPressedThisFrame)
            {
                FromWorldToFight().Forget();
            }
        }
    }
}