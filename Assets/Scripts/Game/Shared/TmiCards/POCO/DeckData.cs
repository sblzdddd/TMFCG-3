using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Shared.TmiCards.POCO
{
    [Serializable]
    public class DeckData
    {
        public string name;
        public string author;
        public string description;
        public string version;
        public string apiVersion;
        public List<CardData> cards;
        
        [NonSerialized]
        private Dictionary<string, CardData> cardById; // for later query
        
        protected void BuildIndex()
        {
            cardById = new Dictionary<string, CardData>();

            foreach (var card in cards.Where(card => !cardById.TryAdd(card.id, card)))
            {
                throw new InvalidOperationException($"Duplicate cardId: {card.id}");
            }
        }

        public bool TryGetCard(string cardId, out CardData card)
        {
            if (cardById == null)
                BuildIndex();

            return cardById!.TryGetValue(cardId, out card);
        }

        public bool CardExists(string cardId)
        {
            if (cardById == null)
                BuildIndex();

            return cardById!.ContainsKey(cardId);
        }
    }
}