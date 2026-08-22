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
            public PanelType type;
            [PrefabObjectOnly, NotNull]
            public BasePanel prefab;
        }

        [SerializeField, NotNull] private Canvas canvasRoot;
        [SerializeField, ReorderableList] private List<PanelEntry> panelPrefabs;

        private readonly Dictionary<PanelType, BasePanel> prefabDict = new();
        private readonly Stack<BasePanel> panelStack = new();
        
        [Inject] private IObjectResolver resolver;

        private void Awake()
        {
            foreach (var entry in panelPrefabs)
            {
                if (entry.prefab != null)
                {
                    prefabDict[entry.type] = entry.prefab;
                }
            }
        }
        
        public void Start()
        {
            OpenPanel(PanelType.Start);
        }

        public BasePanel OpenPanel(PanelType type)
        {
            if (!prefabDict.TryGetValue(type, out var prefab))
            {
                Debug.LogError($"Panel type {type} not found in manager!");
                return null;
            }

            // Handle current top panel
            if (panelStack.Count > 0)
            {
                var currentTop = panelStack.Peek();
                currentTop.OnLostFocus();
            }

            // Instantiate new panel
            var instance = resolver.Instantiate(prefab, canvasRoot.transform);
            instance.Manager = this;
            
            // Push and initialize
            panelStack.Push(instance);
            
            // Bring to front handled by instantiation order or explicit sibling index
            instance.transform.SetAsLastSibling();
            
            instance.OnInitialize();
            instance.OnOpen();
            
            return instance;
        }

        public async UniTask<bool> CloseCurrentPanel()
        {
            if (panelStack.Count == 0)
            {
                Debug.LogWarning($"No panels found in stack, cannot close!");
                return false;
            }

            var activePanel = panelStack.Pop();
            try
            {
                // Wait for custom closing logic (animations, cleanup, etc.)
                await activePanel.OnClose();
            
                // Finally destroy the game object
                if (activePanel != null && activePanel.gameObject != null)
                {
                    Destroy(activePanel.gameObject);
                }

                // Resume previous panel
                if (panelStack.Count <= 0) return true;
                var nextTop = panelStack.Peek();
                nextTop.OnFocus();
            }
            catch (Exception e)
            {
               Debug.LogError($"Failed to close panel {activePanel.gameObject.name}: {e.Message}"); 
               return false;
            }
            return true;
        }

        // Helper for panels to close themselves
        public void ClosePanel(BasePanel panel)
        {
            if (panelStack.Count > 0 && panelStack.Peek() == panel)
            {
                CloseCurrentPanel().Forget();
            }
            else
            {
                Debug.LogWarning("Trying to close a panel that is not on top!");
            }
        }
    }
}
