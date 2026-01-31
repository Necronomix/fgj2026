using Masked.GameState;
using Masked.Interact;
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
        private Vector3 _previousPosition;
        [SerializeField]
        private float _animationSpeedMultiplier = 0.2f;

        // Raycast settings to detect obstacles and prevent movement
        [SerializeField]
        private LayerMask _obstacleLayer;
        [SerializeField]
        private float _obstacleCheckDistance = 0.9f;
        [SerializeField]
        private Vector3 _rayOriginOffset = new Vector3(0f, 0.5f, 0f);

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
            var dir2D = _directions[(int)direction];
            var worldDir = new Vector3(dir2D.x, 0f, dir2D.y);
            var origin = transform.position + _rayOriginOffset;

            _animator.SetBool("Up", direction == WalkDirection.Up);
            _animator.SetBool("Down", direction == WalkDirection.Down);
            _animator.SetBool("Left", direction == WalkDirection.Left);
            _animator.SetBool("Right", direction == WalkDirection.Right);

            // Raycast without layer mask to detect interactables first
            if (Physics.Raycast(origin, worldDir.normalized, out var hitInfo, _obstacleCheckDistance))
            {
                // Interact with all components implementing IInteractable on the hit object
                var interactables = hitInfo.collider.GetComponents<IInteractable>();
                foreach (var interactable in interactables)
                {
                    interactable.Interact();
                }

                // If the hit object's layer is in the obstacle mask, block movement
                if ((_obstacleLayer.value & (1 << hitInfo.collider.gameObject.layer)) != 0)
                {
                    return;
                }
            }

            WorldManager.CurrentPosition += dir2D;
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

            // Update animation speed
            var movementSpeed = (transform.position - _previousPosition).magnitude / Time.deltaTime;
            _animator.SetFloat("Speed", movementSpeed > 0.1f ? _animationSpeedMultiplier : 0f);

            _previousPosition = transform.position;
        }
    }
}
