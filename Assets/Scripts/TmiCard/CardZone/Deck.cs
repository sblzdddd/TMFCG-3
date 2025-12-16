using TmiCard.Card;
using TmiCard.CardZone.Base;

namespace TmiCard.CardZone
{
    public class Deck: Base.CardZone
    {
        public Deck() : base(CardLocation.Deck)
        {
        }

        public new void Add(PlayingCard card)
        {
            base.Add(card);
        }

        public new void Remove(PlayingCard card)
        {
            base.Remove(card);
        }
    }
}