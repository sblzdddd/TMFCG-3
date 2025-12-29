using Game.Client.SimpleMystiaUI.Toggles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleMystiaUI.Editor
{
    [CustomEditor(typeof(MToggle), true)]
    [CanEditMultipleObjects]
    public class MToggleEditor : MSelectableEditor
    {
        
        SerializedProperty m_OnValueChangedProperty;
        SerializedProperty m_GraphicProperty;
        SerializedProperty m_GroupProperty;
        SerializedProperty m_IsOnProperty;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            m_GraphicProperty = serializedObject.FindProperty("graphic");
            m_GroupProperty = serializedObject.FindProperty("m_Group");
            m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
            m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MToggle toggle = serializedObject.targetObject as MToggle;
            EditorGUILayout.PropertyField(m_GraphicProperty);
            serializedObject.ApplyModifiedProperties();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_IsOnProperty);
            if (EditorGUI.EndChangeCheck())
            {
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(toggle.gameObject.scene);

                MToggleGroup group = m_GroupProperty.objectReferenceValue as MToggleGroup;

                toggle.isOn = m_IsOnProperty.boolValue;

                if (group != null && group.isActiveAndEnabled && toggle.IsActive())
                {
                    if (toggle.isOn || (!group.AnyMTogglesOn() && !group.allowSwitchOff))
                    {
                        toggle.isOn = true;
                        group.NotifyMToggleOn(toggle);
                    }
                }
            }
            EditorGUILayout.PropertyField(m_GroupProperty);
            if (EditorGUI.EndChangeCheck())
            {
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(toggle.gameObject.scene);

                MToggleGroup group = m_GroupProperty.objectReferenceValue as MToggleGroup;
                toggle.group = group;
            }

            EditorGUILayout.Space();
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            // Draw the event notification options
            EditorGUILayout.PropertyField(m_OnValueChangedProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
} 