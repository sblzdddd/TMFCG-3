using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace Game.Client.SimpleMystiaUI.Utils
{
    public static class MuiNavigationReader
    {
        private static InputSystemUIInputModule uiModule;
        public static bool IsInitialized;

        /// <summary>
        /// Call this once to initialize the reference to InputSystemUIInputModule.
        /// You can pass null to auto-locate it in the scene.
        /// </summary>
        public static void Initialize(InputSystemUIInputModule module = null)
        {
            if (module)
            {
                uiModule = module;
            }
            else if (!uiModule)
            {
                uiModule = Object.FindFirstObjectByType<InputSystemUIInputModule>();
            }

            IsInitialized = true;
        }

        /// <summary>
        /// Returns the current navigation axis (Vector2) from the InputSystemUIInputModule.
        /// </summary>
        public static Vector2 GetNavigationAxis()
        {
            if (uiModule == null)
            {
                Debug.LogWarning("NavigationReader not initialized. Call NavigationReader.Initialize() first.");
                return Vector2.zero;
            }

            if (uiModule.move == null || uiModule.move.action == null)
            {
                Debug.LogWarning("Navigate action is not assigned in InputSystemUIInputModule.");
                return Vector2.zero;
            }

            return uiModule.move.action.ReadValue<Vector2>();
        }
    }
}