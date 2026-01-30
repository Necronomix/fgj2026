using UnityEngine;

namespace Masked.Elements
{
    [CreateAssetMenu(fileName = "ElementType", menuName = "Masked/ElementType")]
    public class ElementType : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;

        public string Name => _name;
        public Sprite Icon => _icon;
    }
}
