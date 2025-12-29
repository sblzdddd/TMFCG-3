using Game.Client.Data.Providers;
using Game.Client.Data.Repositories;
using R3;
using UnityEngine;
using VContainer;

namespace Game.Client.Data.Editors
{
    public class DeckDataEditor
    {
        public readonly ReactiveProperty<string> EditingCardId = new();

        [Inject] private readonly DeckRepository deckRepository;
        [Inject] private readonly CharacterProvider characterProvider;
        [Inject] private readonly DeckDataProvider deckDataProvider;

        public void SelectCard(string id)
        {
            if (!deckDataProvider.CardExists(id))
            {
                Debug.LogError($"Card {id} not found");
                return;
            }
            EditingCardId.Value = id;
        }

        public void UpdateCardCharacter(string characterId, int variantIndex)
        {
            if (EditingCardId.Value == null) return;
            if (!deckDataProvider.TryGetCard(EditingCardId.Value, out var card))
            {
                Debug.LogError($"Deck {EditingCardId.Value} not found");
                return;
            }
            if (!characterProvider.TryGet(characterId, out var character))
            {
                Debug.LogError($"Character {EditingCardId.Value} not found");
                return;
            }
            card.characterId = character.characterId;
            card.characterVariantIndex = variantIndex;
        }

        public void SaveConfig()
        {
            deckRepository.Save();
        }
    }
}