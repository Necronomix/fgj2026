using Masked.Player;
using Masked.World;
using System.Threading.Tasks;
using UnityEngine;
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

        public static GameStateManager Instance;

        public State State;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Instance = this;
            State = State.Loading;

            var loading = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);

            loading.completed += MenuLoaded;
        }

        private void MenuLoaded(AsyncOperation operation)
        {
            State = State.MainMenu;

            //TODO: UI

            FromMenuToWorld();
        }

        public async Task FromMenuToWorld()
        {
            await SceneManager.UnloadSceneAsync("MainMenu");

            await _worldManager.LoadWorld();
            State = State.OverWorld;

            _playerManager.SetPlayerName("NecSimo");
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
    }
}