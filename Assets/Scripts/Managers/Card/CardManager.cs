using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers.Card.Components;
using Managers.Card.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers.Card
{
    [RequireComponent(typeof(MeshRenderer), typeof(CardCharacterManager))]
    public class CardManager: MonoBehaviour
    {
        private MeshRenderer cardBaseRenderer;
        [FormerlySerializedAs("cardValue")] [SerializeField] [ChildObjectOnly] [NotNull]
        private CardFaceManager cardFace;
        private CardCharacterManager cardCharacter;
        private CardOutlineManager cardOutline;
        
        public CardCharacterManager CharacterManager => cardCharacter;
        private readonly Dictionary<Type, CardExtension> extensions = new();

        public bool IsSelected { get; private set; }

        public void Initialize()
        {
            cardBaseRenderer = GetComponent<MeshRenderer>();
            cardOutline = GetComponent<CardOutlineManager>();
            cardCharacter = GetComponent<CardCharacterManager>();
            
            cardFace.Initialize();
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

        public async UniTask Dispose()
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
