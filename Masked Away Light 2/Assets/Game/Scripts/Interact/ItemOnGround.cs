using UnityEngine;

namespace Masked.Interact
{
    public class ItemOnGround : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            Debug.Log("Picked up item: " + gameObject.name);
            Destroy(gameObject);
        }

        void Start()
        {
        }

        void Update()
        {
        }
    }
}