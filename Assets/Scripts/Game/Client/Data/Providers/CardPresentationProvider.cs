using Game.Client.Dto;

namespace Game.Client.Data.Providers
{
    public sealed class CardPresentationProvider
    {
        private readonly DeckDataProvider deckProvider;
        private readonly CharacterProvider characterProvider;

        public CardPresentationProvider(
            DeckDataProvider deckProvider,
            CharacterProvider characterProvider)
        {
            this.deckProvider = deckProvider;
            this.characterProvider = characterProvider;
        }

        public CardPresentation Get(string cardId)
        {
            if (!deckProvider.TryGetCard(cardId, out var card))
                return null;

            if (!characterProvider.TryGet(card.characterId, out var character))
                return null;

            return new CardPresentation(
                card,
                character,
                card.characterVariantIndex);
        }
        
        public CardPresentation GetPreview(
            string cardId,
            string overrideCharacterId,
            int overrideVariantIndex)
        {
            if (!deckProvider.TryGetCard(cardId, out var card))
                return null;

            var characterId = string.IsNullOrEmpty(overrideCharacterId)
                ? card.characterId
                : overrideCharacterId;

            var variantIndex = overrideVariantIndex >= 0
                ? overrideVariantIndex
                : card.characterVariantIndex;

            if (!characterProvider.TryGet(characterId, out var character))
                return null;

            return new CardPresentation(card, character, variantIndex);
        }
    }

}