using System.Collections.Generic;
using System.Linq;
using R3;
using Repositories;
using TmiCard.Card;
using TmiCard.POCO;
using UnityEngine;

namespace Providers
{
    public class DeckDataProvider
    {
        public readonly ReactiveProperty<string> CurrentDeckName = new("Official Deck B");
        private readonly Dictionary<string, DeckData> decksDict;

        public DeckDataProvider(DeckRepository deckRepository)
        {
            decksDict = deckRepository.Decks.Distinct().ToDictionary(d => d.name);
        }

        public DeckData CurrentDeckData
        {
            get
            {
                if(decksDict.TryGetValue(CurrentDeckName.Value, out var deck)) return deck;
                throw new KeyNotFoundException($"Deck {CurrentDeckName.Value} was not found.");
            }
        }

        public List<CardData> Cards => CurrentDeckData.Cards;

        public bool TryGetCard(string id, out CardData card) => CurrentDeckData.TryGetCard(id, out card);

        public bool CardExists(string id) => CurrentDeckData.CardExists(id);

        public void SelectDeck(string deckName)
        {
            if (!decksDict.ContainsKey(deckName))
            {
                Debug.LogError($"Deck {deckName} does not exist.");
                return;
            }
            CurrentDeckName.Value = deckName;   // will trigger reactivity change; no return needed
        }
    }
}