using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Masked.Fights
{
    public class FighterVisualRepresentation : MonoBehaviour
    {
        public const int IDLE_SPRITE = 0;
        public const int HIT_SPRITE = 1;
        public const int ATTACK_SPRITE = 2;
        public const int DEAD_SPRITE = 3;

        [SerializeField] private ReactToDamage _reactToDamage;
        [SerializeField] private SpriteRenderer _image;


        private Sprite[] _sprites;

        public void SetSprite(Sprite[] sprites)
        {
            _sprites = sprites;
            _image.sprite = sprites[IDLE_SPRITE];
        }

        public UniTask TakeDamage(int damage)
        {
            return _reactToDamage.TakeDamage(damage);
        }

        internal void ApplyHitVisual()
        {
            if (_sprites == null)
            {
                return;
            }
            _image.sprite = _sprites[HIT_SPRITE];
        }

        internal void ResetVisual()
        {
            if (_sprites == null)
            {
                return;
            }
            _image.sprite = _sprites[IDLE_SPRITE];
        }

        internal void AttackVisual()
        {
            if (_sprites == null)
            {
                return;
            }
            _image.sprite = _sprites[ATTACK_SPRITE];
        }

        internal void DeadVisual()
        {
            if (_sprites == null)
            {
                return;
            }
            _image.sprite = _sprites[DEAD_SPRITE];
        }
    }
}
