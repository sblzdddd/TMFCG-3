using Cysharp.Threading.Tasks;
using Extensions;
using LitMotion;
using R3;
using UnityEngine;
using VContainer;

namespace Managers.Card
{
    [RequireComponent(typeof(CardManager))]
    public class CustomPreviewCard : MonoBehaviour
    {
        private readonly CompositeDisposable disposables = new();
        
        [Inject] private readonly CharacterEditor editor;
        private CardManager cardManager;

        private void Start()
        {
            cardManager = GetComponent<CardManager>();
            cardManager.Initialize();
            // When Character or Variant index changes, reload the image
            editor.EditingCharacterId.Where(c => c != null)
                .CombineLatest(editor.EditingVariantIndex, 
                    (characterId, index) => (characterId, index)
                )
                .Subscribe(pair =>
                    cardManager.CharacterManager.SetCharacter(pair).Forget()
                )
                .AddTo(disposables);

            // When Transform changes, update the material properties only
            editor.EditTransform.Subscribe(t =>
            {
                cardManager.CharacterManager.UpdateCharacterTransform(t);
            }).AddTo(disposables);
        }

        public void Show()
        {
            LMotion.Create(11f, 4f, 0.25f)
                .WithEase(Ease.OutQuad)
                .BindToPosX(transform);
        }

        public void Hide()
        {
            LMotion.Create(4f, 11f, 0.25f)
                .WithEase(Ease.OutQuad)
                .BindToPosX(transform);
        }
        
        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}