using UnityEngine;

namespace Masked.World
{
    public class CameraController : MonoBehaviour
    {
        public GameObject followTarget;

        [SerializeField]
        private Vector3 cameraOffset = new Vector3(0, 9, -7);

        void Start()
        {
        }

        void LateUpdate()
        {
            if (followTarget != null)
            {
                transform.position = followTarget.transform.position + cameraOffset;
                transform.LookAt(followTarget.transform);
            }
        }
    }
}