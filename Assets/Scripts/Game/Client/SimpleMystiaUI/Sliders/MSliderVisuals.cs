using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.SimpleMystiaUI.Sliders
{
    public class MSliderVisuals
    {
        private readonly RectTransform m_FillRect;
        private readonly RectTransform m_HandleRect;
        private Image m_FillImage;
        private Transform m_FillTransform;
        private RectTransform m_FillContainerRect;
        private Transform m_HandleTransform;
        private RectTransform m_HandleContainerRect;
        private DrivenRectTransformTracker m_Tracker;
        private MotionHandle? m_CurrentTween;

        // Duration for the slider tween animation in seconds
        private const float TweenDurationDrag = 0.1f;
        private const float TweenDurationNormal = 0.2f;

        public MSliderVisuals(RectTransform fillRect, RectTransform handleRect)
        {
            m_FillRect = fillRect;
            m_HandleRect = handleRect;
            UpdateCachedReferences();
        }

        private void SafeKillTween()
        {
            if (m_CurrentTween == null || !m_CurrentTween.Value.IsPlaying()) return;
            m_CurrentTween.Value.TryCancel();
        }

        public void UpdateVisuals(float normalizedValue, bool instant = false, bool isDragging = false)
        {
            // Force instant updates when not in play mode
            if (!Application.isPlaying)
            {
                instant = true;
            }

            float tweenDuration = isDragging ? TweenDurationDrag : TweenDurationNormal;

            m_Tracker.Clear();
            SafeKillTween();

            if (m_FillContainerRect)
            {
                m_Tracker.Add(null, m_FillRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                if (m_FillImage && m_FillImage.type == Image.Type.Filled)
                {
                    if (instant)
                    {
                        m_FillImage.fillAmount = normalizedValue;
                    }
                    else
                    {
                        m_CurrentTween = LMotion.Create(
                            m_FillImage.fillAmount,
                            normalizedValue,
                            tweenDuration
                        )
                            .WithEase(Ease.OutQuad)
                            .Bind(x => m_FillImage.fillAmount = x);
                    }
                }
                else
                {
                    if (instant)
                    {
                        anchorMax.x = normalizedValue;
                        m_FillRect.anchorMin = anchorMin;
                        m_FillRect.anchorMax = anchorMax;
                    }
                    else
                    {
                        Vector2 currentAnchorMax = m_FillRect.anchorMax;
                        m_CurrentTween = LMotion.Create(
                            currentAnchorMax.x,
                            normalizedValue,
                            duration: tweenDuration
                        ).WithEase(Ease.OutQuad)
                        .Bind(x =>
                            {
                                currentAnchorMax.x = x;
                                m_FillRect.anchorMax = currentAnchorMax;
                            }
                        );
                        m_FillRect.anchorMin = anchorMin;
                    }
                }
            }

            if (m_HandleContainerRect != null)
            {
                m_Tracker.Add(null, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                if (instant)
                {
                    anchorMin.x = anchorMax.x = normalizedValue;
                    m_HandleRect.anchorMin = anchorMin;
                    m_HandleRect.anchorMax = anchorMax;
                }
                else
                {
                    Vector2 currentAnchorMin = m_HandleRect.anchorMin;
                    Vector2 currentAnchorMax = m_HandleRect.anchorMax;
                    m_CurrentTween = LMotion.Create(
                        currentAnchorMin.x,
                        normalizedValue,
                        duration: tweenDuration
                    ).WithEase(Ease.OutQuad)
                    .Bind(x =>
                        {
                            currentAnchorMin.x = x;
                            currentAnchorMax.x = x;
                            m_HandleRect.anchorMin = currentAnchorMin;
                            m_HandleRect.anchorMax = currentAnchorMax;
                        }
                    );
                }
            }
        }

        public void ClearTweens()
        {
            SafeKillTween();
        }

        public void UpdateCachedReferences()
        {
            if (m_FillRect && m_FillRect)
            {
                m_FillTransform = m_FillRect.transform;
                m_FillImage = m_FillRect.GetComponent<Image>();
                if (m_FillTransform.parent != null)
                    m_FillContainerRect = m_FillTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_FillContainerRect = null;
                m_FillImage = null;
            }

            if (m_HandleRect && m_HandleRect != null)
            {
                m_HandleTransform = m_HandleRect.transform;
                if (m_HandleTransform.parent != null)
                    m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HandleContainerRect = null;
            }
        }

        public void ClearTracker()
        {
            m_Tracker.Clear();
        }

        public bool HasValidVisuals => m_HandleContainerRect != null || m_FillContainerRect != null;

        public RectTransform HandleRect => m_HandleRect;
        public RectTransform HandleContainerRect => m_HandleContainerRect;
        public RectTransform FillContainerRect => m_FillContainerRect;
    }
} 