using Cysharp.Threading.Tasks;
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
            backButton.onClick.AddListener(() => Manager.CloseCurrentPanel().Forget());
        }

        public override void OnOpen()
        {
            base.OnOpen();
            MoCo.FadeIn(Cg);
        }
    }
}