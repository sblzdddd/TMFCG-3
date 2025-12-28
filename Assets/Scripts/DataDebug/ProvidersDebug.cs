using System.Collections.Generic;
using Providers;
using TmiCard.POCO;
using UnityEngine;
using VContainer;

namespace DataDebug
{
    public class ProvidersDebug: MonoBehaviour
    {
        [Inject] private readonly DeckDataProvider deckDataProvider;
        [Inject] private readonly CharacterProvider characterProvider;
        [SerializeField, ScrollableItems]
        private List<CardData> cards;
        [SerializeField, ScrollableItems]
        private List<CharacterData> characters;

        private void OnGUI()
        {
            cards = deckDataProvider.Cards;
            characters = characterProvider.Characters;
        }
    }
}