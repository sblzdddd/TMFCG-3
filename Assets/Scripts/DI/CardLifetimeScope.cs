using Managers.Card;
using Repositories;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class CardLifetimeScope: LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<CharacterRepository>(Lifetime.Singleton);

            // TODO: Implement Card Pool to Avoid This
            builder.RegisterComponentInHierarchy<CardCharacterManager>();
        }
    }
}