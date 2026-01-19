using Game.Client.Data.Editors;
using Game.Client.Data.Providers;
using Game.Client.Managers.Card;
using Game.Client.Managers.UI.Panels.Base;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Cysharp.Threading.Tasks;

namespace Game.Client.Managers.UI.Panels.Panels
{
    [RequireComponent(typeof(RectTransform))]
    public class CardCustomPanel: BasePanel
    {
        [Header("UI")]
        [SerializeField, ChildObjectOnly, NotNull]
        public Button backButton;
        [SerializeField, ChildObjectOnly]
        public Button saveButton;

        [SerializeField, ChildObjectOnly, NotNull]
        private Button selectCharacterButton;
        
        [Inject] private readonly DeckDataEditor deckDataEditor;
        [Inject] private readonly DeckDataProvider deckDataProvider;
        // [Inject] private readonly CharacterConfigPanel characterConfigPanel; // Removed
        [Inject] private readonly CustomPreviewCard previewCard;
        [Inject] private EditorContext editorContext;
        
        private readonly CompositeDisposable disposables = new();

        public void Start()
        {
            backButton.onClick.AddListener(deckDataEditor.SaveConfig);
            backButton.onClick.AddListener(() => Manager.CloseCurrentPanel()); // Added Close
            saveButton?.onClick.AddListener(deckDataEditor.SaveConfig);
            selectCharacterButton.onClick.AddListener(() =>
            {
                editorContext.CurrentContext.Value = PreviewContext.CharacterEditor;
                deckDataProvider.TryGetCard(deckDataEditor.EditingCardId.Value, out var cardData);
                
                var panel = Manager.OpenPanel(PanelType.CharacterConfig);
                if (panel is CharacterConfigPanel config)
                {
                    config.Setup(cardData.characterId, cardData.characterVariantIndex);
                }
            });
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }


        public override void OnOpen()
        {
            previewCard.Show();
            base.OnOpen();
            MoCo.FadeIn(Cg);
        }

        public override UniTask OnClose()
        {
            previewCard.Hide();
            return base.OnClose();
        }
    }
}