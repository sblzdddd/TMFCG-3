using Managers.Card;

namespace Interfaces.Card
{
    public interface ICardPool
    {
        CardManager Create();
        void Dispose(CardManager card);
    }

}