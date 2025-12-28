using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TmiCard.Card;
using TMPro;
using UnityEngine;
using Utils;

namespace Managers.Card.Components
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CardFaceManager: MonoBehaviour
    {
        private static readonly int OuterContentTex = Shader.PropertyToID("_OuterContentTex");
        private static readonly int TopSuitTex = Shader.PropertyToID("_TopSuitTex");
        private static readonly int TopSuitLaserTex = Shader.PropertyToID("_TopSuitLaserTex");
        private static readonly int BottomSuitTex = Shader.PropertyToID("_BottomSuitTex");
        private static readonly int BottomSuitLaserTex = Shader.PropertyToID("_BottomSuitLaserTex");
        private static readonly int IsJqk = Shader.PropertyToID("_IsJQK");

        [SerializeField, ChildObjectOnly, NotNull]
        private List<TMP_Text> cardValueTexts = new(2);
        //                                       0, A, 2, 3, 4, 5, 6, 7, 8, 9, 10, J,  Q,  K
        private readonly int[] top = new[]    { -1, 0, 1, 2, 3, 2, 3, 4, 3, 4, 5, -1, -1, -1 };
        private readonly int[] bottom = new[] {  0, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3,  0,  0,  0 };

        private MeshRenderer meshRenderer;

        private CancellationToken cts;
        
        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            SetFace(CardValue.Ace, CardSuit.Clubs).Forget();
        }

        public async UniTask SetFace(CardValue value, CardSuit suit)
        {
            cardValueTexts.ForEach(text => text.text = $"{value.ToDisplayString()}\n<size=3.2><sprite={(int)suit}>");
            try
            {
                var isSpecial = top[(int)value] == -1;
                meshRenderer.material.SetFloat(IsJqk, isSpecial ? 1f : 0f);
                if (isSpecial)
                {
                    var handle = await AssetLoader.LoadAssetAsync<Texture>($"Suits/Special/{value.ToDisplayString()}.png", cts);
                    meshRenderer.material.SetTexture(OuterContentTex, handle.Result);
                    meshRenderer.material.SetTexture(TopSuitTex, null);
                }
                else
                {
                    var handle = await AssetLoader.LoadAssetAsync<Texture>($"Suits/{suit.ToString()}/Top{top[(int)value]}.png", cts);
                    meshRenderer.material.SetTexture(TopSuitTex, handle.Result);
                    if (top[(int)value] != 0)
                    {
                        var handle2 = await AssetLoader.LoadAssetAsync<Texture>($"Suits/{suit.ToString()}/Top{top[(int)value]}Laser.png", cts);
                        meshRenderer.material.SetTexture(TopSuitLaserTex, handle2.Result);
                    }
                    else
                    {
                        meshRenderer.material.SetTexture(TopSuitLaserTex, null);
                    }
                }
                var handle3 = await AssetLoader.LoadAssetAsync<Texture>($"Suits/{suit.ToString()}/Bottom{bottom[(int)value]}.png", cts);
                meshRenderer.material.SetTexture(BottomSuitTex, handle3.Result);
                var bottomLaser = bottom[(int)value] == 0 ? 1 : bottom[(int)value];
                var handle4 = await AssetLoader.LoadAssetAsync<Texture>($"Suits/{suit.ToString()}/Bottom{bottomLaser}Laser.png", cts);
                meshRenderer.material.SetTexture(BottomSuitLaserTex, handle4.Result);
            }
            catch (OperationCanceledException) {}
            catch (Exception e) {Debug.LogException(e);}
        }
    }
}