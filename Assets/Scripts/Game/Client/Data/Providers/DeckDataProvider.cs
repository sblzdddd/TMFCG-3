using System.Collections.Generic;
using System.Linq;
using Game.Client.Data.Repositories;
using Game.Shared.TmiCards.POCO;
using R3;
using UnityEngine;

namespace Game.Client.Data.Providers
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

        public List<CardData> Cards => CurrentDeckData.cards;

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