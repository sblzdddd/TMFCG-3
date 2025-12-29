using Game.Shared.TmiCards.POCO;
using UnityEngine;

namespace Game.Shared.TmiCards.CardCharacter
{
    public class CardCharacter: ICardCharacter
    {
        public CharacterData Data { get; }
        public int VariantIndex { get; }
        
        public string currentVariant => Data.variants[VariantIndex];
        public Vector3 transform => Data.transform;

        public CardCharacter(CharacterData data, int variantIndex = 0)
        {
            Data = data;
            VariantIndex = variantIndex;
        }
    }
}