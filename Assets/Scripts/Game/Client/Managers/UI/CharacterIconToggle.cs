using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Client.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.Managers.UI
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
            try
            {
                var handle = await AssetLoader.LoadAssetAsync<Sprite>($"Characters/{image}.png", token);
                buttonImage.sprite = handle.Result;
                buttonImage.SetNativeSize();
            }
            catch (OperationCanceledException) {}
            catch (Exception e) {Debug.LogException(e);}
        }
    }
}