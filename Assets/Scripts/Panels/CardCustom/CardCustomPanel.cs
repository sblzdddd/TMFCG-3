using Cysharp.Threading.Tasks;
using Managers;
using Panels.Base;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using R3;
using Repositories;
using SimpleMystiaUI.Sliders;

namespace Panels.CardCustom
{
    [RequireComponent(typeof(RectTransform))]
    public class CardCustomPanel: BasePanel
    {
        [SerializeField] [ChildObjectOnly] [NotNull] public Button backButton;
        [SerializeField] [ChildObjectOnly] [NotNull] public MSlider characterSize;
        [SerializeField] [ChildObjectOnly] [NotNull] public MSlider characterX;
        [SerializeField] [ChildObjectOnly] [NotNull] public MSlider characterY;
        [Inject] private readonly CharacterGridView characterGridView;
        [Inject] private readonly CharacterEditor characterEditor;
        [Inject] private readonly Managers.Card.Custom.CustomPreviewCard previewCard;
        private readonly CompositeDisposable disposables = new();

        public void Start()
        {
            FadePosition = 0;
            
            // Bind sliders to service properties
            characterEditor.EditTransform.Subscribe(t =>
            {
                characterX.SetValueWithoutNotify(t.x);
                characterY.SetValueWithoutNotify(t.y);
                characterSize.SetValueWithoutNotify(t.z);
            }).AddTo(disposables);

            // Bind slider changes to service update
            characterX.onValueChanged.AddListener(val => UpdateService());
            characterY.onValueChanged.AddListener(val => UpdateService());
            characterSize.onValueChanged.AddListener(val => UpdateService());
            
            backButton.onClick.AddListener(() => characterEditor.SaveConfig());
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void UpdateService()
        {
            var editTransform = new Vector3(characterX.value, characterY.value, characterSize.value);
            characterEditor.UpdateTransform(editTransform);
        }

        public override void Show(int direction = -1)
        {
            characterGridView.RefreshGrid().Forget();
            base.Show(direction);
        }
    }
}