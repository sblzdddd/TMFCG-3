using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Plugins.CurvedTextMeshPro.Scripts;
using TmiCard.CardCharacter;
using TmiCard.POCO;
using TMPro;
using UnityEngine;
using Utils;

namespace Managers.Card.Components
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class CardCharacterManager: MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private CancellationTokenSource cts;
        private CardCharacter currentCharacter;
        private TextProOnACircle circularTextPro;
        private string currentImage;
        [SerializeField, ChildObjectOnly, NotNull] private TMP_Text characterName;
        
        private static readonly int CharacterTex = Shader.PropertyToID("_CharacterTex");
        private static readonly int CharacterTransform = Shader.PropertyToID("_CharacterTransform");

        public void Initialize()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            circularTextPro = characterName.gameObject.GetComponent<TextProOnACircle>();
            meshRenderer.material = new Material(meshRenderer.sharedMaterial);
        }

        public async UniTask SetCharacter(CharacterData character, int variantIndex)
        {
            currentCharacter = new CardCharacter(character, variantIndex);
            var loadedSprite = await LoadCharacterImageDedup();
            if (!loadedSprite) return;
            characterName.text = character.enName[0];
            circularTextPro?.ForceUpdate();
            meshRenderer.material.SetTexture(CharacterTex, loadedSprite.texture);
            UpdateCharacterTransform(currentCharacter.transform);
        }

        private async UniTask<Sprite> LoadCharacterImageDedup()
        {
            if(currentImage == currentCharacter.currentVariant) return null;
            currentImage = currentCharacter.currentVariant;
            cts?.Cancel();
            cts = new CancellationTokenSource();
            try
            {
                var handle = await AssetLoader.LoadAssetAsync<Sprite>($"Characters/{currentImage}.png", cts.Token);
                return handle.Result;
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load character image: {e.Message}");
            }

            return null;
        }

        public void UpdateCharacterTransform(Vector3 characterTransform)
        {
            meshRenderer.material.SetVector(CharacterTransform, characterTransform);
        }
    }
}