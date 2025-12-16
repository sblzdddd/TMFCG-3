using UnityEngine;

namespace Panels.Base
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class BasePanel: MonoBehaviour
    {
        protected RectTransform Rt;
        protected CanvasGroup Cg;
        protected int FadePosition = 0;

        protected virtual void Awake()
        {
            Rt = GetComponent<RectTransform>();
            Cg = GetComponent<CanvasGroup>();
            SetInteractability(false);
        }
        
        public virtual void Show(int direction = -1)
        {
            SetInteractability(true);
            if (direction != -1)
            {
                MoCo.FadeIn(direction, Rt);
                return;
            }
            if (FadePosition != -1)
            {
                MoCo.FadeIn(FadePosition, Rt);
                return;
            }
            MoCo.FadeIn(0, Rt);
        }

        public virtual void Hide(int direction = -1)
        {
            SetInteractability(false);
            if (direction != -1)
            {
                MoCo.FadeOut(direction, Rt);
                return;
            }
            if (FadePosition != -1)
            {
                MoCo.FadeOut(FadePosition, Rt);
                return;
            }
            MoCo.FadeOut(0, Rt);
        }

        public void SetInteractability(bool interactable)
        {
            Cg.interactable = interactable;
            Cg.blocksRaycasts = interactable;
            Cg.alpha = interactable ? 1f : 0.75f;
        }
    }
}