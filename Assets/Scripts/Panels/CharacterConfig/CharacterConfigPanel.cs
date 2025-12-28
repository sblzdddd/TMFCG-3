using Cysharp.Threading.Tasks;
using Editors;
using Managers;
using Panels.Base;
using R3;
using SimpleMystiaUI.Sliders;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Panels.CharacterConfig
{
    [RequireComponent(typeof(RectTransform))]
    public class CharacterConfigPanel: BasePanel
    {
        [Header("UI")]
        [SerializeField, ChildObjectOnly, NotNull]
        public Button cancelButton;
        [SerializeField, ChildObjectOnly, NotNull]
        public Button confirmButton;
        [SerializeField, ChildObjectOnly, NotNull]
        private MSlider characterSize;
        [SerializeField, ChildObjectOnly, NotNull]
        private MSlider characterX;
        [SerializeField, ChildObjectOnly, NotNull]
        private MSlider characterY;

        private string oldCharacterId = "Fallback";
        private int oldCharacterVariantIndex;
        
        [Inject] private readonly CharacterGridView characterGridView;
        [Inject] private readonly CharacterEditor characterEditor;
        [Inject] private readonly DeckDataEditor deckDataEditor;
        [Inject] private EditorContext editorContext;

        private readonly CompositeDisposable disposables = new();
        
        public void Start()
        {
            FadePosition = 1;
            // EditTransform => slider value
            characterEditor.EditTransform
                .Subscribe(UpdateSliders)
                .AddTo(disposables);

            // slider value => EditTransform
            characterX.onValueChanged.AddListener(OnSliderChanged);
            characterY.onValueChanged.AddListener(OnSliderChanged);
            characterSize.onValueChanged.AddListener(OnSliderChanged);

            confirmButton.onClick.AddListener(Apply);
            cancelButton.onClick.AddListener(Revert);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void UpdateSliders(Vector3 t)
        {
            characterX.SetValueWithoutNotify(t.x);
            characterY.SetValueWithoutNotify(t.y);
            characterSize.SetValueWithoutNotify(t.z);
        }
        
        private void OnSliderChanged(float _)
        {
            characterEditor.UpdateTransform(new Vector3(
                characterX.value, 
                characterY.value, 
                characterSize.value
            ));
        }
        
        private void Apply()
        {
            characterEditor.SaveConfig();
            deckDataEditor.UpdateCardCharacter(characterEditor.EditingCharacterId.Value, characterEditor.EditingVariantIndex.Value);
            deckDataEditor.SaveConfig();
            Hide();
        }
        
        private void Revert()
        {
            characterEditor.SelectCharacter(oldCharacterId, oldCharacterVariantIndex);
            Hide();
        }

        public void Show(string characterId, int characterVariantIndex, int direction = -1)
        {
            oldCharacterId = characterId;
            oldCharacterVariantIndex = characterVariantIndex;
            characterGridView.RefreshGrid()
                .ContinueWith(async () =>
                {
                    await characterGridView.Select(characterId, characterVariantIndex);
                });
            base.Show(direction);
        }

        public override void Hide(int direction = -1)
        {
            editorContext.CurrentContext.Value = PreviewContext.CardEditor;
            base.Hide(direction);
        }
    }
}