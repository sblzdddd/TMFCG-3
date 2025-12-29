using Game.Shared.TmiCards.Card;
using Game.Shared.TmiCards.CardPlayer;
using Game.Shared.TmiCards.CardZone.Base;
using Game.Shared.TmiCards.POCO;
using JetBrains.Annotations;

namespace Game.Shared.TmiCards.SkillCard
{
    public class SkillCard: PlayingCard
    {
        public SkillCard([NotNull] CardData data, [NotNull] ICardPlayer owner, [NotNull] ICardZone zone) : base(data, owner, zone)
        {
        }
    }
}