using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(RawImage))]
public class MouseTracer : MonoBehaviour
{
    RawImage uiImage;
    RectTransform uiRect;

    void Awake()
    {
        uiImage = GetComponent<RawImage>();
        uiRect = uiImage.rectTransform;
    }

    void Update()
    {
        if (!TryGetMouseScreenPosition(out var mouseScreen)) return;
        
        if (uiRect)
        {
            uiRect.position = mouseScreen;
        }
    }

    bool TryGetMouseScreenPosition(out Vector3 screenPos)
    {
        if (Mouse.current == null)
        {
            screenPos = default;
            return false;
        }
        Vector2 p = Mouse.current.position.ReadValue();
        screenPos = new Vector3(p.x, p.y, 0f);
        return true;
    }
}
