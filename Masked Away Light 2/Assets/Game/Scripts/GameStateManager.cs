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
using UnityEngine.UIElements;

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

        public async UniTask FromWorldToFight(MonsterConfig monsterSelected)
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
                    InitializeFightController(controller, monsterSelected);
                    FightTheFight(controller, monsterSelected, destroyCancellationToken).Forget();
                }
            }
        }

        private async UniTask FightTheFight(FightController controller, MonsterConfig monster, CancellationToken ct)
        {
            var (playerWon, hpAfter) = await controller.AwaitFight(ct);
            _playerManager.SetPlayerHp(hpAfter);

            if (hpAfter <= 0)
            {
                _worldManager.ResetPlayerToTown();
                _playerManager.FullHeal();
                await FromFightToWorld();
                var uiDocument = FindFirstObjectByType<UIDocument>();
                uiDocument.rootVisualElement.Q<VisualElement>("ChatMessage").visible = true;
                uiDocument.rootVisualElement.Q<Label>("ChatLabel").text = "You fainted and woke up back in town.";
                return;
            }

            var expGained = monster.Experience;
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
                _playerManager.FullHeal();
            }

            await FromFightToWorld();
        }

        private MonsterConfig InitializeFightController(FightController controller, MonsterConfig monsterSelected)
        {
            var maxHP = _playerManager.GetMaxHP();
            var player = new FightParty(
                _playerManager.PlayerName,
                damage: _playerManager.Player.Damage,
                hp: Mathf.Min(maxHP, _playerManager.Player.HP),
                maxHP: maxHP);

            var enemy = new FightParty(
                monsterSelected.Name,
                damage: monsterSelected.Damage,
                hp: monsterSelected.HP,
                maxHP: monsterSelected.HP);

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
            enemy.Visuals.SetSprite(monsterSelected.Sprites);
            return monsterSelected;
        }

        public async UniTask FromFightToWorld()
        {
            if (State != State.InFight)
            {
                return;
            }

            _worldManager.TriggerSave();
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
                FromWorldToFight(_monsters[UnityEngine.Random.Range(0, _monsters.Length)]).Forget();
            }
        }
    }
}