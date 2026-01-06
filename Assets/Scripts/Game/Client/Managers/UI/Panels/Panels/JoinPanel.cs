using Game.Client.Managers.UI.Panels.Base;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Client.Managers.UI.Panels.Panels
{
    public class JoinPanel: BasePanel
    {
        [SerializeField, ChildObjectOnly, NotNull] public Button backButton;
        [SerializeField, ChildObjectOnly, NotNull] public Button joinButton;
        [SerializeField, ChildObjectOnly, NotNull] public Button hostButton;
        
        [Inject] private readonly GameLauncher launcher;

        public void Start()
        {
            FadePosition = 2;
            hostButton.onClick.AddListener(() => launcher.StartHost());
            joinButton.onClick.AddListener(() => launcher.StartClient("127.0.0.1"));
        }
    }
}