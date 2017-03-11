namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A Canvas placable element that can be used to modify children Alpha, Raycasting, Enabled state.</para>
    /// </summary>
    public sealed class CanvasGroup : Component, ICanvasRaycastFilter
    {
        /// <summary>
        /// <para>Returns true if the Group allows raycasts.</para>
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="eventCamera"></param>
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) => 
            this.blocksRaycasts;

        /// <summary>
        /// <para>Set the alpha of the group.</para>
        /// </summary>
        public float alpha { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Does this group block raycasting (allow collision).</para>
        /// </summary>
        public bool blocksRaycasts { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should the group ignore parent groups?</para>
        /// </summary>
        public bool ignoreParentGroups { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is the group interactable (are the elements beneath the group enabled).</para>
        /// </summary>
        public bool interactable { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

