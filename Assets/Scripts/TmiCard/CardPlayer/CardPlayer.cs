using System;
using System.Collections.Generic;
using TmiCard.Card;
using TmiCard.CardZone;

namespace TmiCard.CardPlayer
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