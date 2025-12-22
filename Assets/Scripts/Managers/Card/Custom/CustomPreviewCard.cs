namespace Managers.Card.Custom
{
    using UnityEngine;
    using VContainer;
    using R3;
    using TmiCard.CardCharacter;
    using Cysharp.Threading.Tasks;

    public class CustomPreviewCard : MonoBehaviour
    {
        [Inject] private readonly CharacterEditor characterEditor;
        [SerializeField] private CardCharacterManager cardCharacterManager;
        private readonly CompositeDisposable disposables = new();
        
        [Inject] private CharacterEditor editor;

        private CardCharacter currentCardCharacter;

        private void Start()
        {
            // When Character or Variant index changes, reload the image
            characterEditor.EditingCharacterId.Where(c => c != null)
                .CombineLatest(characterEditor.EditingVariantIndex, 
                    (characterId, index) => (characterId, index)
                )
                .Subscribe(pair =>
                    cardCharacterManager.SetCharacter(pair).Forget()
                )
                .AddTo(disposables);

            // When Transform changes, update the material properties only
            characterEditor.EditTransform.Subscribe(t =>
            {
                if (currentCardCharacter == null) return;
                cardCharacterManager.UpdateCharacterTransform(t);
            }).AddTo(disposables);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}