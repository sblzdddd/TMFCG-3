using Game.Client.Managers.UI.Panels.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.Managers.UI.Panels.Panels
{
    public class SettingsPanel: BasePanel
    {
        [SerializeField, ChildObjectOnly, NotNull] public Button backButton;

        public void Start()
        {
            FadePosition = 1;
        }
    }
}