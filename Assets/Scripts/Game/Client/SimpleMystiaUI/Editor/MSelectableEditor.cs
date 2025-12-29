using Game.Client.SimpleMystiaUI.Selectables;
using UnityEditor;
using UnityEditor.UI;

namespace SimpleMystiaUI.Editor
{
    [CustomEditor(typeof(MSelectable), true)]
    [CanEditMultipleObjects]
    public class MSelectableEditor : SelectableEditor
    {
        private bool showBaseProperties;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Always show MSelectable properties
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Interactable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isDefaultSelected"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Navigation"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideSounds"));
            if (serializedObject.FindProperty("overrideSounds").boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedSound"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("clickSound"));
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // Show base Selectable properties in a foldout
            showBaseProperties = EditorGUILayout.Foldout(showBaseProperties, "Base Properties", true);
            if (showBaseProperties)
            {
                EditorGUI.indentLevel++;
                base.OnInspectorGUI();
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSelectionChanged"));
            
            serializedObject.ApplyModifiedProperties();
            
        }
    }
} 