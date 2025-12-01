using SimpleMystiaUI.Buttons;
using UnityEditor;

namespace SimpleMystiaUI.Editor
{
    [CustomEditor(typeof(MButtonBase), true)]
    [CanEditMultipleObjects]
    public class MButtonBaseEditor : MSelectableEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onClick"));
            serializedObject.ApplyModifiedProperties();
        }
    }
} 