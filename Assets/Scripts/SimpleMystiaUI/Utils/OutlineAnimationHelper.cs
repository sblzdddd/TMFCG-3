 using UnityEngine;
using UnityEngine.UI;

namespace SimpleMystiaUI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class OutlineAnimationHelper : MonoBehaviour
    {
        private static readonly int OutlineWidthProperty = Shader.PropertyToID("_OutlineWidth");
        
        [SerializeField] private float outlineWidth = 1f;
        
        private Image image;
        private Material materialInstance;
        private bool materialInstanceCreated = false;

        private void Awake()
        {
            EnsureSetup();
        }
        
        private void OnEnable()
        {
            EnsureSetup();
        }

        private void EnsureSetup()
        {
            if (materialInstanceCreated) return;
            
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            // Create a material instance if we don't have one
            if (image != null && image.material != null && 
                image.material.shader.name == "SimpleMystiaUI/UIOutlinePixel")
            {
                // If we already have a material instance, don't create a new one
                if (materialInstance == null)
                {
                    materialInstance = new Material(Shader.Find("SimpleMystiaUI/UIOutlinePixel"));
                    materialInstanceCreated = true;
                }
                image.material = materialInstance;
                
                // Apply current value immediately
                materialInstance.SetFloat(OutlineWidthProperty, outlineWidth);
            }
        }

        private void Reset()
        {
            // Called when component is first added or reset
            EnsureSetup();
        }
        
        private void Update()
        {
            #if UNITY_EDITOR
            if (!materialInstanceCreated || materialInstance == null)
            {
                EnsureSetup();
            }
            #endif

            if (materialInstance != null)
            {
                materialInstance.SetFloat(OutlineWidthProperty, outlineWidth);
            }
        }
        
        public float OutlineWidth
        {
            get => outlineWidth;
            set
            {
                outlineWidth = value;
                if (!materialInstanceCreated || materialInstance == null)
                {
                    EnsureSetup();
                }
                if (materialInstance != null)
                {
                    materialInstance.SetFloat(OutlineWidthProperty, value);
                }
            }
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure setup when inspector values change
            if (!materialInstanceCreated || materialInstance == null)
            {
                EnsureSetup();
            }
            
            if (materialInstance != null)
            {
                materialInstance.SetFloat(OutlineWidthProperty, outlineWidth);
            }
        }
#endif
    }
}