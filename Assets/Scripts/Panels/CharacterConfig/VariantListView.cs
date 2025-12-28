using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Editors;
using Managers;
using TmiCard.POCO;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Panels.CharacterConfig
{
    [RequireComponent(typeof(GridLayoutGroup), typeof(ToggleGroup))]
    public class VariantListView: MonoBehaviour
    {
        [SerializeField, PrefabObjectOnly, NotNull] public GameObject characterButtonPrefab;
        [SerializeField, NotNull] public ScrollRect scrollRect;
        [Inject] private readonly CharacterEditor characterEditor;
        private readonly List<GameObject> characterButtons = new();
        private CancellationTokenSource refreshCts;
        private int refreshVersion;
        private ToggleGroup toggleGroup;

        private void Start()
        {
            toggleGroup = GetComponent<ToggleGroup>();
        }

        public async UniTask RefreshList(CharacterData characterData, int selectIndex = 0)
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
                Toggle targetToggle = null;
                characterButtons.ForEach(Destroy);
                characterButtons.Clear();
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
                for (var i = 0; i < characterData.variants.Count; i++)
                {
                    var variant = characterData.variants[i];
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
                    loadingTasks.Add(LoadWithVersionCheck(btn, variant, myVersion, token, characterData, i));
                    
                    if (i == selectIndex)
                    {
                        targetToggle = btnGo.GetComponent<Toggle>();
                    }

                    LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
                }
                await UniTask.WhenAll(loadingTasks).AttachExternalCancellation(token);
                
                // Auto-select the target toggle
                if (targetToggle != null && !token.IsCancellationRequested)
                {
                    targetToggle.isOn = true;
                }
                // Fallback to 0 if target not found (e.g. index out of bounds)
                else if (characterButtons.Count > 0 && !token.IsCancellationRequested)
                {
                    var t = characterButtons[0].GetComponent<Toggle>();
                    if (t != null) t.isOn = true;
                }
                Debug.Log("All character variants have been loaded successfully!");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Grid refresh operation was cancelled.");
            }
        }
        
        private async UniTask LoadWithVersionCheck(
            CharacterIconToggle btn,
            string image,
            int version,
            CancellationToken token,
            CharacterData data,
            int index)
        {
            if (version != refreshVersion) return;
            await btn.LoadCharacterIcon(image, token);
            var toggle = btn.GetComponent<Toggle>();
            if(toggle == null) return;
            toggle.group = toggleGroup;
            
            toggle.onValueChanged.AddListener((val) =>
            {
                if(!val) return;
                characterEditor.SelectCharacter(data.characterId, index);
                Debug.Log(image);
            });
        }
    }
}