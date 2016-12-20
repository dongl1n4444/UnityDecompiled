namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Used by GUIUtility.GetControlID to inform the IMGUI system if a given control can get keyboard focus. This allows the IMGUI system to give focus appropriately when a user presses tab for cycling between controls.</para>
    /// </summary>
    public enum FocusType
    {
        /// <summary>
        /// <para>This control can receive keyboard focus.</para>
        /// </summary>
        Keyboard = 1,
        [Obsolete("FocusType.Native now behaves the same as FocusType.Passive in all OS cases. (UnityUpgradable) -> Passive", false)]
        Native = 0,
        /// <summary>
        /// <para>This control can not receive keyboard focus.</para>
        /// </summary>
        Passive = 2
    }
}

