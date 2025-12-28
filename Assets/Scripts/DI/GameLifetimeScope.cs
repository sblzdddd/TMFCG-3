using DataDebug;
using Editors;
using Managers;
using Managers.Card;
using Managers.Card.Components;
using Panels;
using Panels.CardCustom;
using Panels.CharacterConfig;
using Panels.Navigator;
using Providers;
using Repositories;
using TmiCard.POCO;
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
            builder.Register<CharacterProvider>(Lifetime.Scoped);
            builder.Register<CharacterEditor>(Lifetime.Scoped);
            builder.Register<DeckRepository>(Lifetime.Singleton);
            builder.Register<DeckDataProvider>(Lifetime.Scoped);
            builder.Register<DeckDataEditor>(Lifetime.Scoped);
            builder.Register<CardPresentationProvider>(Lifetime.Scoped);
            
            builder.Register<EditorContext>(Lifetime.Scoped);


            // Panels
            builder.RegisterComponentInHierarchy<StartPanel>();
            builder.RegisterComponentInHierarchy<CardCustomPanel>();
            builder.RegisterComponentInHierarchy<SettingsPanel>();
            builder.RegisterComponentInHierarchy<JoinPanel>();
            
            builder.RegisterComponentInHierarchy<CharacterConfigPanel>();
            
            // Components
            builder.RegisterComponentInHierarchy<CharacterGridView>();
            builder.RegisterComponentInHierarchy<VariantListView>();
            builder.RegisterComponentInHierarchy<CardGridView>();

            builder.RegisterComponentInHierarchy<CustomPreviewCard>();
            builder.RegisterComponentInHierarchy<CardCharacterManager>();
            builder.RegisterComponentInHierarchy<CardManager>();

            builder.RegisterComponentInHierarchy<ProvidersDebug>();
        }
    }
}