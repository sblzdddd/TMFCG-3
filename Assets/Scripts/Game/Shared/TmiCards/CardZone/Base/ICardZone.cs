using Game.Shared.TmiCards.Card;

namespace Game.Shared.TmiCards.CardZone.Base
{
    public interface ICardZone
    {
        void Add(PlayingCard card);
        void Remove(PlayingCard card);
        PlayingCard Draw();
        PlayingCard Get(string cardId);
    }
}