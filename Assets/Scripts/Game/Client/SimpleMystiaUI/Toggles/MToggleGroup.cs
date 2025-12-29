using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Client.SimpleMystiaUI.Toggles
{
    /// <summary>
    /// A component that represents a group of UI.Toggles.
    /// </summary>
    /// <remarks>
    /// When using a group reference the group from a UI.Toggle. Only one member of a group can be active at a time.
    /// </remarks>
    [AddComponentMenu("SimpleMystiaUI/MToggle Group", 31)]
    [DisallowMultipleComponent]
    public class MToggleGroup : UIBehaviour
    {
        [SerializeField] private bool m_AllowSwitchOff;

        /// <summary>
        /// Is it allowed that no toggle is switched on?
        /// </summary>
        /// <remarks>
        /// If this setting is enabled, pressing the toggle that is currently switched on will switch it off, so that no toggle is switched on. If this setting is disabled, pressing the toggle that is currently switched on will not change its state.
        /// Note that even if allowSwitchOff is false, the Toggle Group will not enforce its constraint right away if no toggles in the group are switched on when the scene is loaded or when the group is instantiated. It will only prevent the user from switching a toggle off.
        /// </remarks>
        public bool allowSwitchOff { get => m_AllowSwitchOff; set { m_AllowSwitchOff = value; } }

        protected List<MToggle> m_MToggles = new();

        protected MToggleGroup()
        {}

        /// <summary>
        /// Because all the Toggles have registered themselves in the OnEnabled, Start should check to
        /// make sure at least one Toggle is active in groups that do not AllowSwitchOff
        /// </summary>
        protected override void Start()
        {
            EnsureValidState();
            base.Start();
        }

        protected override void OnEnable()
        {
            EnsureValidState();
            base.OnEnable();
        }

        private void ValidateMToggleIsInGroup(MToggle toggle)
        {
            if (toggle == null || !m_MToggles.Contains(toggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] {toggle, this}));
        }

        /// <summary>
        /// Notify the group that the given toggle is enabled.
        /// </summary>
        /// <param name="toggle">The toggle that got triggered on.</param>
        /// <param name="sendCallback">If other toggles should send onValueChanged.</param>
        public void NotifyMToggleOn(MToggle toggle, bool sendCallback = true)
        {
            ValidateMToggleIsInGroup(toggle);
            // disable all toggles in the group
            for (var i = 0; i < m_MToggles.Count; i++)
            {
                if (m_MToggles[i] == toggle)
                    continue;

                if (sendCallback)
                    m_MToggles[i].isOn = false;
                else
                    m_MToggles[i].SetIsOnWithoutNotify(false);
            }
        }

        /// <summary>
        /// Unregister a toggle from the group.
        /// </summary>
        /// <param name="toggle">The toggle to remove.</param>
        public void UnregisterMToggle(MToggle toggle)
        {
            if (m_MToggles.Contains(toggle))
                m_MToggles.Remove(toggle);
        }

        /// <summary>
        /// Register a toggle with the toggle group so it is watched for changes and notified if another toggle in the group changes.
        /// </summary>
        /// <param name="toggle">The toggle to register with the group.</param>
        public void RegisterMToggle(MToggle toggle)
        {
            if (!m_MToggles.Contains(toggle))
                m_MToggles.Add(toggle);
        }

        /// <summary>
        /// Ensure that the toggle group still has a valid state. This is only relevant when a ToggleGroup is Started
        /// or a Toggle has been deleted from the group.
        /// </summary>
        public void EnsureValidState()
        {
            if (!allowSwitchOff && !AnyMTogglesOn() && m_MToggles.Count != 0)
            {
                m_MToggles[0].isOn = true;
                NotifyMToggleOn(m_MToggles[0]);
            }

            IEnumerable<MToggle> activeMToggles = ActiveMToggles();

            var mToggles = activeMToggles.ToList();
            if (mToggles.Count() > 1)
            {
                MToggle firstActive = GetFirstActiveMToggle();

                foreach (MToggle toggle in mToggles)
                {
                    if (toggle == firstActive)
                    {
                        continue;
                    }
                    toggle.isOn = false;
                }
            }
        }

        /// <summary>
        /// Are any of the toggles on?
        /// </summary>
        /// <returns>Are and of the toggles on?</returns>
        public bool AnyMTogglesOn()
        {
            return m_MToggles.Find(x => x.isOn) != null;
        }

        /// <summary>
        /// Returns the toggles in this group that are active.
        /// </summary>
        /// <returns>The active toggles in the group.</returns>
        /// <remarks>
        /// Toggles belonging to this group but are not active either because their GameObject is inactive or because the Toggle component is disabled, are not returned as part of the list.
        /// </remarks>
        public IEnumerable<MToggle> ActiveMToggles()
        {
            return m_MToggles.Where(x => x.isOn);
        }

        /// <summary>
        /// Returns the toggle that is the first in the list of active toggles.
        /// </summary>
        /// <returns>The first active toggle from m_Toggles</returns>
        /// <remarks>
        /// Get the active toggle for this group. As the group
        /// </remarks>
        public MToggle GetFirstActiveMToggle()
        {
            IEnumerable<MToggle> activeMToggles = ActiveMToggles();
            return activeMToggles.Count() > 0 ? activeMToggles.First() : null;
        }

        /// <summary>
        /// Switch all toggles off.
        /// </summary>
        /// <remarks>
        /// This method can be used to switch all toggles off, regardless of whether the allowSwitchOff property is enabled or not.
        /// </remarks>
        public void SetAllMTogglesOff(bool sendCallback = true)
        {
            bool oldAllowSwitchOff = m_AllowSwitchOff;
            m_AllowSwitchOff = true;

            if (sendCallback)
            {
                for (var i = 0; i < m_MToggles.Count; i++)
                    m_MToggles[i].isOn = false;
            }
            else
            {
                for (var i = 0; i < m_MToggles.Count; i++)
                    m_MToggles[i].SetIsOnWithoutNotify(false);
            }

            m_AllowSwitchOff = oldAllowSwitchOff;
        }
    }
}
