using Game.Shared.TmiCards.Card;
using Game.Shared.TmiCards.CardZone.Base;

namespace Game.Shared.TmiCards.CardZone
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