using Cysharp.Threading.Tasks;
using Masked.Fights;
using Masked.Player;
using Masked.World;
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
        [SerializeField] private string _menuScene = "MainMenu";
        [SerializeField] private string _fightScene = "FightScene";


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
                    InitializeFightController(controller);
                    FightTheFight(controller).Forget();
                }
            }
        }

        private async UniTask FightTheFight(FightController controller)
        {
            var (playerWon, hpAfter) = await controller.AwaitFight();
            _playerManager.SetPlayerHp(hpAfter);

            //TODO: if player hp == 0, return to town

            //TODO: give player da lootz!
            await FromFightToWorld();
        }

        private void InitializeFightController(FightController controller)
        {
            var player = new FightParty(_playerManager.PlayerName, damage: _playerManager.Player.Damage, hp: _playerManager.Player.HP);
            var enemy = new FightParty("Enemy", damage: 1, hp: 15);

            player.Deck = new();
            var (level, deck) = _playerManager.GetDeckByMask();

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