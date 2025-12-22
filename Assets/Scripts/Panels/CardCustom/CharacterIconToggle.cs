using System.Threading;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utils;

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
            var loadedSprite = await AssetLoader.LoadCharacterAsync(characterImage, token);
            if (!loadedSprite) return;
            buttonImage.sprite = loadedSprite;
            buttonImage.SetNativeSize();
        }
    }
}