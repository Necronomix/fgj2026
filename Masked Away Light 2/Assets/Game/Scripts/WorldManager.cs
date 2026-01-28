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

            //TODO: place by vectors
        }
    }
}
