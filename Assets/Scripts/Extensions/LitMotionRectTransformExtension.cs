using LitMotion;
using UnityEngine;

namespace Extensions
{
    public static class LitMotionRectTransformExtension
    {
        public static MotionHandle BindToRectX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, RectTransform target)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.Bind(target, (x, rt) =>
            {
                rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y);
            });
        }
        public static MotionHandle BindToRectY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, RectTransform target)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.Bind(target, (y, rt) =>
            {
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
            });
        }
    }
}