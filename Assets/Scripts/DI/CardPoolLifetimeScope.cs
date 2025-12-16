using Managers;
using Panels;
using Panels.CardCustom;
using Panels.Navigator;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class CardPoolLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CharacterConfigService>();
        }
    }
}