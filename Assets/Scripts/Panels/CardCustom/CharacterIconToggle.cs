using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Panels.CardCustom
{
    [RequireComponent(typeof(Toggle))]
    public class CharacterIconToggle: MonoBehaviour
    {
        [SerializeField][Disable] private string characterImage;
        [SerializeField][ChildObjectOnly][NotNull] private Image buttonImage;
        
        public async UniTask LoadCharacterIcon(string image, CancellationToken token)
        {
            
            token.ThrowIfCancellationRequested();
            characterImage = image;
            var assetPath = $"Characters/{characterImage}.png";

            try
            {
                var loadOperation = Addressables.LoadAssetAsync<Sprite>(assetPath);
                var loadedSprite = await loadOperation.ToUniTask(cancellationToken: token);

                if (token.IsCancellationRequested)
                {
                    Addressables.Release(loadOperation);
                    return;
                }

                buttonImage.sprite = loadedSprite;
                buttonImage.SetNativeSize();
            }
            catch (OperationCanceledException)
            {
                // This is expected when the item scrolls out of view. No need to log.
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load sprite at path '{assetPath}': {ex.Message}");
            }
        }
    }
}