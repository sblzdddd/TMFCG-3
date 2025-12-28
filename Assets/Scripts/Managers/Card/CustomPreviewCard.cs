using Cysharp.Threading.Tasks;
using Editors;
using Extensions;
using LitMotion;
using Providers;
using R3;
using UnityEngine;
using VContainer;

namespace Managers.Card
{
    [RequireComponent(typeof(CardManager))]
    public class CustomPreviewCard : MonoBehaviour
    {
        private readonly CompositeDisposable disposables = new();
        
        [Inject] private readonly CharacterEditor characterEditor;
        [Inject] private readonly DeckDataEditor deckDataEditor;
        [Inject] private readonly CardPresentationProvider provider;
        [Inject] private EditorContext editorContext;
        private CardManager cardManager;

        private void Start()
        {
            cardManager = GetComponent<CardManager>();
            cardManager.Start();
            
            // Observe Card Changes / Character Changes
            editorContext.CurrentContext
                .Select(context =>
                {
                    if (context == PreviewContext.CharacterEditor)
                    {
                        return Observable.CombineLatest(
                            deckDataEditor.EditingCardId.Where(id => id != null),
                            characterEditor.EditingCharacterId,
                            characterEditor.EditingVariantIndex,
                            (cardId, characterId, variantIndex) =>
                                provider.GetPreview(cardId, characterId, variantIndex)
                        );
                    }

                    // CardEditor
                    return deckDataEditor.EditingCardId
                        .Where(id => id != null)
                        .Select(cardId => provider.GetPreview(cardId, null, -1));
                })
                .Switch()
                .Where(p => p != null)
                .Subscribe(cardManager.SetCard)
                .AddTo(disposables);


            // When Transform changes, update the material properties only
            characterEditor.EditTransform.Subscribe(t =>
            {
                cardManager.CharacterManager.UpdateCharacterTransform(t);
            }).AddTo(disposables);
        }

        public void Show()
        {
            LMotion.Create(-12f, 4f, 0.25f)
                .WithEase(Ease.OutQuad)
                .BindToPosX(transform);
        }

        public void Hide()
        {
            LMotion.Create(4f, -12f, 0.25f)
                .WithEase(Ease.OutQuad)
                .BindToPosX(transform);
        }
        
        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}