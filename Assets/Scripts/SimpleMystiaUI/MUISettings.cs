using UnityEngine;

namespace SimpleMystiaUI
{
    public class MUISettings : ScriptableObject
    {
        private static MUISettings _instance;
        public static MUISettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<MUISettings>("MUI Settings");
                    if (_instance == null)
                    {
                        Debug.LogWarning("MUI Settings not found in Resources folder. Please create one at Resources/MUI Settings");
                    }
                }
                return _instance;
            }
        }

        [BeginGroup("Slider")]
        [Tooltip("Default style for sliders if none is assigned")]
        [Header("Keyboard Step Scale")]
        public float defaultSliderStepAcceleration = 0.2f;
        [EndGroup]
        public float defaultSliderStepMax = 6f;

        [BeginGroup("Audio")]
        [Tooltip("Default sound for buttons if none is assigned")]
        [InLineEditor] public AudioClip defaultClickSound;
        [InLineEditor] public AudioClip defaultSelectedSound;
        [EndGroup]
        [InLineEditor] public AudioClip defaultQuitSound;

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Force refresh the instance if we're in the editor
            _instance = null;
        }
#endif
    }
} 