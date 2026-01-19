using Cysharp.Threading.Tasks;
using Game.Client.Data.Editors;
using Game.Client.Managers.UI.Panels.Base;
using Game.Client.SimpleMystiaUI.Sliders;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Client.Managers.UI.Panels.Panels
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
        
        [SerializeField, ChildObjectOnly, NotNull]
        private CharacterGridView characterGridView;
        [SerializeField, ChildObjectOnly, NotNull]
        private CharacterVariantsView characterVariantView;
        [Inject] private readonly CharacterEditor characterEditor;
        [Inject] private readonly DeckDataEditor deckDataEditor;
        [Inject] private EditorContext editorContext;

        private readonly CompositeDisposable disposables = new();
        
        public void Start()
        {
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
            
            characterVariantView.onCharacterChanged.AddListener(data =>
            {
                characterEditor.SelectCharacter(data.Item1, data.Item2);
            });
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
            Manager.CloseCurrentPanel();
        }
        
        private void Revert()
        {
            characterEditor.SelectCharacter(oldCharacterId, oldCharacterVariantIndex);
            Manager.CloseCurrentPanel();
        }

        public void Setup(string characterId, int characterVariantIndex)
        {
            oldCharacterId = characterId;
            oldCharacterVariantIndex = characterVariantIndex;
            characterGridView.RefreshGrid()
                .ContinueWith(async () =>
                {
                    await characterGridView.Select(characterId, characterVariantIndex);
                });
            // base.Show() removed, handled by PanelManager calling OnOpen
        }


        public override void OnOpen()
        {
            base.OnOpen();
            MoCo.FadeIn(Cg);
        }

        public override UniTask OnClose()
        {
            editorContext.CurrentContext.Value = PreviewContext.CardEditor;
            return base.OnClose();
        }
    }
}