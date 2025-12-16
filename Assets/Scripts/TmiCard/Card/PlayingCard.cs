#nullable enable
using System;
using TmiCard.CardCharacter;
using TmiCard.CardPlayer;
using TmiCard.CardZone.Base;
using TmiCard.POCO;

namespace TmiCard.Card
{
    [Serializable]
    public abstract class PlayingCard: IPlayingCard
    {
        public CardLocation location = CardLocation.Unknown;
        public CardData Data;
        
        // cards should not hold zones as Zone is a volatile dynamic state.
        // cards should hold owners for access convenience
        public ICardPlayer Owner;
        
        public ICardCharacter Character;

        protected PlayingCard(CardData data, ICardPlayer owner, ICardZone zone)
        {
            Data = data;
            Owner = owner;
            Character = new CardCharacter.CardCharacter(Data.Character);
        }
    }
}