using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Client.Data.Providers;
using Game.Shared.TmiCards.POCO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;

namespace Game.Client.Managers.UI
{
    [RequireComponent(typeof(GridLayoutGroup), typeof(ToggleGroup), typeof(CanvasGroup))]
    public class CharacterGridView: MonoBehaviour
    {
        [SerializeField, PrefabObjectOnly, NotNull] public GameObject characterButtonPrefab;
        [SerializeField, NotNull] public Button refreshGridButton;
        [SerializeField, NotNull] public ScrollRect scrollRect;
        [SerializeField, NotNull] public CharacterVariantsView characterVariantsView;
        [Inject] private readonly CharacterProvider provider;
        public UnityEvent<(string, int)> onCharacterChanged;
        private readonly List<GameObject> characterButtons = new();
        private CancellationTokenSource refreshCts;
        private int refreshVersion;
        private ToggleGroup toggleGroup;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            toggleGroup = GetComponent<ToggleGroup>();
            canvasGroup = GetComponent<CanvasGroup>();
            refreshGridButton.onClick.AddListener(() => RefreshGrid().Forget());
        }


        public async UniTask RefreshGrid()
        {
            // init version check and cts
            canvasGroup.interactable = false;
            var myVersion = ++refreshVersion;
            refreshCts?.Cancel();
            refreshCts?.Dispose();
            refreshCts = new CancellationTokenSource();
            var token = refreshCts.Token;
            try
            {
                // stop early on wrong version
                if (myVersion != refreshVersion) return;
                var loadingTasks = new List<UniTask>();
                // destroy old
                characterButtons.ForEach(Destroy);
                characterButtons.Clear();
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
                foreach (var characterData in provider.ValidCharacters)
                {
                    // early stop on cancel
                    token.ThrowIfCancellationRequested();
                    // yield 1 frame on every 5 frames to avoid freezing
                    if (characterButtons.Count % 5 == 0) await UniTask.Yield();

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
            catch (Exception e)
            {
                Debug.LogError($"Error during character grid refresh: {e}");
            }
            canvasGroup.interactable = true;
        }
        
        private async UniTask LoadWithVersionCheck(
            CharacterIconToggle btn,
            CharacterData data,
            int version,
            CancellationToken token)
        {
            if (version != refreshVersion) return;
            await btn.LoadCharacterIcon(data.variants[0], token);
            var toggle = btn.gameObject.GetComponent<Toggle>();
            if(toggle == null) return;
            toggle.group = toggleGroup;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(val =>
            {
                if(!val) return;
                onCharacterChanged.Invoke((data.characterId, 0));
                characterVariantsView.RefreshList(data).Forget();
            });
        }

        public async UniTask Select(string characterId, int characterVariantIndex)
        {
            CharacterData targetData = null;
            var buttonIndex = 0;
            var found = false;

            foreach (var characterData in provider.Characters.Where(characterData => characterData.variants.Count > 0))
            {
                if (characterData.characterId == characterId)
                {
                    targetData = characterData;
                    found = true;
                    break;
                }
                buttonIndex++;
            }

            if (found && buttonIndex < characterButtons.Count)
            {
                var btnGo = characterButtons[buttonIndex];
                var toggle = btnGo.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.SetIsOnWithoutNotify(true);
                    onCharacterChanged.Invoke((targetData.characterId, 0));
                    await characterVariantsView.RefreshList(targetData, characterVariantIndex);
                }
            }
        }
        
        private void OnDestroy()
        {
            refreshCts?.Cancel();
            refreshCts?.Dispose();
        }
    }
}