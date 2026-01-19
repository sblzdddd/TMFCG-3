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

            // Panel Manager
            builder.RegisterComponentInHierarchy<PanelManager>().AsImplementedInterfaces().AsSelf();
            
            // Components
            builder.RegisterComponentInHierarchy<CustomPreviewCard>(); // Might be part of panel now? StartPanel used it? No CardCustomPanel used it. If CardCustomPanel is prefab, is this part of it? 
            // CardCustomPanel: [Inject] private readonly CustomPreviewCard previewCard; 
            // If CustomPreviewCard is separate scene object, we keep this. If it's child of prefab, we remove.
            // Assuming it's separate for now as user didn't mention it.
            
            builder.RegisterComponentInHierarchy<CardCharacterManager>();
            builder.RegisterComponentInHierarchy<CardManager>();

            builder.RegisterComponentInHierarchy<ProvidersDebug>();
            
            builder.RegisterComponentInHierarchy<AudioManager>();
            builder.RegisterComponentInHierarchy<MSelectable>();
            
            builder.RegisterComponentInHierarchy<GameLauncher>();
        }
    }
}