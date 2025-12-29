using LitMotion;
using UnityEngine;

namespace Game.Client.Extensions
{
    public static class LitMotionMaterialExtension
    {
        public static MotionHandle BindToMaterialColorAlpha<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Material target, int nameId)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.Bind(target, (x, mat) =>
            {
                var c = mat.GetColor(nameId);
                mat.SetColor(nameId, new Color(c.r, c.g, c.b, x));;
            });
        }
    }
}