using System.Collections.Generic;
using Game.Client.Data.Providers;
using Game.Shared.TmiCards.POCO;
using UnityEngine;
using VContainer;

namespace Game.Client.Data.DataDebug
{
    public class ProvidersDebug: MonoBehaviour
    {
        [Inject] private readonly DeckDataProvider deckDataProvider;
        [Inject] private readonly CharacterProvider characterProvider;
        [Inject] private readonly PlayerDataProvider playerDataProvider;
        [SerializeField, ScrollableItems]
        private List<CardData> cards;
        [SerializeField, ScrollableItems]
        private List<CharacterData> characters;
        [SerializeField, ScrollableItems]
        private PlayerInfoData playerInfo;

        private void OnGUI()
        {
            cards = deckDataProvider.Cards;
            characters = characterProvider.Characters;
            playerInfo = playerDataProvider.PlayerInfo.CurrentValue;
        }
    }
}