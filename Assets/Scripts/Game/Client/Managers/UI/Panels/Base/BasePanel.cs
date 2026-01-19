using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Client.Managers.UI.Panels.Base
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class BasePanel : MonoBehaviour
    {
        public PanelManager Manager { get; set; }
        
        protected CanvasGroup Cg;
        protected RectTransform Rt;

        protected virtual void Awake()
        {
            Cg = GetComponent<CanvasGroup>();
            Rt = GetComponent<RectTransform>();
        }

        public virtual void OnInitialize()
        {
            // Called immediately after instantiation and injection
        }

        public virtual void OnOpen()
        {
            // Default behavior: just be visible
            SetInteractability(true);
        }

        public virtual UniTask OnClose()
        {
            // Default behavior: destroy immediately
            return UniTask.CompletedTask;
        }

        public virtual void OnFocus()
        {
            // Brought to front / Resumed
            SetInteractability(true);
        }

        public virtual void OnLostFocus()
        {
            // Brought to back / Paused
            SetInteractability(false);
        }

        protected void SetInteractability(bool interactable)
        {
            if (Cg == null) Cg = GetComponent<CanvasGroup>();
            Cg.interactable = interactable;
            Cg.blocksRaycasts = interactable;
        }
    }
}