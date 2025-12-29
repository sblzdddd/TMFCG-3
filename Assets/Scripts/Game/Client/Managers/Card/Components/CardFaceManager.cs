using System.Collections.Generic;
using System.Threading;
using Game.Shared.TmiCards.Card;
using TMPro;
using UnityEngine;

namespace Game.Client.Managers.Card.Components
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CardFaceManager: MonoBehaviour
    {
        private static readonly int OuterContentTex = Shader.PropertyToID("_OuterContentTex");
        private static readonly int TopMask = Shader.PropertyToID("_TopMask");
        private static readonly int BottomMask = Shader.PropertyToID("_BottomMask");
        private static readonly int SuitIndex = Shader.PropertyToID("_SuitIndex");

        [SerializeField, NotNull, ReorderableList]
        private List<Texture2D> topMasks = new();
        [SerializeField, NotNull, ReorderableList]
        private List<Texture2D> bottomMasks = new();
        [SerializeField, NotNull, ReorderableList]
        private List<Texture2D> outerContents = new();

        [SerializeField, ChildObjectOnly, NotNull]
        private List<TMP_Text> cardValueTexts = new(2);
        //                                 0, A, 2, 3, 4, 5, 6, 7, 8, 9, 10, J,  Q,  K
        private readonly int[] top =    { -1, 0, 5, 4, 3, 4, 3, 2, 3, 2, 1, -1, -1, -1 };
        private readonly int[] bottom = {  0, 3, 3, 3, 3, 2, 2, 2, 1, 1, 1,  0,  0,  0 };

        private MeshRenderer meshRenderer;

        private CancellationToken cts;
        
        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            SetFace(CardValue.Ace, CardSuit.Clubs);
        }

        public void SetFace(CardValue value, CardSuit suit)
        {
            var suitIndex = (int)suit;
            var valueIndex = (int)value;
            cardValueTexts.ForEach(text => text.text = $"{value.ToDisplayString()}\n<size=3.2><sprite={suitIndex}>");

            meshRenderer.material.SetTexture(TopMask, suitIndex <= 3 ? 
                topMasks[(int)suit] : null);
            meshRenderer.material.SetTexture(BottomMask, suitIndex <= 3 ? 
                bottomMasks[(int)suit] : null);
            meshRenderer.material.SetTexture(OuterContentTex, top[valueIndex] == -1?
                outerContents[valueIndex-11] : null);
            meshRenderer.material.SetVector(SuitIndex, new Vector4(bottom[(int)value], top[(int)value]));
        }
    }
}