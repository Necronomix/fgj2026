using Cysharp.Threading.Tasks;
using Masked.GameState;
using Masked.Interact;
using Masked.Inventory;
using Masked.Monsters;
using Masked.World;
using System.Collections.Generic;
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

        private int fightCooldown = 5;

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
            // Use the grid/world position from WorldManager to compute ray origin and direction
            var currentWorldPos = new Vector3(WorldManager.CurrentPosition.x, 0f, WorldManager.CurrentPosition.y);
            var targetWorldPos = currentWorldPos + new Vector3(dir2D.x, 0f, dir2D.y);
            var worldDir = targetWorldPos - currentWorldPos;
            var origin = currentWorldPos + _rayOriginOffset;

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
                    // Its getting late
                    var inventory = GameObject.FindAnyObjectByType<InventoryManager>();
                    if (inventory != null)
                    {
                        interactable.Interact(inventory);
                    }
                }

                // If the hit object's layer is in the obstacle mask, block movement
                if ((_obstacleLayer.value & (1 << hitInfo.collider.gameObject.layer)) != 0)
                {
                    return;
                }
            }

            WorldManager.CurrentPosition += dir2D;

            // Prevent fighting too frequently
            if (fightCooldown > 0)
            {
                fightCooldown--;
                return;
            }
            // After moving, check for fight areas at the new position
            var newWorldPos = new Vector3(WorldManager.CurrentPosition.x, 0f, WorldManager.CurrentPosition.y);
            CheckForFightAreasAtPosition(newWorldPos);
        }

        private void CheckForFightAreasAtPosition(Vector3 worldPos)
        {
            var center = worldPos + new Vector3(0f, 0.5f, 0f);
            var halfExtents = new Vector3(0.45f, 0.5f, 0.45f);

            var encounterChance = 0f;
            var allPossibleEncounters = new List<MonsterConfig>();

            var hits = Physics.OverlapBox(center, halfExtents);
            foreach (var col in hits)
            {
                // Only consider BoxColliders
                if (!(col is BoxCollider)) continue;

                var fightComp = col.GetComponent<FightArea>();
                if (fightComp == null) continue;

                encounterChance = Mathf.Max(encounterChance, fightComp.EncounterChance);

                foreach (var monster in fightComp.PossibleEncounters)
                {
                    allPossibleEncounters.Add(monster);
                }
            }

            if (UnityEngine.Random.Range(0.0f, 1.0f) <= encounterChance && allPossibleEncounters.Count > 0)
            {
                MonsterConfig selectedMonster = allPossibleEncounters[UnityEngine.Random.Range(0, allPossibleEncounters.Count)];
                Debug.Log($"A wild {selectedMonster.Name} appears!");
                GameStateManager.Instance.FromWorldToFight(selectedMonster).Forget();
            }
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
