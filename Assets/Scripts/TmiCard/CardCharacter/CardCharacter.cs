using TmiCard.POCO;

namespace TmiCard.CardCharacter
{
    public class CardCharacter: ICardCharacter
    {
        public CharacterData Data { get; }
        public int VariantIndex { get; }
        
        public CharacterVariant currentVariant => Data.variants[VariantIndex];

        public CardCharacter(CharacterData data, int variantIndex = 0)
        {
            Data = data;
            VariantIndex = variantIndex;
        }
    }
}