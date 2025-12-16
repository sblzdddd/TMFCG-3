using TmiCard.Card;

namespace TmiCard.CardZone.Base
{
    public interface ICardZone
    {
        void Add(PlayingCard card);
        void Remove(PlayingCard card);
        PlayingCard Draw();
        PlayingCard Get(string cardId);
    }
}