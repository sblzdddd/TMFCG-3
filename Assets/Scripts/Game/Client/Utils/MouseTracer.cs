using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Client.Utils
{
    [RequireComponent(typeof(RawImage))]
    public class MouseTracer : MonoBehaviour
    {
        private RawImage uiImage;
        private RectTransform uiRect;

        private void Awake()
        {
            uiImage = GetComponent<RawImage>();
            uiRect = uiImage.rectTransform;
        }

        private void Update()
        {
            if (Mouse.current == null) return;
            
            var mouseScreen = Mouse.current.position.ReadValue();
        
            if (uiRect) uiRect.position = mouseScreen;
            
            var isInside = mouseScreen.x >= 0 && mouseScreen.x <= Screen.width &&
                           mouseScreen.y >= 0 && mouseScreen.y <= Screen.height;

            // Set visibility based on position
            Cursor.visible = !isInside;
        }
    }
}
