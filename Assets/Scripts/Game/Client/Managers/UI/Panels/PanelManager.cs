using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Client.Managers.UI.Panels.Base;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Client.Managers.UI.Panels
{
    public class PanelManager : MonoBehaviour, IStartable
    {
        [Serializable]
        public struct PanelEntry
        {
            public PanelType Type;
            public BasePanel Prefab;
        }

        [SerializeField] private Transform canvasRoot;
        [SerializeField] private List<PanelEntry> panelPrefabs;

        private readonly Dictionary<PanelType, BasePanel> _prefabDict = new();
        private readonly Stack<BasePanel> _panelStack = new();
        
        [Inject] private IObjectResolver _resolver;

        private void Awake()
        {
            foreach (var entry in panelPrefabs)
            {
                if (entry.Prefab != null)
                {
                    _prefabDict[entry.Type] = entry.Prefab;
                }
            }
        }
        
        public void Start()
        {
            OpenPanel(PanelType.Start);
        }

        public BasePanel OpenPanel(PanelType type)
        {
            if (!_prefabDict.TryGetValue(type, out var prefab))
            {
                Debug.LogError($"Panel type {type} not found in manager!");
                return null;
            }

            // Handle current top panel
            if (_panelStack.Count > 0)
            {
                var currentTop = _panelStack.Peek();
                currentTop.OnLostFocus();
            }

            // Instantiate new panel
            var instance = _resolver.Instantiate(prefab, canvasRoot);
            instance.Manager = this;
            
            // Push and initialize
            _panelStack.Push(instance);
            
            // Bring to front handled by instantiation order or explicit sibling index
            instance.transform.SetAsLastSibling();
            
            instance.OnInitialize();
            instance.OnOpen();
            
            return instance;
        }

        public async void CloseCurrentPanel()
        {
            if (_panelStack.Count == 0) return;

            var activePanel = _panelStack.Pop();
            
            // Wait for custom closing logic (animations, cleanup, etc.)
            await activePanel.OnClose();
            
            // Finally destroy the game object
            if (activePanel != null && activePanel.gameObject != null)
            {
                Destroy(activePanel.gameObject);
            }

            // Resume previous panel
            if (_panelStack.Count > 0)
            {
                var nextTop = _panelStack.Peek();
                nextTop.OnFocus();
            }
        }

        // Helper for panels to close themselves
        public void ClosePanel(BasePanel panel)
        {
            if (_panelStack.Count > 0 && _panelStack.Peek() == panel)
            {
                CloseCurrentPanel();
            }
            else
            {
                Debug.LogWarning("Trying to close a panel that is not on top!");
            }
        }
    }
}
