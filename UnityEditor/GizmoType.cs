namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Determines how a gizmo is drawn or picked in the Unity editor.</para>
    /// </summary>
    public enum GizmoType
    {
        /// <summary>
        /// <para>Draw the gizmo if it is active (shown in the inspector).</para>
        /// </summary>
        Active = 8,
        /// <summary>
        /// <para>Draw the gizmo if it is selected or it is a child/descendent of the selected.</para>
        /// </summary>
        InSelectionHierarchy = 0x10,
        /// <summary>
        /// <para>Draw the gizmo if it is not selected.</para>
        /// </summary>
        NonSelected = 0x20,
        /// <summary>
        /// <para>Draw the gizmo if it is not selected and also no parent/ancestor is selected.</para>
        /// </summary>
        NotInSelectionHierarchy = 2,
        [Obsolete("Use NotInSelectionHierarchy instead (UnityUpgradable) -> NotInSelectionHierarchy")]
        NotSelected = -127,
        /// <summary>
        /// <para>The gizmo can be picked in the editor.</para>
        /// </summary>
        Pickable = 1,
        /// <summary>
        /// <para>Draw the gizmo if it is selected.</para>
        /// </summary>
        Selected = 4,
        [Obsolete("Use InSelectionHierarchy instead (UnityUpgradable) -> InSelectionHierarchy")]
        SelectedOrChild = -127
    }
}

