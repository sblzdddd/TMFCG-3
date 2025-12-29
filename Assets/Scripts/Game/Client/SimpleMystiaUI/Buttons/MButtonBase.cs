using Game.Client.SimpleMystiaUI.Selectables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Client.SimpleMystiaUI.Buttons
{
    [DisallowMultipleComponent]
    [AddComponentMenu("SimpleMystiaUI/MButtonBase", 30)]
    public class MButtonBase : MSelectable, ISubmitHandler
    {
        [SerializeField] private UnityEvent onClick;

        public UnityEvent OnClick => onClick;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            PlayClickSound();
            Press();
        }
        
        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            OnClick.Invoke();
        }
    }

}