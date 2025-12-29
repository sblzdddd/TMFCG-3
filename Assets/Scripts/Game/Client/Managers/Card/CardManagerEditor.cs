using UnityEditor;
using UnityEngine;

namespace Game.Client.Managers.Card
{
    [CustomEditor (typeof(CardManager))]
    public class CardManagerEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            var castedTarget = target as CardManager;
            if (castedTarget == null) return;
            castedTarget.GetComponent<BoxCollider>().hideFlags = HideFlags.HideInInspector;
            castedTarget.GetComponent<MeshFilter>().hideFlags = HideFlags.HideInInspector;
        }

        private void Reset(){
            var castedTarget = target as CardManager;
            if (castedTarget == null) return;
            castedTarget.GetComponent<BoxCollider>().hideFlags = HideFlags.HideInInspector;
            castedTarget.GetComponent<MeshFilter>().hideFlags = HideFlags.HideInInspector;
        }
    }
}