using Game.Client.Extensions;
using LitMotion;
using UnityEngine;

namespace Game.Client.Managers.UI.Panels.Base
{
    public static class MoCo
    {
        public static MotionHandle FadeIn(CanvasGroup cg, float duration = 0.35f, Ease ease = Ease.OutCubic)
        {
            return LMotion.Create(0f, 1f, duration).WithEase(ease).BindToCanvasGroupAlpha(cg);
        }
        public static MotionHandle FadeOut(CanvasGroup cg, float duration = 0.35f, Ease ease = Ease.OutCubic)
        {
            return LMotion.Create(1f, 0f, duration).WithEase(ease).BindToCanvasGroupAlpha(cg);
        }

    }
}