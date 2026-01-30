using UnityEngine;

namespace Masked.Masks
{
    [CreateAssetMenu(fileName = "Mask config",  menuName ="Masked/MaskConfig")]
    internal class MaskConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _name;


    }
}
