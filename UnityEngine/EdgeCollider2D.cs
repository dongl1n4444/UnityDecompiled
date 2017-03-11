namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Collider for 2D physics representing an arbitrary set of connected edges (lines) defined by its vertices.</para>
    /// </summary>
    public sealed class EdgeCollider2D : Collider2D
    {
        /// <summary>
        /// <para>Reset to a single edge consisting of two points.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Reset();

        /// <summary>
        /// <para>Gets the number of edges.</para>
        /// </summary>
        public int edgeCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Controls the radius of all edges created by the collider.</para>
        /// </summary>
        public float edgeRadius { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Gets the number of points.</para>
        /// </summary>
        public int pointCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Get or set the points defining multiple continuous edges.</para>
        /// </summary>
        public Vector2[] points { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

