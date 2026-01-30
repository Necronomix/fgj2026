using UnityEngine;

namespace Masked.Fights
{
    internal class FightController : MonoBehaviour
    {
        private FightParty _player;
        private FightParty _enemy;

        public void InitializeFight(FightParty player, FightParty enemy)
        {
            _player = player;
            _enemy = enemy;
        }


        public void SelectCardForTurn(Card cardPlayed)
        {

        }

    }
}
