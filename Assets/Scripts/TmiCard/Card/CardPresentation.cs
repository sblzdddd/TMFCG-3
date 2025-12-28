using TmiCard.POCO;

namespace TmiCard.Card
{
    // dto for displaying a card
    public class CardPresentation
    {
        public readonly CardData Card;
        public readonly CharacterData Character;
        public readonly int VariantIndex;

        public CardPresentation(
            CardData card,
            CharacterData character,
            int variantIndex)
        {
            Card = card;
            Character = character;
            VariantIndex = variantIndex;
        }
    }
}