using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Managers;
using TmiCard.POCO;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Panels.CardCustom
{
    [RequireComponent(typeof(GridLayoutGroup), typeof(ToggleGroup))]
    public class CharacterGridView: MonoBehaviour
    {
        [SerializeField] [PrefabObjectOnly] [NotNull] public GameObject characterButtonPrefab;
        [SerializeField] [NotNull] public Button refreshGridButton;
        [SerializeField] [NotNull] public ScrollRect scrollRect;
        [SerializeField] [NotNull] public VariantListView variantListView;
        [Inject] private readonly CharacterConfigService characterConfigService;
        private readonly List<GameObject> characterButtons = new();
        private CancellationTokenSource refreshCts;
        private int refreshVersion;
        private ToggleGroup toggleGroup;

        private void Start()
        {
            toggleGroup = GetComponent<ToggleGroup>();
            refreshGridButton.onClick.AddListener(() => RefreshGrid().Forget());
        }

        public async UniTask RefreshGrid()
        {
            var myVersion = ++refreshVersion;
            refreshCts?.Cancel();
            refreshCts?.Dispose();
            refreshCts = new CancellationTokenSource();
            var token = refreshCts.Token;
            try
            {
                if (myVersion != refreshVersion) return;
                var loadingTasks = new List<UniTask>();
                characterButtons.ForEach(Destroy);
                characterButtons.Clear();
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
                foreach (var characterData in characterConfigService.characters)
                {
                    if (characterData.variants.Count <= 0) continue;
                    if (token.IsCancellationRequested) return;
                    
                    if (characterButtons.Count % 5 == 0)
                    {
                        await UniTask.Yield();
                        if (token.IsCancellationRequested) return;
                    }
                    var btnGo = Instantiate(characterButtonPrefab, transform);
                    characterButtons.Add(btnGo);
                    var btn = btnGo.GetComponent<CharacterIconToggle>();
                    if (btn == null) return;
                    loadingTasks.Add(LoadWithVersionCheck(btn, characterData, myVersion, token));
                    LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
                }
                await UniTask.WhenAll(loadingTasks).AttachExternalCancellation(token);
                Debug.Log("All character images have been loaded successfully!");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Grid refresh operation was cancelled.");
            }
        }
        
        private async UniTask LoadWithVersionCheck(
            CharacterIconToggle btn,
            CharacterData data,
            int version,
            CancellationToken token)
        {
            if (version != refreshVersion) return;
            await btn.LoadCharacterIcon(data.variants[0].image, token);
            var toggle = btn.gameObject.GetComponent<Toggle>();
            if(toggle == null) return;
            toggle.group = toggleGroup;
            btn.gameObject.GetComponent<Toggle>()?.onValueChanged.AddListener((val) =>
            {
                if(!val) return;
                variantListView.RefreshList(data).Forget();
            });
        }
        
        private void OnDestroy()
        {
            refreshCts?.Cancel();
            refreshCts?.Dispose();
        }
    }
}