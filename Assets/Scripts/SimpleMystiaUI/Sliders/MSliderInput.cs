using UnityEngine;
using UnityEngine.EventSystems;
using SimpleMystiaUI.Utils;

namespace SimpleMystiaUI.Sliders
{
    public class MSliderInput
    {
        private readonly MSlider m_Slider;
        private readonly MSliderVisuals m_Visuals;
        private float stepScaleAcceleration = 0.2f;
        private float stepScaleMax = 6f;
        private Vector2 m_Offset = Vector2.zero;
        public float stepScale { get; private set; } = 1;

        public void Start()
        {
            if(MUISettings.Instance != null)
            {
                stepScaleAcceleration = MUISettings.Instance.defaultSliderStepAcceleration;
                stepScaleMax = MUISettings.Instance.defaultSliderStepMax;
            }
        }

        public MSliderInput(MSlider slider, MSliderVisuals visuals)
        {
            m_Slider = slider;
            m_Visuals = visuals;
            Start();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            m_Offset = Vector2.zero;
            if (m_Visuals.HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_Visuals.HandleRect,
                    eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Visuals.HandleRect,
                        eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out Vector2 localMousePos))
                    m_Offset = localMousePos;
            }
            else
            {
                // Outside the slider handle - jump to this point instead
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public bool OnMove(AxisEventData eventData, float stepSize)
        {
            if(eventData.moveDir == MoveDirection.Left && m_Slider.FindSelectableOnLeft() == null)
            {
                m_Slider.value -= stepSize * stepScale;
                stepScale += (stepScale <= stepScaleMax) ? stepScaleAcceleration : 0;
                return true;
            }
            if(eventData.moveDir == MoveDirection.Right && m_Slider.FindSelectableOnRight() == null)
            {
                m_Slider.value += stepSize * stepScale;
                stepScale += (stepScale <= stepScaleMax) ? stepScaleAcceleration : 0;
                return true;
            }
            return false;
        }

        public void UpdateStepScale()
        {
            if(!MuiNavigationReader.IsInitialized)
                MuiNavigationReader.Initialize();
            Vector2 axis = MuiNavigationReader.GetNavigationAxis();
            if (axis.x != 0f) return;
            stepScale = 1;
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return m_Slider.IsActive() && m_Slider.IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        private void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_Visuals.HandleContainerRect ?? m_Visuals.FillContainerRect;
            if (clickRect != null && clickRect.rect.size[0] > 0)
            {
                Vector2 position = Vector2.zero;
                if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                    return;

                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out Vector2 localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                float val = Mathf.Clamp01((localCursor - m_Offset)[0] / clickRect.rect.size[0]);
                m_Slider.normalizedValue = val;
            }
        }
    }
} 