using LitMotion;
using UnityEngine;

namespace Game.Client.Extensions
{
    public static class LitMotionCanvasGroupExtension
    {
        public static MotionHandle BindToCanvasGroupAlpha<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, CanvasGroup target)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.Bind(target, (x, cg) =>
            {
                cg.alpha = x;
            });
        }
    }
}