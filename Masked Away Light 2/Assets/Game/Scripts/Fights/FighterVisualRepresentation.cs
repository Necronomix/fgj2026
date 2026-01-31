using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Masked.Fights
{
    public class FighterVisualRepresentation : MonoBehaviour
    {
        [SerializeField] private ReactToDamage _reactToDamage;


        public UniTask TakeDamage(int damage)
        {
            return _reactToDamage.TakeDamage(damage);
        }
    }
}
