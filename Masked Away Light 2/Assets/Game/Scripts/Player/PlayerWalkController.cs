using Masked.GameState;
using Masked.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Masked.Player
{
    public class PlayerWalkController : MonoBehaviour
    {
        private WorldManager _worldManager;

        void Start()
        {
            // TODO: FindObjectOfType is deprecated
            _worldManager = FindObjectOfType<WorldManager>();
        }

        void Update()
        {
            var kb = Keyboard.current;
            if (kb == null || _worldManager == null) return;

            // Move player with arrow keys
            if (kb.upArrowKey.wasPressedThisFrame)
            {
                _worldManager.CurrentPosition += new Vector2Int(0, 1);
            }
            if (kb.downArrowKey.wasPressedThisFrame)
            {
                _worldManager.CurrentPosition += new Vector2Int(0, -1);
            }
            if (kb.leftArrowKey.wasPressedThisFrame)
            {
                _worldManager.CurrentPosition += new Vector2Int(-1, 0);
            }
            if (kb.rightArrowKey.wasPressedThisFrame)
            {
                _worldManager.CurrentPosition += new Vector2Int(1, 0);
            }

            // Update player transform smoothly to move to currentPosition
            transform.position = Vector3.Lerp(transform.position, new Vector3(_worldManager.CurrentPosition.x, 0, _worldManager.CurrentPosition.y), Time.deltaTime * 10);
        }
    }
}
