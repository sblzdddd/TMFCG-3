using System;
using TmiCard.POCO;

namespace TmiCard.CardSet
{
    [Serializable]
    public abstract class CardSet: ICardSet
    {
        public CardSetData Data;
    }
}