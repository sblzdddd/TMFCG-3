using Managers;
using Panels;
using Panels.CardCustom;
using Panels.Navigator;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class UiLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<PanelNavigator>();
            builder.RegisterComponentInHierarchy<StartPanel>();
            builder.RegisterComponentInHierarchy<CardCustomPanel>();
            builder.RegisterComponentInHierarchy<SettingsPanel>();
            builder.RegisterComponentInHierarchy<JoinPanel>();
            
            builder.RegisterComponentInHierarchy<CharacterConfigService>();
            builder.RegisterComponentInHierarchy<CharacterGridView>();
        }
    }
}