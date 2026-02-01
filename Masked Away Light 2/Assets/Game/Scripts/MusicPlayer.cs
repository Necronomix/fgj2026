using UnityEngine;

namespace Masked.Game
{
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }
}
