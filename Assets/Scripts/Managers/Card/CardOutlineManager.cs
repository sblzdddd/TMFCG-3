using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Managers.Card
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class CardOutlineManager: MonoBehaviour
    {
        [SerializeField] [ColorUsage(true, true)] private Color defaultOutlineColor;
        [SerializeField] private Vector3 defaultOutlineSize;
        [BeginHorizontal(LabelWidth = 100.0f)] [SerializeField] [NewLabel("Radius: Before")]
        private float radiusBefore = 0.026f;
        [EndHorizontal] [SerializeField] [NewLabel("After")]
        private float radiusAfter = 0.03f;
        
        [SerializeField] private Ease ease = Ease.OutQuad;
        
        private MeshRenderer meshRenderer;
        private static readonly int MatColorProp = Shader.PropertyToID("_Color");
        private static readonly int MatOpacityProp = Shader.PropertyToID("_Opacity");
        private static readonly int MatRadiusProp = Shader.PropertyToID("_Radius");

        private MotionHandle sequence;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(meshRenderer.sharedMaterial);
            meshRenderer.material.SetColor(MatColorProp, defaultOutlineColor);
            meshRenderer.material.SetFloat(MatOpacityProp, 0);
        }

        private void SetOutlineAnimation(float duration=0.15f, bool enableOutline=true)
        {
            if (sequence.IsPlaying()) sequence.Cancel();
            sequence = LSequence.Create()
                .Append(
                    LMotion.Create(meshRenderer.material.GetFloat(MatOpacityProp), enableOutline?1:0, duration)
                        .WithEase(ease)
                        .BindToMaterialFloat(meshRenderer.material, MatOpacityProp)
                )
                .Join(
                    LMotion.Create(transform.localScale, enableOutline?defaultOutlineSize:Vector3.one, duration)
                        .WithEase(ease)
                        .BindToLocalScale(transform)
                )
                .Join(
                    LMotion.Create(meshRenderer.material.GetFloat(MatRadiusProp), enableOutline?radiusAfter:radiusBefore, duration)
                        .WithEase(ease)
                        .BindToMaterialFloat(meshRenderer.material, MatRadiusProp)
                )
                .Run();
        }

        public void SetOutlineColor(Color color, float duration=0.15f)
        {
            LMotion.Create(meshRenderer.material.GetColor(MatColorProp), color, duration)
                .WithEase(ease)
                .BindToMaterialColor(meshRenderer.material, MatColorProp);
        }

        public void EnableOutline(float duration=0.15f)
        {
            SetOutlineAnimation(duration);
        }

        public void DisableOutline(float duration=0.15f)
        {
            SetOutlineAnimation(duration, false);
        }
    }
}