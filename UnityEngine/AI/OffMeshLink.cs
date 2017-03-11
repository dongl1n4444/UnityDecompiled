namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// <para>Link allowing movement outside the planar navigation mesh.</para>
    /// </summary>
    [MovedFrom("UnityEngine")]
    public sealed class OffMeshLink : Behaviour
    {
        /// <summary>
        /// <para>Explicitly update the link endpoints.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void UpdatePositions();

        /// <summary>
        /// <para>Is link active.</para>
        /// </summary>
        public bool activated { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>NavMesh area index for this OffMeshLink component.</para>
        /// </summary>
        public int area { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Automatically update endpoints.</para>
        /// </summary>
        public bool autoUpdatePositions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Can link be traversed in both directions.</para>
        /// </summary>
        public bool biDirectional { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Modify pathfinding cost for the link.</para>
        /// </summary>
        public float costOverride { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The transform representing link end position.</para>
        /// </summary>
        public Transform endTransform { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>NavMeshLayer for this OffMeshLink component.</para>
        /// </summary>
        [Obsolete("Use area instead.")]
        public int navMeshLayer { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is link occupied. (Read Only)</para>
        /// </summary>
        public bool occupied { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The transform representing link start position.</para>
        /// </summary>
        public Transform startTransform { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

