using Cysharp.Threading.Tasks;
using Panels.Base;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Panels.CardCustom
{
    [RequireComponent(typeof(RectTransform))]
    public class CardCustomPanel: BasePanel
    {
        [SerializeField] [ChildObjectOnly] [NotNull] public Button backButton;
        [Inject] private readonly CharacterGridView characterGridView;

        public void Start()
        {
            FadePosition = 0;
        }

        public override void Show(int direction = -1)
        {
            characterGridView.RefreshGrid().Forget();
            base.Show(direction);
        }
    }
}