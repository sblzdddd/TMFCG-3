using System;
using System.Collections.Generic;
using Game.Shared.TmiCards.Card;
using Game.Shared.TmiCards.CardZone;

namespace Game.Shared.TmiCards.CardPlayer
{
    [Serializable]
    public class CardPlayer: ICardPlayer
    {
        public List<PlayingCard> Cards = new();
        private Hand PlayerHand { get; }

        public CardPlayer(Hand playerHand)
        {
            this.PlayerHand = new Hand(this);
        }
    }
}