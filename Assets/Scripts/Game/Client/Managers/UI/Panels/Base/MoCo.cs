using Game.Client.Extensions;
using LitMotion;
using UnityEngine;

namespace Game.Client.Managers.UI.Panels.Base
{
    public static class MoCo
    {
        private static readonly Vector2 WorldCanvasSize =  new(800f, 450f);
        
        public static MotionHandle FadeIn(int direction, RectTransform rt)
        {
            switch (direction)
            {
                case 0:
                    return LMotion.Create(-WorldCanvasSize.x, 0, 0.35f).WithEase(Ease.OutCubic).BindToRectX(rt);
                case 1:
                    return LMotion.Create(WorldCanvasSize.y, 0, 0.35f).WithEase(Ease.OutCubic).BindToRectY(rt);
                case 2:
                    return LMotion.Create(WorldCanvasSize.x, 0, 0.35f).WithEase(Ease.OutCubic).BindToRectX(rt);
                case 3:
                    return LMotion.Create(-WorldCanvasSize.y, 0, 0.35f).WithEase(Ease.OutCubic).BindToRectY(rt);
                default:
                    return LMotion.Create(-WorldCanvasSize.x, 0, 0.35f).WithEase(Ease.OutCubic).BindToRectX(rt);
            }
        }
        public static MotionHandle FadeOut(int direction, RectTransform rt)
        {
            switch (direction)
            {
                case 0:
                    return LMotion.Create(0, -WorldCanvasSize.x, 0.35f).WithEase(Ease.OutCubic).BindToRectX(rt);
                case 1:
                    return LMotion.Create(0, WorldCanvasSize.y, 0.35f).WithEase(Ease.OutCubic).BindToRectY(rt);
                case 2:
                    return LMotion.Create(0, WorldCanvasSize.x, 0.35f).WithEase(Ease.OutCubic).BindToRectX(rt);
                case 3:
                    return LMotion.Create(0, -WorldCanvasSize.y, 0.35f).WithEase(Ease.OutCubic).BindToRectY(rt);
                default:
                    return LMotion.Create(0, -WorldCanvasSize.x, 0.35f).WithEase(Ease.OutCubic).BindToRectX(rt);
            }
        }

    }
}