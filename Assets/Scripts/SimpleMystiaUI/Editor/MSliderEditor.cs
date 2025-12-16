using SimpleMystiaUI.Sliders;
using UnityEditor;
using UnityEngine;

namespace SimpleMystiaUI.Editor
{
    [CustomEditor(typeof(MSlider), true)]
    [CanEditMultipleObjects]
    public class MSliderEditor : MSelectableEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_FillRect"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HandleRect"));
            
            var minProp = serializedObject.FindProperty("m_MinValue");
            var maxProp = serializedObject.FindProperty("m_MaxValue");
            var valueProp = serializedObject.FindProperty("m_Value");
            var wholeNumbersProp = serializedObject.FindProperty("m_WholeNumbers");
            var stepSizeProp = serializedObject.FindProperty("m_StepSize");
            var displayTextProp = serializedObject.FindProperty("m_ValueDisplay");
            
            // Draw min value field and ensure max stays above min
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(minProp);
            if (EditorGUI.EndChangeCheck())
            {
                // If min is changed to be greater than max, increase max
                if (minProp.floatValue >= maxProp.floatValue)
                {
                    maxProp.floatValue = minProp.floatValue + 1;
                }
                // Clamp the value when min changes
                valueProp.floatValue = Mathf.Clamp(valueProp.floatValue, minProp.floatValue, maxProp.floatValue);
                serializedObject.ApplyModifiedProperties();
            }
            
            // Draw max value field and ensure min stays below max
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(maxProp);
            if (EditorGUI.EndChangeCheck())
            {
                // If max is changed to be less than min, decrease min
                if (maxProp.floatValue <= minProp.floatValue)
                {
                    minProp.floatValue = maxProp.floatValue - 1;
                }
                // Clamp the value when max changes
                valueProp.floatValue = Mathf.Clamp(valueProp.floatValue, minProp.floatValue, maxProp.floatValue);
                serializedObject.ApplyModifiedProperties();
            }

            // Draw value as a slider
            EditorGUI.BeginChangeCheck();
            float newValue;
            if (wholeNumbersProp.boolValue)
            {
                newValue = EditorGUILayout.IntSlider(
                    new GUIContent("Value", "Slider value within the range of min and max."),
                    (int)valueProp.floatValue, 
                    (int)minProp.floatValue, 
                    (int)maxProp.floatValue);
            }
            else
            {
                newValue = EditorGUILayout.Slider(
                    new GUIContent("Value", "Slider value within the range of min and max."),
                    valueProp.floatValue, 
                    minProp.floatValue, 
                    maxProp.floatValue);
                
                // Snap to step size if not using whole numbers
                if (!Mathf.Approximately(stepSizeProp.floatValue, 0))
                {
                    float steps = Mathf.Round((newValue - minProp.floatValue) / stepSizeProp.floatValue);
                    newValue = minProp.floatValue + (steps * stepSizeProp.floatValue);
                    newValue = Mathf.Clamp(newValue, minProp.floatValue, maxProp.floatValue);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                valueProp.floatValue = newValue;
                serializedObject.ApplyModifiedProperties();
            }
            
            // Handle whole numbers toggle
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(wholeNumbersProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (wholeNumbersProp.boolValue)
                {
                    // If switching to whole numbers, round all values
                    minProp.floatValue = Mathf.Round(minProp.floatValue);
                    maxProp.floatValue = Mathf.Round(maxProp.floatValue);
                    valueProp.floatValue = Mathf.Round(valueProp.floatValue);
                    stepSizeProp.floatValue = 1f;
                }
                serializedObject.ApplyModifiedProperties();
            }

            // Handle step size - only show when not using whole numbers
            if (!wholeNumbersProp.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                float maxStep = Mathf.Abs(maxProp.floatValue - minProp.floatValue);
                float newStepSize = EditorGUILayout.Slider(
                    new GUIContent("Step Size", "Size of each step. Clamped between 0.0001 and (Max - Min)."),
                    stepSizeProp.floatValue,
                    0.0001f,
                    maxStep);
                if (EditorGUI.EndChangeCheck())
                {
                    stepSizeProp.floatValue = newStepSize;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            // Display current step scale (read-only)
            EditorGUI.BeginDisabledGroup(true);
            var slider = target as MSlider;
            if (slider != null)
            {
                EditorGUILayout.FloatField(new GUIContent("Current Step Scale", "Real-time step scale value from input system"), slider.StepScale);
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space();
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnValueChanged"));
            EditorGUILayout.PropertyField(displayTextProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
} 