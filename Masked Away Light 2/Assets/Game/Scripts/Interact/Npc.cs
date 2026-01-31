using Masked.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace Masked.Interact
{
    public class Npc : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string _message;

        public void Interact(InventoryManager inventory)
        {
            FindFirstObjectByType<UIDocument>().rootVisualElement.Q<VisualElement>("ChatMessage").visible = true;
            FindFirstObjectByType<UIDocument>().rootVisualElement.Q<Label>("ChatLabel").text = _message;
        }

        void Start()
        {
        }

        void Update()
        {
        }
    }
}