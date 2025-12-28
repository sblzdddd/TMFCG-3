using Providers;
using R3;
using Repositories;
using UnityEngine;
using VContainer;

namespace Editors
{
    public class CharacterEditor: MonoBehaviour
    {
        public readonly ReactiveProperty<string> EditingCharacterId = new();
        public readonly ReactiveProperty<int> EditingVariantIndex = new();
        public readonly ReactiveProperty<Vector3> EditTransform = new();

        [Inject] private CharacterProvider provider;
        [Inject] private CharacterRepository repository;

        public void SelectCharacter(string id, int variantIndex = 0)
        {
            if (!provider.TryGet(id, out var character))
            {
                Debug.LogError($"Character {id} not found");
                return;
            }
            EditingCharacterId.Value = id;
            EditingVariantIndex.Value = variantIndex;
            EditTransform.Value = character.transform;
        }

        public void UpdateTransform(Vector3 editTransform)
        {
            if (EditingCharacterId.Value == null) return;
            if (!provider.TryGet(EditingCharacterId.Value, out var character))
            {
                Debug.LogError($"Character {EditingCharacterId.Value} not found");
                return;
            }

            character.transform = editTransform;
            EditTransform.Value = editTransform;
        }

        public void SaveConfig()
        {
            repository.Save();
        }
    }
}