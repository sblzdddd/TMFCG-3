using Game.Client.Managers.UI.Panels.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.Managers.UI.Panels.Panels
{
    public class StartPanel: BasePanel
    {
        public Button startButton;
        public Button customButton;
        public Button settingsButton;
        public Button exitButton;
        
        private void Start()
        {
            startButton.onClick.AddListener(() => Manager.OpenPanel(PanelType.Join));
            customButton.onClick.AddListener(() => Manager.OpenPanel(PanelType.CardCustom));
            settingsButton.onClick.AddListener(() => Manager.OpenPanel(PanelType.Settings));
            exitButton.onClick.AddListener(Application.Quit);
        }

        public override void OnOpen()
        {
            base.OnOpen();
            MoCo.FadeIn(Cg);
        }
    }
}
