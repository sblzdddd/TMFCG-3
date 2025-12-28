using Panels.CardCustom;
using Panels.CharacterConfig;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;

namespace Panels.Navigator
{
    public class PanelNavigator : IStartable
    {
        private readonly StartPanel startPanel;
        private readonly JoinPanel joinPanel;
        private readonly CardCustomPanel customPanel;
        private readonly SettingsPanel settingsPanel;
        private readonly CharacterConfigPanel characterConfigPanel;

        public PanelNavigator(StartPanel startPanel, JoinPanel joinPanel, CardCustomPanel customPanel, SettingsPanel settingsPanel, CharacterConfigPanel characterConfigPanel)
        {
            this.startPanel = startPanel;
            this.joinPanel = joinPanel;
            this.customPanel = customPanel;
            this.settingsPanel = settingsPanel;
            this.characterConfigPanel = characterConfigPanel;
        }

        void IStartable.Start()
        {
            startPanel.startButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                startPanel.Hide(0); joinPanel.Show();
            });
            startPanel.customButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                startPanel.Hide(2); customPanel.Show();
            });
            startPanel.settingsButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                startPanel.Hide(3); settingsPanel.Show();
            });
            
            startPanel.exitButton.GetComponent<Button>().onClick.AddListener(Application.Quit);
            
            joinPanel.backButton.onClick.AddListener(() =>
            {
                joinPanel.Hide(); startPanel.Show(0);
            });
            
            customPanel.backButton.onClick.AddListener(() =>
            {
                customPanel.Hide(); startPanel.Show(2);
            });
            
            settingsPanel.backButton.onClick.AddListener(() =>
            {
                settingsPanel.Hide(); startPanel.Show(3);
            });

            characterConfigPanel.cancelButton.onClick.AddListener(() =>
            {
                customPanel.SetInteractability(true);
            });
            characterConfigPanel.confirmButton.onClick.AddListener(() =>
            {
                customPanel.SetInteractability(true);
            });
        }
    }    
}