using Cysharp.Threading.Tasks;
using Masked.GameState;
using Masked.Player;
using Masked.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Masked.World
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private GameStateManager _gameStateManager;
        [SerializeField] private PlayerStateManager _playerStateManager;

        private string _currentArea;
        private Vector2Int _currentPosition;
        public Vector2Int CurrentPosition
        {
            get => _currentPosition;
            set => _currentPosition = value;
        }

        [SerializeField]
        private GameObject _playerPrefab;
        [SerializeField]
        private GameObject _cameraPrefab;
        [SerializeField]
        private GameObject _worldUiPrefab;
        [SerializeField]
        private GameObject _inventoryUiPrefab;
        private GameObject _inventoryUiObject;

        public string WorldPath => Path.Combine(Application.persistentDataPath, "world.json");

        private void Awake()
        {
            WorldData data = null;
            data = JsonAccess.FetchOrCreateJson(new WorldData
            {
                Coordinates = new[] { 0, 0 },
                Location = "City"
            }, WorldPath);
            UpdateFromData(data);
        }

        private void UpdateFromData(WorldData data)
        {
            _currentArea = data.Location;
            _currentPosition = ConvertArrayToVector(data.Coordinates);
        }

        private Vector2Int ConvertArrayToVector(int[] coordinates)
        {
            if (coordinates.Length < 2)
            {
                return new Vector2Int(0, 0);
            }
            return new Vector2Int(coordinates[0], coordinates[1]);
        }

        public async UniTask LoadWorld()
        {
            await SceneManager.LoadSceneAsync(_currentArea, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentArea));

            // Spawn player
            var playerObject = Instantiate(_playerPrefab, new Vector3(_currentPosition.x, 0, _currentPosition.y), Quaternion.identity);
            var playerWalkController = playerObject.GetComponent<Player.PlayerWalkController>();
            playerWalkController.WorldManager = this;

            // Spawn camera
            var cameraObject = Instantiate(_cameraPrefab, new Vector3(_currentPosition.x, 0, _currentPosition.y), Quaternion.identity);
            cameraObject.GetComponent<CameraController>().followTarget = playerObject;

            // Spawn world UI
            var worldUiObject = Instantiate(_worldUiPrefab, Vector3.zero, Quaternion.identity);
            var uiDocument = worldUiObject.GetComponent<UIDocument>();

            uiDocument.rootVisualElement.Q<Button>("Inventory").clicked += () =>
            {
                OpenInventory();
            };
            uiDocument.rootVisualElement.Q<Button>("Up").clicked += () =>
            {
                playerWalkController.Move(Player.PlayerWalkController.WalkDirection.Up);
            };
            uiDocument.rootVisualElement.Q<Button>("Down").clicked += () =>
            {
                playerWalkController.Move(Player.PlayerWalkController.WalkDirection.Down);
            };
            uiDocument.rootVisualElement.Q<Button>("Left").clicked += () =>
            {
                playerWalkController.Move(Player.PlayerWalkController.WalkDirection.Left);
            };
            uiDocument.rootVisualElement.Q<Button>("Right").clicked += () =>
            {
                playerWalkController.Move(Player.PlayerWalkController.WalkDirection.Right);
            };
        }

        private void OpenInventory()
        {
            _inventoryUiObject = Instantiate(_inventoryUiPrefab, Vector3.zero, Quaternion.identity);
            var uiDocument = _inventoryUiObject.GetComponent<UIDocument>();

            var result = uiDocument.rootVisualElement.Query<Button>(name: "Slot").ToList();
            var closeButton = uiDocument.rootVisualElement.Q<Button>(name: "CloseButton");
            closeButton.clicked += () =>
            {
                if (_inventoryUiObject)
                {
                    GameObject.Destroy(_inventoryUiObject);
                }
            }; 

            var inventory =  _playerStateManager.GetInventory();

            var columnLength = 5;
            for (int row = 0; row < 4; row++) {
                for (int col = 0; col < columnLength; col++)
                {
                    var slot = row * columnLength + col;
                    var item = result[slot];
                    var equipped = item.Q<VisualElement>("Equipped");
                    if (!inventory.TryGetValue(slot, out var representation))
                    {
                        equipped.enabledSelf = false;
                        continue;
                    }
                    item.iconImage = Background.FromSprite(representation.Icon);

                    item.clicked += () =>
                    {
                        _playerStateManager.UseItemInInventory(representation);
                    };
                    
                    equipped.enabledSelf = representation.Equipped;
                }
            }
        }

        private void SlotSelected(int slot)
        {
           
        }

        internal async UniTask UnloadWorld()
        {
            if (_inventoryUiPrefab != null)
            {
                GameObject.Destroy(_inventoryUiObject);
            }
            await SceneManager.UnloadSceneAsync(_currentArea);
        }
    }
}
