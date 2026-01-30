using Masked.Elements;
using UnityEngine;

namespace Masked
{
    [CreateAssetMenu(fileName = "Card", menuName = "Masked/Card")]
    public class Card : ScriptableObject
    {
        [SerializeField] private string _cardName;
        [SerializeField] private ElementPairing _attackPairing;
        [SerializeField] private ElementPairing _defencePairing;
    }
}
