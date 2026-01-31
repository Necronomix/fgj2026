using Masked.Inventory;
using UnityEngine;

namespace Masked.Interact
{
    public class ItemOnGround : MonoBehaviour, IInteractable
    {
        [SerializeField] private InventoryItemBehaviour _itemOnGround;

        public void Interact(Inventory.InventoryManager inventory)
        {
            inventory.GiveItems(new System.Collections.Generic.List<Inventory.InventoryItemBehaviour>
            {
                _itemOnGround
            });

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