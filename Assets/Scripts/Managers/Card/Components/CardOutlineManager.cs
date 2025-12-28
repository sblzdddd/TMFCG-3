using Extensions;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Managers.Card.Components
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class CardOutlineManager: MonoBehaviour
    {
        [SerializeField, ColorUsage(true, true)] private Color defaultOutlineColor;
        [SerializeField] private float outlineSize=0.015f;
        
        [SerializeField] private Ease ease = Ease.OutQuad;
        
        private MeshRenderer meshRenderer;
        private static readonly int MatColorProp = Shader.PropertyToID("_OutlineColor");
        private static readonly int MatRadiusProp = Shader.PropertyToID("_OutlineRadius");

        private MotionHandle sequence;

        public void Initialize()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(meshRenderer.sharedMaterial);
            // 设置默认材质参数
            meshRenderer.material.SetColor(MatColorProp, defaultOutlineColor);
        }

        public void SetOutline(bool enable=true)
        {
            if (sequence.IsPlaying()) sequence.Cancel();
            var duration = enable ? 0.01f : 0.15f;
            sequence = LSequence.Create()
                .Append(
                    LMotion.Create(meshRenderer.material.GetColor(MatColorProp).a, enable?1:0, duration)
                        .WithEase(ease)
                        .BindToMaterialColorAlpha(meshRenderer.material, MatColorProp)
                )
                .Join(
                    LMotion.Create(meshRenderer.material.GetFloat(MatRadiusProp), enable?outlineSize:0, duration)
                        .WithEase(ease)
                        .BindToMaterialFloat(meshRenderer.material, MatRadiusProp)
                )
                .Run();
        }
    }
}