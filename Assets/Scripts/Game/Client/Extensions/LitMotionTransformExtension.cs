using LitMotion;
using UnityEngine;

namespace Game.Client.Extensions
{
    public static class LitMotionTransformExtension
    {
        public static MotionHandle BindToPosX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform target)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.Bind(target, (x, t) =>
            {
                t.position = new Vector3(x, t.position.y, t.position.z);
            });
        }
        
        public static MotionHandle BindToPosY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform target)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.Bind(target, (y, t) =>
            {
                t.position = new Vector3(t.position.x, y, t.position.z);
            });
        }
        
        public static MotionHandle BindToPosZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform target)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.Bind(target, (z, t) =>
            {
                t.position = new Vector3(t.position.x, t.position.y, z);
            });
        }
    }
}