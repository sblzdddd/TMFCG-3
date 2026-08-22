using System.Collections.Generic;
using Game.Shared.TmiCards.Card;

namespace Game.Domain
{
    public record HandCardItem(string CardId, CardSuit Suit, CardValue Value);

    public class HandCard
    {
        public List<HandCardItem> Cards { get; set; } = new List<HandCardItem>();
        
        public HandCard() { }

        public HandCard(IEnumerable<HandCardItem> cards)
        {
            Cards = new List<HandCardItem>(cards);
        }
    }
}
