namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Parent class for collider types used with 2D gameplay.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class Collider2D : Behaviour
    {
        [ExcludeFromDocs]
        public int Cast(Vector2 direction, RaycastHit2D[] results)
        {
            bool ignoreSiblingColliders = true;
            float positiveInfinity = float.PositiveInfinity;
            return this.Cast(direction, results, positiveInfinity, ignoreSiblingColliders);
        }

        [ExcludeFromDocs]
        public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
        {
            bool ignoreSiblingColliders = true;
            float positiveInfinity = float.PositiveInfinity;
            return this.Cast(direction, contactFilter, results, positiveInfinity, ignoreSiblingColliders);
        }

        /// <summary>
        /// <para>Casts the collider shape into the scene starting at the collider position ignoring the collider itself.</para>
        /// </summary>
        /// <param name="direction">Vector representing the direction to cast the shape.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="ignoreSiblingColliders">Should colliders attached to the same Rigidbody2D (known as sibling colliders) be ignored?</param>
        /// <param name="contactFilter">Filter results defined by the contact filter.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        [ExcludeFromDocs]
        public int Cast(Vector2 direction, RaycastHit2D[] results, float distance)
        {
            bool ignoreSiblingColliders = true;
            return this.Cast(direction, results, distance, ignoreSiblingColliders);
        }

        /// <summary>
        /// <para>Casts the collider shape into the scene starting at the collider position ignoring the collider itself.</para>
        /// </summary>
        /// <param name="direction">Vector representing the direction to cast the shape.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="ignoreSiblingColliders">Should colliders attached to the same Rigidbody2D (known as sibling colliders) be ignored?</param>
        /// <param name="contactFilter">Filter results defined by the contact filter.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("true")] bool ignoreSiblingColliders)
        {
            ContactFilter2D contactFilter = new ContactFilter2D {
                useTriggers = Physics2D.queriesHitTriggers
            };
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(base.gameObject.layer));
            return this.Internal_Cast(direction, contactFilter, distance, ignoreSiblingColliders, results);
        }

        [ExcludeFromDocs]
        public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance)
        {
            bool ignoreSiblingColliders = true;
            return this.Cast(direction, contactFilter, results, distance, ignoreSiblingColliders);
        }

        /// <summary>
        /// <para>Casts the collider shape into the scene starting at the collider position ignoring the collider itself.</para>
        /// </summary>
        /// <param name="direction">Vector representing the direction to cast the shape.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="ignoreSiblingColliders">Should colliders attached to the same Rigidbody2D (known as sibling colliders) be ignored?</param>
        /// <param name="contactFilter">Filter results defined by the contact filter.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("true")] bool ignoreSiblingColliders) => 
            this.Internal_Cast(direction, contactFilter, distance, ignoreSiblingColliders, results);

        /// <summary>
        /// <para>Calculates the minimum separation of this collider against another collider.</para>
        /// </summary>
        /// <param name="collider">A collider used to calculate the minimum separation against this collider.</param>
        /// <returns>
        /// <para>The minimum separation of collider and this collider.</para>
        /// </returns>
        public ColliderDistance2D Distance(Collider2D collider) => 
            Physics2D.Distance(this, collider);

        /// <summary>
        /// <para>Retrieves contacts for the collider based on the parameters passed.</para>
        /// </summary>
        /// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
        /// <param name="colliders">An array of Collider2D used to receive the results.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Returns the number of contacts placed in the contacts array.</para>
        /// </returns>
        public int GetContacts(Collider2D[] colliders)
        {
            ContactFilter2D filterd = new ContactFilter2D();
            return Physics2D.GetContacts(this, filterd.NoFilter(), colliders);
        }

        /// <summary>
        /// <para>Retrieves contacts for the collider based on the parameters passed.</para>
        /// </summary>
        /// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
        /// <param name="colliders">An array of Collider2D used to receive the results.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Returns the number of contacts placed in the contacts array.</para>
        /// </returns>
        public int GetContacts(ContactPoint2D[] contacts)
        {
            ContactFilter2D filterd = new ContactFilter2D();
            return Physics2D.GetContacts(this, filterd.NoFilter(), contacts);
        }

        /// <summary>
        /// <para>Retrieves contacts for the collider based on the parameters passed.</para>
        /// </summary>
        /// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
        /// <param name="colliders">An array of Collider2D used to receive the results.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Returns the number of contacts placed in the contacts array.</para>
        /// </returns>
        public int GetContacts(ContactFilter2D contactFilter, Collider2D[] colliders) => 
            Physics2D.GetContacts(this, contactFilter, colliders);

        /// <summary>
        /// <para>Retrieves contacts for the collider based on the parameters passed.</para>
        /// </summary>
        /// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
        /// <param name="colliders">An array of Collider2D used to receive the results.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Returns the number of contacts placed in the contacts array.</para>
        /// </returns>
        public int GetContacts(ContactFilter2D contactFilter, ContactPoint2D[] contacts) => 
            Physics2D.GetContacts(this, contactFilter, contacts);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_Internal_Cast(Collider2D self, ref Vector2 direction, ref ContactFilter2D contactFilter, float distance, bool ignoreSiblingColliders, RaycastHit2D[] results);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Internal_IsTouching(Collider2D self, Collider2D collider, ref ContactFilter2D contactFilter);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_Internal_Raycast(Collider2D self, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, RaycastHit2D[] results);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsTouching(Collider2D self, ref ContactFilter2D contactFilter);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_OverlapPoint(Collider2D self, ref Vector2 point);
        private int Internal_Cast(Vector2 direction, ContactFilter2D contactFilter, float distance, bool ignoreSiblingColliders, RaycastHit2D[] results) => 
            INTERNAL_CALL_Internal_Cast(this, ref direction, ref contactFilter, distance, ignoreSiblingColliders, results);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_offset(out Vector2 value);
        private bool Internal_IsTouching(Collider2D collider, ContactFilter2D contactFilter) => 
            INTERNAL_CALL_Internal_IsTouching(this, collider, ref contactFilter);

        private int Internal_Raycast(Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results) => 
            INTERNAL_CALL_Internal_Raycast(this, ref direction, distance, ref contactFilter, results);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_offset(ref Vector2 value);
        /// <summary>
        /// <para>Checks whether the collider is touching other colliders.</para>
        /// </summary>
        /// <param name="collider">The collider to check if it is touching this collider.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Whether the collider is touching this collider or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool IsTouching(Collider2D collider);
        /// <summary>
        /// <para>Checks whether the collider is touching other colliders.</para>
        /// </summary>
        /// <param name="collider">The collider to check if it is touching this collider.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Whether the collider is touching this collider or not.</para>
        /// </returns>
        public bool IsTouching(ContactFilter2D contactFilter) => 
            INTERNAL_CALL_IsTouching(this, ref contactFilter);

        /// <summary>
        /// <para>Checks whether the collider is touching other colliders.</para>
        /// </summary>
        /// <param name="collider">The collider to check if it is touching this collider.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Whether the collider is touching this collider or not.</para>
        /// </returns>
        public bool IsTouching(Collider2D collider, ContactFilter2D contactFilter) => 
            this.Internal_IsTouching(collider, contactFilter);

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
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask);
        /// <summary>
        /// <para>Get a list of all colliders that overlap this collider.</para>
        /// </summary>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
        /// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
        /// <returns>
        /// <para>Returns the number of results placed in the results array.</para>
        /// </returns>
        public int OverlapCollider(ContactFilter2D contactFilter, Collider2D[] results) => 
            Physics2D.OverlapCollider(this, contactFilter, results);

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
            return this.Raycast(direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            return this.Raycast(direction, contactFilter, results, positiveInfinity);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -1;
            return this.Raycast(direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return this.Raycast(direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
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
        /// <param name="contactFilter">Filter results defined by the contact filter.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public int Raycast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance) => 
            this.Internal_Raycast(direction, distance, contactFilter, results);

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return this.Raycast(direction, results, distance, layerMask, minDepth, positiveInfinity);
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
        /// <param name="contactFilter">Filter results defined by the contact filter.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public int Raycast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("Physics2D.AllLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
            return this.Internal_Raycast(direction, distance, contactFilter, results);
        }

        /// <summary>
        /// <para>The Rigidbody2D attached to the Collider2D's GameObject.</para>
        /// </summary>
        public Rigidbody2D attachedRigidbody { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Get the bounciness used by the collider.</para>
        /// </summary>
        public float bounciness { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

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
        /// <para>Get the CompositeCollider2D that is available to be attached to the collider.</para>
        /// </summary>
        public CompositeCollider2D composite { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal bool compositeCapable { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The density of the collider used to calculate its mass (when auto mass is enabled).</para>
        /// </summary>
        public float density { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal ColliderErrorState2D errorState { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Get the friction used by the collider.</para>
        /// </summary>
        public float friction { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is this collider configured as a trigger?</para>
        /// </summary>
        public bool isTrigger { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

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
        public int shapeCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The PhysicsMaterial2D that is applied to this collider.</para>
        /// </summary>
        public PhysicsMaterial2D sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Sets whether the Collider will be used or not used by a CompositeCollider2D.</para>
        /// </summary>
        public bool usedByComposite { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Whether the collider is used by an attached effector or not.</para>
        /// </summary>
        public bool usedByEffector { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

