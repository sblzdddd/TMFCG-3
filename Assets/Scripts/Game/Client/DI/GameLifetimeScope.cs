using Game.Client.Data.DataDebug;
using Game.Client.Data.Editors;
using Game.Client.Data.Providers;
using Game.Client.Data.Repositories;
using Game.Client.Managers;
using Game.Client.Managers.Card;
using Game.Client.Managers.Card.Components;
using Game.Client.Managers.UI;
using Game.Client.Managers.UI.Panels;
using Game.Client.Managers.UI.Panels.Panels;
using Game.Client.SimpleMystiaUI.Selectables;
using VContainer;
using VContainer.Unity;

namespace Game.Client.DI
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
            builder.Register<PlayerRepository>(Lifetime.Singleton);
            builder.Register<PlayerDataProvider>(Lifetime.Scoped);
            builder.Register<RandomPlayerInfoProvider>(Lifetime.Scoped);
            builder.Register<PlayerDataEditor>(Lifetime.Scoped);
            
            builder.Register<EditorContext>(Lifetime.Scoped);

            // Panels
            builder.RegisterComponentInHierarchy<StartPanel>();
            builder.RegisterComponentInHierarchy<CardCustomPanel>();
            builder.RegisterComponentInHierarchy<SettingsPanel>();
            builder.RegisterComponentInHierarchy<JoinPanel>();
            
            builder.RegisterComponentInHierarchy<CharacterConfigPanel>();
            
            // Components
            builder.RegisterComponentInHierarchy<CharacterGridView>();
            builder.RegisterComponentInHierarchy<CharacterVariantsView>();
            builder.RegisterComponentInHierarchy<CardGridView>();

            builder.RegisterComponentInHierarchy<CustomPreviewCard>();
            builder.RegisterComponentInHierarchy<CardCharacterManager>();
            builder.RegisterComponentInHierarchy<CardManager>();

            builder.RegisterComponentInHierarchy<ProvidersDebug>();
            
            builder.RegisterComponentInHierarchy<AudioManager>();
            builder.RegisterComponentInHierarchy<MSelectable>();
            
            builder.RegisterComponentInHierarchy<GameLauncher>();
        }
    }
}