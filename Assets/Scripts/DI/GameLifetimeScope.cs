using Managers;
using Managers.Card;
using Managers.Card.Components;
using Panels;
using Panels.CardCustom;
using Panels.Navigator;
using Repositories;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class GameLifetimeScope: LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Entry Points
            builder.RegisterEntryPoint<PanelNavigator>();
            
            // Services
            builder.Register<CharacterRepository>(Lifetime.Singleton);
            builder.Register<CharacterEditor>(Lifetime.Scoped);

            // Panels
            builder.RegisterComponentInHierarchy<StartPanel>();
            builder.RegisterComponentInHierarchy<CardCustomPanel>();
            builder.RegisterComponentInHierarchy<SettingsPanel>();
            builder.RegisterComponentInHierarchy<JoinPanel>();
            
            // Components
            builder.RegisterComponentInHierarchy<CharacterGridView>();
            builder.RegisterComponentInHierarchy<VariantListView>();

            builder.RegisterComponentInHierarchy<CustomPreviewCard>();
            builder.RegisterComponentInHierarchy<CardCharacterManager>();
        }
    }
}