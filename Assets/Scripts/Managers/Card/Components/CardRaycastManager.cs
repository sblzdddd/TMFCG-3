using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers.Card.Components
{
    public class CardRaycastManager: MonoBehaviour
    {
        private Camera cam;
        private CardManager previousSelectedCard;
        
        [SerializeField] private LayerMask raycastLayerMask = ~0;
        [SerializeField] private float raycastDistance = 10f;

        private void Start()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            if (TryGetHoveredCard(out var hoveredCard))
            {
                if (previousSelectedCard && previousSelectedCard != hoveredCard)
                    previousSelectedCard.SetSelected(false);
                
                if (hoveredCard && !hoveredCard.IsSelected)
                    hoveredCard.SetSelected(true);
                
                previousSelectedCard = hoveredCard;
            }
            else if (previousSelectedCard && previousSelectedCard.IsSelected)
            {
                previousSelectedCard.SetSelected(false);
                previousSelectedCard = null;
            }
        }

        // raycast the mouse position and retrieve a hovered card
        private bool TryGetHoveredCard(out CardManager card)
        {
            card = null;
            if (!cam || Mouse.current == null) return false;
            var mousePos = Mouse.current.position.ReadValue();
            var ray = cam.ScreenPointToRay(mousePos);

            if (!Physics.Raycast(ray, out var hit, raycastDistance, raycastLayerMask)) 
                return false;
            
            var hitTransform = hit.collider.transform;
            if (!hitTransform.CompareTag("Card")) return false;
            card = hitTransform.GetComponent<CardManager>();
            return card;

        }
    }
}