namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A Collider that can merge other Colliders together.</para>
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class CompositeCollider2D : Collider2D
    {
        /// <summary>
        /// <para>Regenerates the Composite Collider geometry.</para>
        /// </summary>
        public void GenerateGeometry()
        {
            INTERNAL_CALL_GenerateGeometry(this);
        }

        /// <summary>
        /// <para>Gets a path from the Collider by its index.</para>
        /// </summary>
        /// <param name="index">The index of the path from 0 to pathCount.</param>
        /// <param name="points">An ordered array of the vertices or points in the selected path.</param>
        /// <returns>
        /// <para>Returns the number of points placed in the points array.</para>
        /// </returns>
        public int GetPath(int index, Vector2[] points)
        {
            if ((index < 0) || (index >= this.pathCount))
            {
                throw new ArgumentOutOfRangeException("index", $"Path index {index} must be in the range of 0 to {this.pathCount - 1}.");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            return this.Internal_GetPath(index, points);
        }

        /// <summary>
        /// <para>Gets the number of points in the specified path from the Collider by its index.</para>
        /// </summary>
        /// <param name="index">The index of the path from 0 to pathCount.</param>
        /// <returns>
        /// <para>Returns the number of points in the path specified by index.</para>
        /// </returns>
        public int GetPathPointCount(int index)
        {
            if ((index < 0) || (index >= this.pathCount))
            {
                throw new ArgumentOutOfRangeException("index", $"Path index {index} must be in the range of 0 to {this.pathCount - 1}.");
            }
            return this.Internal_GetPathPointCount(index);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GenerateGeometry(CompositeCollider2D self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern int Internal_GetPath(int index, Vector2[] points);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern int Internal_GetPathPointCount(int index);

        /// <summary>
        /// <para>Controls the radius of all edges created by the Collider.</para>
        /// </summary>
        public float edgeRadius { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Specifies when to generate the Composite Collider geometry.</para>
        /// </summary>
        public GenerationType generationType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Specifies the type of geometry the Composite Collider should generate.</para>
        /// </summary>
        public GeometryType geometryType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The number of paths in the Collider.</para>
        /// </summary>
        public int pathCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Gets the total number of points in all the paths within the Collider.</para>
        /// </summary>
        public int pointCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Controls the minimum distance allowed between generated vertices.</para>
        /// </summary>
        public float vertexDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Specifies when to generate the Composite Collider geometry.</para>
        /// </summary>
        public enum GenerationType
        {
            Synchronous,
            Manual
        }

        /// <summary>
        /// <para>Specifies the type of geometry the Composite Collider generates.</para>
        /// </summary>
        public enum GeometryType
        {
            Outlines,
            Polygons
        }
    }
}

