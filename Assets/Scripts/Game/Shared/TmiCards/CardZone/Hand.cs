using System.Collections.Generic;
using Game.Shared.TmiCards.Card;
using Game.Shared.TmiCards.CardZone.Base;

namespace Game.Shared.TmiCards.CardZone
{
    public class Hand: Base.CardZone, ICardZone
    {
        public CardPlayer.CardPlayer Player;
        public List<PlayingCard> Cards;

        public Hand(CardPlayer.CardPlayer player) : base(CardLocation.Hand)
        {
            Player = player;
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