using System;
using TmiCard.Card;

namespace TmiCard.POCO
{
    [Serializable]
    public abstract class CardData
    {
        public string id;
        public CardSuit suit;
        public CardValue value;
        public CharacterData Character;
    }
}