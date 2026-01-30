using Masked.Elements;
using System.Linq;
using UnityEngine;

namespace Masked.Fights
{
    internal class EnemyAI
    {
        internal CardRepresentation SelectCard(FightParty enemy, FightParty player, ElementalEffectivenessChart chart)
        {
            if (enemy.Hand == null || enemy.Hand.Count == 0)
            {
                throw new UnityException("No hand was initialized for enemy");
            }

            var pointPairedHand = enemy.Hand.Select((c) => (priority: c.AttackPairing.Effectiveness, card: c));

            pointPairedHand = pointPairedHand.Select(
                p => //Player has always defence if player always starts
                    (priority: p.priority * chart.GetMultiplier(p.card.AttackPairing.Element, player.CurrentDefence.Element),
                     p.card));

            return pointPairedHand.OrderByDescending(p => p.priority).First().card;
        }
    }
}