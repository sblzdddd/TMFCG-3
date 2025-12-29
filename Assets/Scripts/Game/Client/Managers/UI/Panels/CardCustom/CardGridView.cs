using System;
using System.Collections.Generic;
using Game.Client.Data.Editors;
using Game.Client.Data.Providers;
using Game.Shared.TmiCards.Card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Client.Managers.UI.Panels.CardCustom
{
    [RequireComponent(typeof(GridLayoutGroup), typeof(ToggleGroup))]
    public class CardGridView: MonoBehaviour
    {
        [SerializeField, PrefabObjectOnly, NotNull] public GameObject cardButtonPrefab;
        [SerializeField, NotNull] public Button refreshGridButton;
        [SerializeField, NotNull] public ScrollRect scrollRect;
        
        [Inject] private readonly DeckDataProvider deckDataProvider;
        [Inject] private readonly DeckDataEditor editor;
        [Inject] private readonly CharacterProvider provider;
        private readonly List<GameObject> cardButtons = new();
        
        private ToggleGroup toggleGroup;

        private void Start()
        {
            toggleGroup = GetComponent<ToggleGroup>();
            refreshGridButton.onClick.AddListener(RefreshGrid);
        }


        public void RefreshGrid()
        {
            try
            {
                // destroy old
                cardButtons.ForEach(Destroy);
                cardButtons.Clear();
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
                foreach (var cardData in deckDataProvider.Cards)
                {
                    try
                    {
                        var btnGo = Instantiate(cardButtonPrefab, transform);
                        cardButtons.Add(btnGo);

                        var toggle = btnGo.GetComponent<Toggle>();
                        if (toggle == null) throw new Exception("Toggle not found in Prefab");
                        toggle.group = toggleGroup;
                        toggle.onValueChanged.RemoveAllListeners();
                        toggle.onValueChanged.AddListener(val =>
                        {
                            if (!val) return;
                            editor.SelectCard(cardData.id);
                        });
                        var text = btnGo.GetComponentInChildren<TMP_Text>();
                        if (text == null) throw new Exception("TMP_Text not found in Prefab");

                        var characterVariant = "Invalid";
                        if (provider.TryGet(cardData.characterId, out var data))
                            characterVariant = data.variants[cardData.characterVariantIndex] ?? "Invalid";

                        text.text =
                            $"<sprite={(int)cardData.suit}>{cardData.value.ToDisplayString()}\n<size=10> {characterVariant}";
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error during character grid refresh: {e}");
                    }
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
                Debug.Log("All cards have been loaded successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during character grid refresh: {e}");
            }
        }
    }
}