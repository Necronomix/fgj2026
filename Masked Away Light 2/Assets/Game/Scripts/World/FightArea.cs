using Masked.Monsters;
using UnityEngine;

namespace Masked.World
{
    public class FightArea : MonoBehaviour
    {
        [SerializeField]
        private MonsterConfig[] possibleEncounters;
        public MonsterConfig[] PossibleEncounters => possibleEncounters;

        [SerializeField]
        private float encounterChance = 0.1f;
        public float EncounterChance => encounterChance;

        void Start()
        {
        }

        void Update()
        {
        }
    }
}