using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleMystiaUI.Sliders
{
    [DisallowMultipleComponent]
    [AddComponentMenu("SimpleMystiaUI/MSlider", 34)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class MSlider : MSliderBase, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        [SerializeField] private RectTransform m_FillRect;
        [SerializeField] private RectTransform m_HandleRect;
        [SerializeField] private TMP_Text m_ValueDisplay;

        private MSliderVisuals m_Visuals;
        private MSliderInput m_Input;
        private bool m_DelayedUpdateVisuals;
        private bool m_IsDragging;

        public float StepScale => m_Input?.stepScale ?? 1;

        protected MSlider()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Visuals = new MSliderVisuals(m_FillRect, m_HandleRect);
            m_Input = new MSliderInput(this, m_Visuals);
            Set(m_Value, false);
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Visuals?.ClearTracker();
            base.OnDisable();
        }

        protected override void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                m_Visuals?.UpdateCachedReferences();
#endif
            m_Visuals?.UpdateVisuals(normalizedValue, false, m_IsDragging);
            if(m_ValueDisplay)
                m_ValueDisplay.text = value.ToString("F2");
        }

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(value);
#endif
        }

        public virtual void LayoutComplete()
        {
        }

        public virtual void GraphicUpdateComplete()
        {
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (wholeNumbers)
            {
                m_MinValue = Mathf.Round(m_MinValue);
                m_MaxValue = Mathf.Round(m_MaxValue);
            }

            //OnValidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
            if (IsActive())
            {
                m_Visuals?.UpdateCachedReferences();
                // Update rects in next update since other things might affect them even if value didn't change.
                m_DelayedUpdateVisuals = true;
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }
#endif

        protected virtual void Update()
        {
            if (m_DelayedUpdateVisuals)
            {
                m_DelayedUpdateVisuals = false;
                Set(m_Value, false);
                UpdateVisuals();
            }
            if (Application.isPlaying)
            {
                m_Input?.UpdateStepScale();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);
            m_Input?.OnPointerDown(eventData);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            m_IsDragging = true;
            m_Input?.OnDrag(eventData);
        }

        public override void OnMove(AxisEventData eventData)
        {
            m_IsDragging = false;
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            // If input handler didn't handle the movement, let base class handle navigation
            if (!m_Input?.OnMove(eventData, stepSize) ?? false)
            {
                base.OnMove(eventData);
            }
            else
            {
                PlaySelectedSound();
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            m_IsDragging = false;
            eventData.useDragThreshold = false;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }

        /// <summary>
        /// Force-update the slider. Useful if you've changed the properties and want it to update visually.
        /// </summary>
        public void ForceUpdateVisuals()
        {
            m_Visuals?.UpdateCachedReferences();
            UpdateVisuals();
        }
    }
}