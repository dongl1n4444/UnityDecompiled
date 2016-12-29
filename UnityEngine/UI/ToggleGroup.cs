namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// <para>A component that represents a group of Toggles.</para>
    /// </summary>
    [AddComponentMenu("UI/Toggle Group", 0x20), DisallowMultipleComponent]
    public class ToggleGroup : UIBehaviour
    {
        [CompilerGenerated]
        private static Predicate<Toggle> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Toggle, bool> <>f__am$cache1;
        [SerializeField]
        private bool m_AllowSwitchOff = false;
        private List<Toggle> m_Toggles = new List<Toggle>();

        protected ToggleGroup()
        {
        }

        /// <summary>
        /// <para>Returns the toggles in this group that are active.</para>
        /// </summary>
        /// <returns>
        /// <para>The active toggles in the group.</para>
        /// </returns>
        public IEnumerable<Toggle> ActiveToggles()
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<Toggle, bool>(null, (IntPtr) <ActiveToggles>m__1);
            }
            return Enumerable.Where<Toggle>(this.m_Toggles, <>f__am$cache1);
        }

        /// <summary>
        /// <para>Are any of the toggles on?</para>
        /// </summary>
        public bool AnyTogglesOn()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => x.isOn;
            }
            return (this.m_Toggles.Find(<>f__am$cache0) != null);
        }

        /// <summary>
        /// <para>Notify the group that the given toggle is enabled.</para>
        /// </summary>
        /// <param name="toggle"></param>
        public void NotifyToggleOn(Toggle toggle)
        {
            this.ValidateToggleIsInGroup(toggle);
            for (int i = 0; i < this.m_Toggles.Count; i++)
            {
                if (this.m_Toggles[i] != toggle)
                {
                    this.m_Toggles[i].isOn = false;
                }
            }
        }

        /// <summary>
        /// <para>Register a toggle with the group.</para>
        /// </summary>
        /// <param name="toggle">To register.</param>
        public void RegisterToggle(Toggle toggle)
        {
            if (!this.m_Toggles.Contains(toggle))
            {
                this.m_Toggles.Add(toggle);
            }
        }

        /// <summary>
        /// <para>Switch all toggles off.</para>
        /// </summary>
        public void SetAllTogglesOff()
        {
            bool allowSwitchOff = this.m_AllowSwitchOff;
            this.m_AllowSwitchOff = true;
            for (int i = 0; i < this.m_Toggles.Count; i++)
            {
                this.m_Toggles[i].isOn = false;
            }
            this.m_AllowSwitchOff = allowSwitchOff;
        }

        /// <summary>
        /// <para>Toggle to unregister.</para>
        /// </summary>
        /// <param name="toggle">Unregister toggle.</param>
        public void UnregisterToggle(Toggle toggle)
        {
            if (this.m_Toggles.Contains(toggle))
            {
                this.m_Toggles.Remove(toggle);
            }
        }

        private void ValidateToggleIsInGroup(Toggle toggle)
        {
            if ((toggle == null) || !this.m_Toggles.Contains(toggle))
            {
                throw new ArgumentException($"Toggle {toggle} is not part of ToggleGroup {this}");
            }
        }

        /// <summary>
        /// <para>Is it allowed that no toggle is switched on?</para>
        /// </summary>
        public bool allowSwitchOff
        {
            get => 
                this.m_AllowSwitchOff;
            set
            {
                this.m_AllowSwitchOff = value;
            }
        }
    }
}

