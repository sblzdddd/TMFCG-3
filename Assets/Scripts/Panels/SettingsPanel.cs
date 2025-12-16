using Panels.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public class SettingsPanel: BasePanel
    {
        [SerializeField] [ChildObjectOnly] [NotNull] public Button backButton;

        public void Start()
        {
            FadePosition = 1;
        }
    }
}