using UnityEngine;

namespace Managers.Card
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class CharacterManager: MonoBehaviour
    {
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(meshRenderer.sharedMaterial);
        }
    }
}