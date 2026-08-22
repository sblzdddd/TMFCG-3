using Cysharp.Threading.Tasks;
using Game.Client.Managers.UI.Panels.Interfaces;
using UnityEngine;

namespace Game.Client.Managers.UI.Panels.Base
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class BasePanel : MonoBehaviour, IPanel
    {
        public PanelManager Manager { get; set; }
        
        protected CanvasGroup Cg;
        protected RectTransform Rt;

        protected virtual void Awake()
        {
            Cg = GetComponent<CanvasGroup>();
            Rt = GetComponent<RectTransform>();
        }

        // Called immediately after instantiation and injection
        public virtual void OnInitialize()
        {
        }

        // Default behavior: just be visible
        public virtual void OnOpen()
        {
            SetInteractability(true);
        }

        // Default behavior: destroy immediately
        public virtual UniTask OnClose()
        {
            SetInteractability(false);
            return UniTask.CompletedTask;
        }

        // Brought to front / Resumed
        public virtual void OnFocus()
        {
            SetInteractability(true);
        }

        // Brought to back / Paused
        public virtual void OnLostFocus()
        {
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