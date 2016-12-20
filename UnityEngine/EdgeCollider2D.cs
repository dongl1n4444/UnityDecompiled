namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Collider for 2D physics representing an arbitrary set of connected edges (lines) defined by its vertices.</para>
    /// </summary>
    public sealed class EdgeCollider2D : Collider2D
    {
        /// <summary>
        /// <para>Reset to a single edge consisting of two points.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Reset();

        /// <summary>
        /// <para>Gets the number of edges.</para>
        /// </summary>
        public int edgeCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Gets the number of points.</para>
        /// </summary>
        public int pointCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Get or set the points defining multiple continuous edges.</para>
        /// </summary>
        public Vector2[] points { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

