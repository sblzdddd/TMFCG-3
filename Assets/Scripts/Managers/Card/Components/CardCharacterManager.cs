using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Repositories;
using TmiCard.CardCharacter;
using TMPro;
using UnityEngine;
using Utils;
using VContainer;

namespace Managers.Card.Components
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class CardCharacterManager: MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private CancellationTokenSource cts;
        private CardCharacter currentCharacter;
        private string currentImage;
        [SerializeField] [ChildObjectOnly] [NotNull] private TMP_Text characterName;
        [Inject] private CharacterRepository repository;
        
        private static readonly int CharacterTex = Shader.PropertyToID("_Character");
        private static readonly int CharacterX = Shader.PropertyToID("_CharacterX");
        private static readonly int CharacterY = Shader.PropertyToID("_CharacterY");
        private static readonly int CharacterScale = Shader.PropertyToID("_CharacterScale");

        public void Initialize()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(meshRenderer.sharedMaterial);
        }

        public async UniTask SetCharacter((string characterId, int variantIndex) setConfig)
        {
            var characterData = repository.Get(setConfig.characterId);
            if (characterData == null) return;
            currentCharacter = new CardCharacter(characterData, setConfig.variantIndex);
            var loadedSprite = await LoadCharacterImageDedup();
            if (!loadedSprite) return;
            characterName.text = characterData.enName[0];
            meshRenderer.material.SetTexture(CharacterTex, loadedSprite.texture);
            UpdateCharacterTransform(currentCharacter.currentVariant.transform);
        }

        private async UniTask<Sprite> LoadCharacterImageDedup()
        {
            if(currentImage == currentCharacter.currentVariant.image) return null;
            currentImage = currentCharacter.currentVariant.image;
            cts?.Cancel();
            cts = new CancellationTokenSource();
            try
            {
                var loadedSprite = await AssetLoader.LoadCharacterAsync(currentImage, cts.Token);
                return loadedSprite;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load character image: {e.Message}");
            }

            return null;
        }

        public void UpdateCharacterTransform(Vector3 characterTransform)
        {
            meshRenderer.material.SetFloat(CharacterX, characterTransform.x);
            meshRenderer.material.SetFloat(CharacterY, characterTransform.y);
            meshRenderer.material.SetFloat(CharacterScale, characterTransform.z);
        }
    }
}