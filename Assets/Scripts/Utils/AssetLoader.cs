using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Utils
{
    public static class AssetLoader
    {
        public static async UniTask<Sprite> LoadCharacterAsync(string image, CancellationToken token)
        {
            return await LoadImageAsync($"Characters/{image}.png", token);
        }

        private static async UniTask<Sprite> LoadImageAsync(string path, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                var loadOperation = Addressables.LoadAssetAsync<Sprite>(path);
                var loadedSprite = await loadOperation.ToUniTask(cancellationToken: token);

                if (!token.IsCancellationRequested) return loadedSprite;
                
                Addressables.Release(loadOperation);
                return null;
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"Failed to load image at path '{path}': Operation cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load image at path '{path}': {ex.Message}");
            }
            return null;
        }
    }
}