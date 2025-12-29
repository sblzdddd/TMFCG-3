using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Client.Utils
{
    public static class AssetLoader
    {
        public static async UniTask<AsyncOperationHandle<T>> LoadAssetAsync<T>(string path, CancellationToken token)
            where T : UnityEngine.Object
        {
            token.ThrowIfCancellationRequested();
            var handle = Addressables.LoadAssetAsync<T>(path);
            try
            {
                await handle.ToUniTask(cancellationToken: token);
                token.ThrowIfCancellationRequested();
                return handle;
            }
            catch
            {
                if(handle.IsValid()) Addressables.Release(handle);
                throw;
            }
        }
    }
}