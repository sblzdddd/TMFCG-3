using System;
using TmiCard.POCO;

namespace TmiCard.Deck
{
    [Serializable]
    public abstract class Deck: IDeck
    {
        public DeckData Data;
    }
}