namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Parent class for collider types used with 2D gameplay.</para>
    /// </summary>
    public class Collider2D : Behaviour
    {
        [ExcludeFromDocs]
        public int Cast(Vector2 direction, RaycastHit2D[] results)
        {
            bool ignoreSiblingColliders = true;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_Cast(this, ref direction, results, positiveInfinity, ignoreSiblingColliders);
        }

        /// <summary>
        /// <para>Casts the collider shape into the scene starting at the collider position ignoring the collider itself.</para>
        /// </summary>
        /// <param name="direction">Vector representing the direction to cast the shape.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="ignoreSiblingColliders">Should colliders attached to the same Rigidbody2D (known as sibling colliders) be ignored?</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        [ExcludeFromDocs]
        public int Cast(Vector2 direction, RaycastHit2D[] results, float distance)
        {
            bool ignoreSiblingColliders = true;
            return INTERNAL_CALL_Cast(this, ref direction, results, distance, ignoreSiblingColliders);
        }

        public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("true")] bool ignoreSiblingColliders) => 
            INTERNAL_CALL_Cast(this, ref direction, results, distance, ignoreSiblingColliders);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_Cast(Collider2D self, ref Vector2 direction, RaycastHit2D[] results, float distance, bool ignoreSiblingColliders);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_OverlapPoint(Collider2D self, ref Vector2 point);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_Raycast(Collider2D self, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_offset(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_offset(ref Vector2 value);
        /// <summary>
        /// <para>Check whether this collider is touching the collider or not.</para>
        /// </summary>
        /// <param name="collider">The collider to check if it is touching this collider.</param>
        /// <returns>
        /// <para>Whether the collider is touching this collider or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsTouching(Collider2D collider);
        [ExcludeFromDocs]
        public bool IsTouchingLayers()
        {
            int layerMask = -1;
            return this.IsTouchingLayers(layerMask);
        }

        /// <summary>
        /// <para>Checks whether this collider is touching any colliders on the specified layerMask or not.</para>
        /// </summary>
        /// <param name="layerMask">Any colliders on any of these layers count as touching.</param>
        /// <returns>
        /// <para>Whether this collider is touching any collider on the specified layerMask or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask);
        /// <summary>
        /// <para>Check if a collider overlaps a point in space.</para>
        /// </summary>
        /// <param name="point">A point in world space.</param>
        /// <returns>
        /// <para>Does point overlap the collider?</para>
        /// </returns>
        public bool OverlapPoint(Vector2 point) => 
            INTERNAL_CALL_OverlapPoint(this, ref point);

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -1;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -1;
            return INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a ray into the scene starting at the collider position ignoring the collider itself.</para>
        /// </summary>
        /// <param name="direction">Vector representing the direction of the ray.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the ray.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public int Raycast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("Physics2D.AllLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth) => 
            INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, minDepth, maxDepth);

        /// <summary>
        /// <para>The Rigidbody2D attached to the Collider2D's GameObject.</para>
        /// </summary>
        public Rigidbody2D attachedRigidbody { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Get the bounciness used by the collider.</para>
        /// </summary>
        public float bounciness { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The world space bounding area of the collider.</para>
        /// </summary>
        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
        }

        /// <summary>
        /// <para>The density of the collider used to calculate its mass (when auto mass is enabled).</para>
        /// </summary>
        public float density { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal ColliderErrorState2D errorState { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Get the friction used by the collider.</para>
        /// </summary>
        public float friction { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is this collider configured as a trigger?</para>
        /// </summary>
        public bool isTrigger { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The local offset of the collider geometry.</para>
        /// </summary>
        public Vector2 offset
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_offset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_offset(ref value);
            }
        }

        /// <summary>
        /// <para>The number of separate shaped regions in the collider.</para>
        /// </summary>
        public int shapeCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The PhysicsMaterial2D that is applied to this collider.</para>
        /// </summary>
        public PhysicsMaterial2D sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Whether the collider is used by an attached effector or not.</para>
        /// </summary>
        public bool usedByEffector { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

