using Panels.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public class JoinPanel: BasePanel
    {
        [SerializeField] [ChildObjectOnly] [NotNull] public Button backButton;

        public void Start()
        {
            FadePosition = 2;
        }
    }
}