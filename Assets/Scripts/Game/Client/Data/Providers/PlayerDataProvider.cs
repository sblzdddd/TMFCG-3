using Game.Client.Data.Repositories;
using Game.Shared.TmiCards.POCO;
using R3;

namespace Game.Client.Data.Providers
{
    public class PlayerDataProvider
    {
        private readonly PlayerRepository repository;
        private readonly RandomPlayerInfoProvider random;

        public PlayerDataProvider(
            RandomPlayerInfoProvider random,
            PlayerRepository repository)
        {
            this.random = random;
            this.repository = repository;

            if (repository.PlayerInfo.Value != null) return;
            
            var avatar = random.GetRandomCharacter();
            repository.PlayerInfo.Value = new PlayerInfoData
            {
                name = random.GetRandomPlayerName(),
                avatarId = avatar.Item1,
                avatarVariantIndex = avatar.Item2
            };
            repository.Save();
        }

        public ReadOnlyReactiveProperty<PlayerInfoData> PlayerInfo =>
            repository.PlayerInfo;
    }

}