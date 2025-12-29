#nullable enable
using System;
using Game.Shared.TmiCards.CardPlayer;
using Game.Shared.TmiCards.CardZone.Base;
using Game.Shared.TmiCards.POCO;

namespace Game.Shared.TmiCards.Card
{
    [Serializable]
    public abstract class PlayingCard: IPlayingCard
    {
        public CardLocation location = CardLocation.Unknown;
        public CardData Data;
        
        // cards should not hold zones as Zone is a volatile dynamic state.
        // cards should hold owners for access convenience
        public ICardPlayer Owner;

        protected PlayingCard(CardData data, ICardPlayer owner, ICardZone zone)
        {
            Data = data;
            Owner = owner;
        }
    }
}