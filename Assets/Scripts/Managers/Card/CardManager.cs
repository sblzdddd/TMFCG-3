using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Managers.Card
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class CardManager: MonoBehaviour
    {
        private MeshRenderer cardBaseRenderer;
        [SerializeField] private List<MeshRenderer> cardValueIconRenderers = new(2);
        [SerializeField] private List<TMP_Text> cardValueTexts = new(2);
        [SerializeField] private MeshRenderer cardTopRenderer;
        [SerializeField] private MeshRenderer cardBottomRenderer;
        [SerializeField] private MeshRenderer cardExtraContentRenderer;
        [SerializeField] private CharacterManager cardCharacterManager;
        [SerializeField] private CardTitleManager cardTitleManager;
        [SerializeField] private CardOutlineManager cardOutlineManager;
        
        private bool isSelected;
        
        public bool IsSelected => isSelected;

        private void Awake()
        {
            cardBaseRenderer = GetComponent<MeshRenderer>();
            
            MeshRenderer[] renderers = {
                cardBaseRenderer,
                cardTopRenderer,
                cardBottomRenderer,
                cardExtraContentRenderer,
                cardValueIconRenderers[0],
                cardValueIconRenderers[1]
            };
            foreach (var r in renderers)
            {
                if (r != null && r.sharedMaterial != null)
                {
                    r.material = new Material(r.sharedMaterial);
                }
            }
        }

        public void SetSelected(bool selected)
        {
            if (!cardOutlineManager) return;
            isSelected = selected;
            if (selected)
                cardOutlineManager.EnableOutline();
            else
                cardOutlineManager.DisableOutline();
        }
    }
}
