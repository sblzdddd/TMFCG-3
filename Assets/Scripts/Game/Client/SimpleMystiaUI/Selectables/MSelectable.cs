using Game.Client.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Client.SimpleMystiaUI.Selectables
{
    public class MSelectable : Selectable, IPointerDownHandler
    {
        private static bool _hasDefaultSelection;

        [SerializeField] protected bool isDefaultSelected;
        [SerializeField] protected bool overrideSounds;
        [SerializeField] protected AudioClip selectedSound;
        [SerializeField] protected AudioClip clickSound;

        [SerializeField] protected UnityEvent<bool> onSelectionChanged;
        
        private AudioManager audioManager;
        
        protected bool _isHovered;
        protected bool _isCurrentlySelected;

        public UnityEvent<bool> OnSelectionChanged => onSelectionChanged;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (isDefaultSelected && !_hasDefaultSelection)
            {
                _hasDefaultSelection = true;
                Select();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (isDefaultSelected && _isCurrentlySelected)
            {
                _hasDefaultSelection = false;
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (!_isCurrentlySelected)
            {
                _isCurrentlySelected = true;
                PlaySelectedSound();
                OnSelectionChanged.Invoke(true);
            }
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (_isCurrentlySelected)
            {
                _isCurrentlySelected = false;
                OnSelectionChanged.Invoke(false);
            }
            base.OnDeselect(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            
            if (!_isHovered)
            {
                _isHovered = true;
                Select();
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _isHovered = false;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            PlayClickSound();
        }

        protected virtual void PlayClickSound()
        {
            if (MUISettings.Instance.defaultClickSound != null && !overrideSounds)
            {
                audioManager?.PlayUISound(MUISettings.Instance.defaultClickSound);
            }
            else if (clickSound != null && overrideSounds)
            {
                audioManager?.PlayUISound(clickSound);
            }
        }

        protected virtual void PlaySelectedSound()
        {
            if (MUISettings.Instance.defaultSelectedSound != null && !overrideSounds)
            {
                audioManager?.PlayUISound(MUISettings.Instance.defaultSelectedSound);
            } else if (selectedSound != null && overrideSounds)
            {
                audioManager?.PlayUISound(selectedSound);
            }
        }


        protected override void Start()
        {
            base.Start();
            
            // If this is marked as default selected, select it
            if (isDefaultSelected)
                Select();
            // If no selectable is selected in the scene and no default selectable exists, select the first selectable
            else if (!_hasDefaultSelection && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            {
                MSelectable[] selectables = FindObjectsByType<MSelectable>(FindObjectsSortMode.InstanceID);
                if (selectables.Length > 0 && selectables[0] == this)
                {
                    Select();
                    _hasDefaultSelection = true;
                }
            }
        }

    }
}
