using Masked.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

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

            var uiDocument = FindFirstObjectByType<UIDocument>();
            uiDocument.rootVisualElement.Q<VisualElement>("ChatMessage").visible = true;
            uiDocument.rootVisualElement.Q<Label>("ChatLabel").text = "You picked up " + _itemOnGround.Name + ".";

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