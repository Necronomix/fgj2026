using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Masked.Fights
{
    [RequireComponent(typeof(FighterVisualRepresentation))]
    public class ReactToDamage : MonoBehaviour
    {
        [SerializeField] private float _pixelMultiplierForDamage = 1;
        [SerializeField] private FighterVisualRepresentation _visuals;

        public async UniTask TakeDamage(int damage)
        {
            var ct = destroyCancellationToken;
            //DOTween would be nice for this
            var initial = transform.position;
            _visuals.ApplyHitVisual();
            transform.position = initial + new Vector3(0, damage * _pixelMultiplierForDamage);
            await UniTask.WaitForSeconds(0.1f, cancellationToken: ct);
            transform.position = initial - new Vector3(0, 2 * damage * _pixelMultiplierForDamage);
            await UniTask.WaitForSeconds(0.1f, cancellationToken: ct);
            transform.position = initial + new Vector3(0, 2 * damage * _pixelMultiplierForDamage);
            await UniTask.WaitForSeconds(0.1f, cancellationToken: ct);
            transform.position = initial - new Vector3(0, 2 * damage * _pixelMultiplierForDamage);
            await UniTask.WaitForSeconds(0.1f, cancellationToken: ct);
            transform.position = initial;
            _visuals.ResetVisual();
        }
    }
}