using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Client.Data.Providers;
using Game.Client.Dto;
using Game.Client.Managers.Card.Components;
using Game.Client.Managers.Card.Extensions;
using UnityEngine;
using VContainer;

namespace Game.Client.Managers.Card
{
    [RequireComponent(typeof(MeshRenderer), typeof(CardCharacterManager), typeof(CardFaceManager))]
    public class CardManager: MonoBehaviour
    {
        private MeshRenderer cardBaseRenderer;
        private CardFaceManager cardFace;
        private CardCharacterManager cardCharacter;
        private CardOutlineManager cardOutline;
        [Inject] private readonly DeckDataProvider deckDataProvider;
        
        public CardCharacterManager CharacterManager => cardCharacter;
        private readonly Dictionary<Type, CardExtension> extensions = new();

        public bool IsSelected { get; private set; }

        public void Start()
        {
            cardBaseRenderer = GetComponent<MeshRenderer>();
            cardOutline = GetComponent<CardOutlineManager>();
            cardCharacter = GetComponent<CardCharacterManager>();
            cardFace = GetComponent<CardFaceManager>();
            
            cardOutline?.Initialize();
            cardCharacter?.Initialize();
            cardBaseRenderer.material = new Material(cardBaseRenderer.sharedMaterial);
                    
            foreach (var extension in GetComponents<CardExtension>())
            {
                extension.Initialize(this);
                extensions[extension.GetType()] = extension;
            }
        }

        public void SetSelected(bool selected)
        {
            if (!cardOutline) return;
            IsSelected = selected;
            cardOutline.SetOutline(selected);
        }

        public void SetCard(CardPresentation presentation)
        {
            cardFace.SetFace(presentation.Card.value, presentation.Card.suit);
            cardCharacter.SetCharacter(presentation.Character, presentation.VariantIndex).Forget();
        }

        public void Dispose()
        {
            foreach (var extension in GetComponents<CardExtension>())
            {
                DisableExtension(extension);
            }
            // play destroy animation
        }
        
        public T EnableExtension<T>() where T : CardExtension
        {
            if (!extensions.TryGetValue(typeof(T), out var extension))
            {
                Debug.LogError($"Variant {typeof(T)} not found on prefab");
                return null;
            }

            EnableExtension(extension);
            return (T)extension;
        }

        public void DisableExtension<T>() where T : CardExtension
        {
            if (!extensions.TryGetValue(typeof(T), out var extension)) 
            {
                Debug.LogError($"Variant {typeof(T)} not found on prefab");
                return;
            }

            DisableExtension(extension);
        }

        private static void DisableExtension(CardExtension extension)
        {
            if (!extension.enabled) return;
            extension.OnDeactivate();
            extension.enabled = false;
        }
        private static void EnableExtension(CardExtension extension)
        {
            if (extension.enabled) return;
            extension.OnActivate();
            extension.enabled = true;
        }
    }
}
