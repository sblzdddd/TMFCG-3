using Game.Client.Data.Editors;
using Game.Client.Data.Providers;
using Game.Client.Managers.Card;
using Game.Client.Managers.UI.Panels.Base;
using Game.Client.Managers.UI.Panels.CharacterConfig;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Client.Managers.UI.Panels.CardCustom
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
        [Inject] private readonly CharacterConfigPanel characterConfigPanel;
        [Inject] private readonly CustomPreviewCard previewCard;
        [Inject] private EditorContext editorContext;
        
        private readonly CompositeDisposable disposables = new();

        public void Start()
        {
            FadePosition = 0;
            
            backButton.onClick.AddListener(deckDataEditor.SaveConfig);
            saveButton?.onClick.AddListener(deckDataEditor.SaveConfig);
            selectCharacterButton.onClick.AddListener(() =>
            {
                editorContext.CurrentContext.Value = PreviewContext.CharacterEditor;
                deckDataProvider.TryGetCard(deckDataEditor.EditingCardId.Value, out var cardData);
                characterConfigPanel.Show(cardData.characterId, cardData.characterVariantIndex);
                SetInteractability(false);
            });
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        public override void Show(int direction = -1)
        {
            previewCard.Show();
            base.Show(direction);
        }
        
        public override void Hide(int direction = -1)
        {
            previewCard.Hide();
            base.Hide(direction);
        }
    }
}