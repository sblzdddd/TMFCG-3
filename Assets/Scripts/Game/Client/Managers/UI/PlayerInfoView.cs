using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Client.Data.Providers;
using Game.Client.Utils;
using Game.Shared.TmiCards.POCO;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Client.Managers.UI
{
    public class PlayerInfoView: MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image avatarImage;

        [Inject] private PlayerDataProvider playerDataProvider;
        [Inject] private CharacterProvider characterProvider;

        private ICollection<IDisposable> disposables;
        private CancellationToken ct;

        private void Start()
        {
            playerDataProvider.PlayerInfo
                .Where(info => info != null)
                .Subscribe(info => UpdateUI(info).Forget())
                .AddTo(disposables);
        }

        private async UniTask UpdateUI(PlayerInfoData info)
        {
            nameText.text = info.name;
            if (!characterProvider.TryGetVariant(info.avatarId, info.avatarVariantIndex, out var variant)) return;
            var result = await AssetLoader.LoadAssetAsync<Sprite>($"Characters/{variant}", ct);
            avatarImage.sprite = result.Result;
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }
    }
}