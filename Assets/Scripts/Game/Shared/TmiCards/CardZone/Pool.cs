using Game.Shared.TmiCards.Card;
using Game.Shared.TmiCards.CardZone.Base;

namespace Game.Shared.TmiCards.CardZone
{
    public class Pool: Base.CardZone, ICardZone
    {
        public Pool() : base(CardLocation.Pool)
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