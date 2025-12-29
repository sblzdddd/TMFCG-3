using System;
using Game.Shared.TmiCards.Card;

namespace Game.Shared.TmiCards.POCO
{
    [Serializable]
    public class CardData
    {
        public string id = "FALLBACK";
        public CardSuit suit = CardSuit.Clubs;
        public CardValue value = CardValue.Ace;
        public string characterId = "FALLBACK";
        public int characterVariantIndex;
    }
}