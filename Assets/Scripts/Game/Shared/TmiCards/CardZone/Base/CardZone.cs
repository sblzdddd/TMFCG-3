using System.Collections.Generic;
using Game.Shared.TmiCards.Card;

namespace Game.Shared.TmiCards.CardZone.Base
{
    public abstract class CardZone : ICardZone
    {
        private List<PlayingCard> Cards { get; } = new();
        private CardLocation Location { get; }

        protected CardZone(CardLocation cardLocation)
        {
            Location = cardLocation;
        }

        public void Add(PlayingCard card)
        {
            card.location = Location;
            Cards.Add(card);
        }

        public void Remove(PlayingCard card)
        {
            card.location = CardLocation.Unknown;
            Cards.Remove(card);
        }

        public PlayingCard Get(string cardId)
        {
            return Cards.Find(card => card.Data.id == cardId);
        }
        
        public PlayingCard Draw()
        {
            var top = Cards[^1];
            Cards.RemoveAt(Cards.Count - 1);
            return top;
        }
    }
}