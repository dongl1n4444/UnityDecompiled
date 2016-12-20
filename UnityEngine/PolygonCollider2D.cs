namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Collider for 2D physics representing an arbitrary polygon defined by its vertices.</para>
    /// </summary>
    public sealed class PolygonCollider2D : Collider2D
    {
        [ExcludeFromDocs]
        public void CreatePrimitive(int sides)
        {
            Vector2 zero = Vector2.zero;
            Vector2 one = Vector2.one;
            INTERNAL_CALL_CreatePrimitive(this, sides, ref one, ref zero);
        }

        [ExcludeFromDocs]
        public void CreatePrimitive(int sides, Vector2 scale)
        {
            Vector2 zero = Vector2.zero;
            INTERNAL_CALL_CreatePrimitive(this, sides, ref scale, ref zero);
        }

        /// <summary>
        /// <para>Creates as regular primitive polygon with the specified number of sides.</para>
        /// </summary>
        /// <param name="sides">The number of sides in the polygon.  This must be greater than two.</param>
        /// <param name="scale">The X/Y scale of the polygon.  These must be greater than zero.</param>
        /// <param name="offset">The X/Y offset of the polygon.</param>
        public void CreatePrimitive(int sides, [DefaultValue("Vector2.one")] Vector2 scale, [DefaultValue("Vector2.zero")] Vector2 offset)
        {
            INTERNAL_CALL_CreatePrimitive(this, sides, ref scale, ref offset);
        }

        /// <summary>
        /// <para>Get a path from the polygon by its index.</para>
        /// </summary>
        /// <param name="index">The index of the path to retrieve.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Vector2[] GetPath(int index);
        /// <summary>
        /// <para>Return the total number of points in the polygon in all paths.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetTotalPointCount();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_CreatePrimitive(PolygonCollider2D self, int sides, ref Vector2 scale, ref Vector2 offset);
        /// <summary>
        /// <para>Define a path by its constituent points.</para>
        /// </summary>
        /// <param name="index">Index of the path to set.</param>
        /// <param name="points">Points that define the path.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetPath(int index, Vector2[] points);

        /// <summary>
        /// <para>The number of paths in the polygon.</para>
        /// </summary>
        public int pathCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Corner points that define the collider's shape in local space.</para>
        /// </summary>
        public Vector2[] points { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

