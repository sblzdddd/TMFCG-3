using System;
using UnityEngine;
using UnityEngine.Events;
using SimpleMystiaUI.Selectables;
using SimpleMystiaUI.Utils;

namespace SimpleMystiaUI.Sliders
{
    public abstract class MSliderBase : MSelectable
    {
        [Serializable]
        public class SliderEvent : UnityEvent<float>
        {
        }

        [SerializeField] protected float m_MinValue;
        [SerializeField] protected float m_MaxValue = 1;
        [SerializeField] protected bool m_WholeNumbers;
        [SerializeField] protected float m_StepSize = 0.1f;
        [SerializeField] protected float m_Value;
        [Space] 
        [SerializeField] protected SliderEvent m_OnValueChanged = new SliderEvent();

        /// <summary>
        /// The minimum allowed value of the slider.
        /// </summary>
        public float minValue
        {
            get => m_MinValue;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_MinValue, value))
                {
                    Set(m_Value);
                    UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// The maximum allowed value of the slider.
        /// </summary>
        public float maxValue
        {
            get => m_MaxValue;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_MaxValue, value))
                {
                    Set(m_Value);
                    UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// Should the value only be allowed to be whole numbers?
        /// </summary>
        public bool wholeNumbers
        {
            get => m_WholeNumbers;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_WholeNumbers, value))
                {
                    Set(m_Value);
                    UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// The size of each step. Will be clamped between 0.0001 and (maxValue - minValue).
        /// When wholeNumbers is true, step size is forced to 1.
        /// </summary>
        public float stepSize
        {
            get => m_WholeNumbers ? 1 : m_StepSize;
            set
            {
                float newStep = m_WholeNumbers ? 1 : Mathf.Clamp(value, 0.0001f, Mathf.Abs(maxValue - minValue));
                if (SetPropertyUtility.SetStruct(ref m_StepSize, newStep))
                {
                    Set(m_Value);
                    UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// The current value of the slider.
        /// </summary>
        public virtual float value
        {
            get => wholeNumbers ? Mathf.Round(m_Value) : m_Value;
            set => Set(value);
        }

        /// <summary>
        /// Callback executed when the value of the slider is changed.
        /// </summary>
        public SliderEvent onValueChanged
        {
            get => m_OnValueChanged;
            set => m_OnValueChanged = value;
        }

        /// <summary>
        /// The current value of the slider normalized into a value between 0 and 1.
        /// </summary>
        public float normalizedValue
        {
            get
            {
                if (Mathf.Approximately(minValue, maxValue))
                    return 0;
                return Mathf.InverseLerp(minValue, maxValue, value);
            }
            set => this.value = Mathf.Lerp(minValue, maxValue, value);
        }

        protected float ClampValue(float input)
        {
            // First clamp to min/max range
            float newValue = Mathf.Clamp(input, minValue, maxValue);
            
            if (wholeNumbers)
                return Mathf.Round(newValue);

            // If we have a non-zero step size, snap to the nearest step
            if (!Mathf.Approximately(stepSize, 0))
            {
                // Calculate how many complete steps we can fit
                float steps = (newValue - minValue) / stepSize;
                float roundedSteps = Mathf.Round(steps);
                float snappedValue = minValue + (roundedSteps * stepSize);

                // If we're very close to maxValue, snap to it instead
                if (Mathf.Abs(newValue - maxValue) < stepSize * 0.5f)
                    return maxValue;
                
                // Otherwise use the regular snapped value
                newValue = snappedValue;
                // Re-clamp in case rounding pushed us outside the valid range
                newValue = Mathf.Clamp(newValue, minValue, maxValue);
            }
            
            return newValue;
        }

        /// <summary>
        /// Set the value of the slider without invoking onValueChanged callback.
        /// </summary>
        public virtual void SetValueWithoutNotify(float input)
        {
            Set(input, false);
        }

        /// <summary>
        /// Set the value of the slider.
        /// </summary>
        protected virtual void Set(float input, bool sendCallback = true)
        {
            // Clamp the input
            float newValue = ClampValue(input);

            // If the stepped value doesn't match the last one, it's time to update
            if (Mathf.Approximately(m_Value, newValue))
                return;

            m_Value = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                m_OnValueChanged.Invoke(newValue);
            }
        }

        /// <summary>
        /// Update the visual representation of the slider.
        /// </summary>
        protected abstract void UpdateVisuals();
    }
} 