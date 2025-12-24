using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Managers.Card.Components
{
    public class CardFaceManager: MonoBehaviour
    {
        [SerializeField] [ChildObjectOnly] [NotNull]
        private List<MeshRenderer> cardValueIconRenderers = new(2);
        [SerializeField] [ChildObjectOnly] [NotNull]
        private List<TMP_Text> cardValueTexts = new(2);
        [SerializeField] [ChildObjectOnly] [NotNull]
        private MeshRenderer cardTopRenderer;
        [SerializeField] [ChildObjectOnly] [NotNull]
        private MeshRenderer cardBottomRenderer;
        [SerializeField] [ChildObjectOnly] [NotNull]
        private MeshRenderer cardExtraContentRenderer;
        
        public void Initialize()
        {
            MeshRenderer[] renderers = {
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
    }
}