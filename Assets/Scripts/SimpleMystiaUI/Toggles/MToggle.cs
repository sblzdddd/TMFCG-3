using System;
using SimpleMystiaUI.Selectables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleMystiaUI.Toggles
{
    [AddComponentMenu("SimpleMystiaUI/MToggle", 30)]
    [RequireComponent(typeof(RectTransform))]
    public class MToggle : MSelectable, ISubmitHandler, ICanvasElement, IPointerDownHandler
    {
        
        /// <summary>
        /// UnityEvent callback for when a toggle is toggled.
        /// </summary>
        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        {
        }

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        [SerializeField]
        public Graphic graphic;

        [SerializeField] private MToggleGroup m_Group;

        /// <summary>
        /// Group the toggle belongs to.
        /// </summary>
        public MToggleGroup group
        {
            get { return m_Group; }
            set
            {
                SetMToggleGroup(value, true);
                PlayEffect();
            }
        }

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        public ToggleEvent onValueChanged = new ToggleEvent();

        // Whether the toggle is on
        [Tooltip("Is the toggle currently on or off?")] [SerializeField]
        private bool m_IsOn;

        protected MToggle()
        {
        }

    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
            {
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
                PlayEffect(); // Add this line to update visual state
            }
        }

    #endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
    #if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(m_IsOn);
    #endif
        }

        public virtual void LayoutComplete() {}

        public virtual void GraphicUpdateComplete() {}

        protected override void OnDestroy()
        {
            if (m_Group != null)
                m_Group.EnsureValidState();
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetMToggleGroup(m_Group, false);
            PlayEffect();
        }

        protected override void OnDisable()
        {
            SetMToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Check if isOn has been changed by the animation.
            // Unfortunately there is no way to check if we don�t have a graphic.
            if (graphic != null)
            {
                bool oldValue = !Mathf.Approximately(graphic.canvasRenderer.GetColor().a, 0);
                if (m_IsOn != oldValue)
                {
                    m_IsOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetMToggleGroup(MToggleGroup newGroup, bool setMemberValue)
        {
            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (m_Group != null)
                m_Group.UnregisterMToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (newGroup != null && IsActive())
                newGroup.RegisterMToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && isOn && IsActive())
                newGroup.NotifyMToggleOn(this);
        }

        /// <summary>
        /// Whether the toggle is currently active.
        /// </summary>
        public bool isOn
        {
            get { return m_IsOn; }

            set
            {
                Set(value);
            }
        }

        /// <summary>
        /// Set isOn without invoking onValueChanged callback.
        /// </summary>
        /// <param name="value">New Value for isOn.</param>
        public void SetIsOnWithoutNotify(bool value)
        {
            Set(value, false);
        }

        void Set(bool value, bool sendCallback = true)
        {
            if (m_IsOn == value)
                return;

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && m_Group.isActiveAndEnabled && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyMTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyMToggleOn(this, sendCallback);
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            PlayEffect();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Toggle.value", this);
                onValueChanged.Invoke(m_IsOn);
            }
        }

        public void PlayEffect()
        {
            if (graphic == null)
                return;

            graphic.canvasRenderer.SetAlpha(m_IsOn ? 1f : 0f);
        }

        protected override void Start()
        {
            PlayEffect();
        }

        private bool canTurnOff()
        {
            if (m_Group != null && m_Group.isActiveAndEnabled && IsActive())
            {
                return m_Group.allowSwitchOff;
            }
            return true;
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            isOn = !isOn;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
                
            if(isOn && !canTurnOff()) return;

            base.OnPointerDown(eventData);

            InternalToggle();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if(isOn && !canTurnOff()) return;
            PlayClickSound();
            InternalToggle();
        }
    }
}
