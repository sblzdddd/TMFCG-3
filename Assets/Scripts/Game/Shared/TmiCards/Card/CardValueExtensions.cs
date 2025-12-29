namespace Game.Shared.TmiCards.Card
{
    public static class CardValueExtensions
    {
        public static string ToDisplayString(this CardValue value)
        {
            return value switch
            {
                CardValue.Ace   => "A",
                CardValue.Two   => "2",
                CardValue.Three => "3",
                CardValue.Four  => "4",
                CardValue.Five  => "5",
                CardValue.Six   => "6",
                CardValue.Seven => "7",
                CardValue.Eight => "8",
                CardValue.Nine  => "9",
                CardValue.Ten   => "10",
                CardValue.Jack  => "J",
                CardValue.Queen => "Q",
                CardValue.King  => "K",
                _ => value.ToString()
            };
        }
    }
}