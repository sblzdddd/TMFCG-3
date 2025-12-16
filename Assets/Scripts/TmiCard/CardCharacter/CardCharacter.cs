using TmiCard.POCO;

namespace TmiCard.CardCharacter
{
    public class CardCharacter: ICardCharacter
    {
        public CharacterData Data { get; }

        public CardCharacter(CharacterData data)
        {
            Data = data;
        }
    }
}