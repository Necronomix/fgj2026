using Masked.Utils;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Masked.World
{
    public class WorldManager : MonoBehaviour
    {
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

        public async Task LoadWorld()
        {
            await SceneManager.LoadSceneAsync(_currentArea, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentArea));

            // Spawn player
            var playerObject = Instantiate(_playerPrefab, new Vector3(_currentPosition.x, 0, _currentPosition.y), Quaternion.identity);
            playerObject.GetComponent<Player.PlayerWalkController>().WorldManager = this;

            // Spawn camera
            var cameraObject = Instantiate(_cameraPrefab, new Vector3(_currentPosition.x, 0, _currentPosition.y), Quaternion.identity);
            cameraObject.GetComponent<CameraController>().followTarget = playerObject;

            // Spawn world UI
            Instantiate(_worldUiPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
