using System;
using System.Linq;
using UnityEngine;

namespace Masked.Elements
{

    [Serializable]
    public class ElementalEffectiveness
    {
        public ElementType Element;
        public ElementPairing[] EffectivinessOnAttack;
    }

    [CreateAssetMenu(fileName = "ElementEffectivenessChart", menuName = "Masked/EffectivenessChart")]
    internal class ElementalEffectivenessChart : ScriptableObject
    {
        [SerializeField] private ElementalEffectiveness[] _elementals;

        public float GetMultiplier(ElementType attackingType, ElementType defendingType)
        {
            var elementFound = _elementals.FirstOrDefault(e => e.Element.Name == attackingType.Name);
            if (elementFound != null)
            {
                var defendingElement = elementFound.EffectivinessOnAttack.FirstOrDefault(e => e.Element.Name == defendingType.Name);
                if (defendingElement == null)
                {
                    return 1;
                }
                return defendingElement.Effectiveness;
            }
            //Ideally this does not happen
            return -1;
        }

    }
}
