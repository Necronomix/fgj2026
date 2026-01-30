using Masked.GameState;
using Masked.World;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Masked.Player
{
    public class PlayerWalkController : MonoBehaviour
    {
        public enum WalkDirection
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
        }

        public WorldManager WorldManager { get; set; }

        private Animator _animator;

        private static Vector2Int[] _directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0),   // Right
        };

        void Start()
        {
            // Find animator in children
            _animator = GetComponentInChildren<Animator>();
            Assert.IsNotNull(_animator, "Animator component not found in children of PlayerWalkController");
        }

        public void Move(WalkDirection direction)
        {
            WorldManager.CurrentPosition += _directions[(int)direction];
            _animator.SetBool("Up", direction == WalkDirection.Up);
            _animator.SetBool("Down", direction == WalkDirection.Down);
            _animator.SetBool("Left", direction == WalkDirection.Left);
            _animator.SetBool("Right", direction == WalkDirection.Right);
        }

        void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            // Move player with arrow keys
            if (kb.upArrowKey.wasPressedThisFrame)
            {
                Move(WalkDirection.Up);
            }
            else if (kb.downArrowKey.wasPressedThisFrame)
            {
                Move(WalkDirection.Down);
            }
            else if (kb.leftArrowKey.wasPressedThisFrame)
            {
                Move(WalkDirection.Left);
            }
            else if (kb.rightArrowKey.wasPressedThisFrame)
            {
                Move(WalkDirection.Right);
            }

            // Update player transform smoothly to move to currentPosition
            transform.position = Vector3.Lerp(transform.position, new Vector3(WorldManager.CurrentPosition.x, 0, WorldManager.CurrentPosition.y), Time.deltaTime * 10);
        }
    }
}
