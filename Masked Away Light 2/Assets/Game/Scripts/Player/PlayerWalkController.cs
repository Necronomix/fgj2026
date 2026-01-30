using Masked.GameState;
using Masked.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Masked.Player
{
    public class PlayerWalkController : MonoBehaviour
    {
        public WorldManager WorldManager { get; set; }

        void Start()
        {
        }

        void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            // Move player with arrow keys
            if (kb.upArrowKey.wasPressedThisFrame)
            {
                WorldManager.CurrentPosition += new Vector2Int(0, 1);
            }
            if (kb.downArrowKey.wasPressedThisFrame)
            {
                WorldManager.CurrentPosition += new Vector2Int(0, -1);
            }
            if (kb.leftArrowKey.wasPressedThisFrame)
            {
                WorldManager.CurrentPosition += new Vector2Int(-1, 0);
            }
            if (kb.rightArrowKey.wasPressedThisFrame)
            {
                WorldManager.CurrentPosition += new Vector2Int(1, 0);
            }

            // Update player transform smoothly to move to currentPosition
            transform.position = Vector3.Lerp(transform.position, new Vector3(WorldManager.CurrentPosition.x, 0, WorldManager.CurrentPosition.y), Time.deltaTime * 10);
        }
    }
}
