using R3;
using Repositories;
using UnityEngine;
using VContainer;

namespace Managers
{
    public class CharacterEditor: MonoBehaviour
    {
        public readonly ReactiveProperty<string> EditingCharacterId = new();
        public readonly ReactiveProperty<int> EditingVariantIndex = new();
        public readonly ReactiveProperty<Vector3> EditTransform = new();

        [Inject] private CharacterRepository repository;

        public void SelectCharacter(string id, int variantIndex = 0)
        {
            var character = repository.Get(id);
            if (character == null)
            {
                Debug.LogError($"Character {id} not found");
                return;
            }
            EditingCharacterId.Value = id;
            EditingVariantIndex.Value = variantIndex;
            EditTransform.Value = character.variants[variantIndex].transform;
        }

        public void UpdateTransform(Vector3 editTransform)
        {
            if (EditingCharacterId.Value == null) return;
            var character = repository.Get(EditingCharacterId.Value);
            if (character == null)
            {
                Debug.LogError($"Character {EditingCharacterId.Value} not found");
                return;
            }

            var variant = character.variants[EditingVariantIndex.Value];
            variant.transform = editTransform;
            EditTransform.Value = editTransform;
        }

        public void SaveConfig()
        {
            repository.Save();
        }
    }
}