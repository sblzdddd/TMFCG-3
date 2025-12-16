using JetBrains.Annotations;
using TmiCard.Card;
using TmiCard.CardPlayer;
using TmiCard.CardZone.Base;
using TmiCard.POCO;

namespace TmiCard.SkillCard
{
    public class SkillCard: PlayingCard
    {
        public SkillCard([NotNull] CardData data, [NotNull] ICardPlayer owner, [NotNull] ICardZone zone) : base(data, owner, zone)
        {
        }
    }
}