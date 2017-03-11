namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Rigidbody physics component for 2D sprites.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class Rigidbody2D : Component
    {
        [ExcludeFromDocs]
        public void AddForce(Vector2 force)
        {
            ForceMode2D mode = ForceMode2D.Force;
            INTERNAL_CALL_AddForce(this, ref force, mode);
        }

        /// <summary>
        /// <para>Apply a force to the rigidbody.</para>
        /// </summary>
        /// <param name="force">Components of the force in the X and Y axes.</param>
        /// <param name="mode">The method used to apply the specified force.</param>
        public void AddForce(Vector2 force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
        {
            INTERNAL_CALL_AddForce(this, ref force, mode);
        }

        [ExcludeFromDocs]
        public void AddForceAtPosition(Vector2 force, Vector2 position)
        {
            ForceMode2D mode = ForceMode2D.Force;
            INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
        }

        /// <summary>
        /// <para>Apply a force at a given position in space.</para>
        /// </summary>
        /// <param name="force">Components of the force in the X and Y axes.</param>
        /// <param name="position">Position in world space to apply the force.</param>
        /// <param name="mode">The method used to apply the specified force.</param>
        public void AddForceAtPosition(Vector2 force, Vector2 position, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
        {
            INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
        }

        [ExcludeFromDocs]
        public void AddRelativeForce(Vector2 relativeForce)
        {
            ForceMode2D force = ForceMode2D.Force;
            INTERNAL_CALL_AddRelativeForce(this, ref relativeForce, force);
        }

        /// <summary>
        /// <para>Adds a force to the rigidbody2D relative to its coordinate system.</para>
        /// </summary>
        /// <param name="relativeForce">Components of the force in the X and Y axes.</param>
        /// <param name="mode">The method used to apply the specified force.</param>
        public void AddRelativeForce(Vector2 relativeForce, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
        {
            INTERNAL_CALL_AddRelativeForce(this, ref relativeForce, mode);
        }

        [ExcludeFromDocs]
        public void AddTorque(float torque)
        {
            ForceMode2D force = ForceMode2D.Force;
            this.AddTorque(torque, force);
        }

        /// <summary>
        /// <para>Apply a torque at the rigidbody's centre of mass.</para>
        /// </summary>
        /// <param name="torque">Torque to apply.</param>
        /// <param name="mode">The force mode to use.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void AddTorque(float torque, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);
        [ExcludeFromDocs]
        public int Cast(Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_Cast(this, ref direction, results, positiveInfinity);
        }

        /// <summary>
        /// <para>All the Collider2D shapes attached to the Rigidbody2D are cast into the scene starting at each collider position ignoring the colliders attached to the same Rigidbody2D.</para>
        /// </summary>
        /// <param name="direction">Vector representing the direction to cast each Collider2D shape.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the shape(s).</param>
        /// <param name="contactFilter">Filter results defined by the contact filter.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance) => 
            INTERNAL_CALL_Cast(this, ref direction, results, distance);

        [ExcludeFromDocs]
        public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            return this.Cast(direction, contactFilter, results, positiveInfinity);
        }

        /// <summary>
        /// <para>All the Collider2D shapes attached to the Rigidbody2D are cast into the scene starting at each collider position ignoring the colliders attached to the same Rigidbody2D.</para>
        /// </summary>
        /// <param name="direction">Vector representing the direction to cast each Collider2D shape.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the shape(s).</param>
        /// <param name="contactFilter">Filter results defined by the contact filter.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance) => 
            this.Internal_Cast(direction, distance, contactFilter, results);

        /// <summary>
        /// <para>Calculates the minimum distance of this collider against all Collider2D attached to this Rigidbody2D.</para>
        /// </summary>
        /// <param name="collider">A collider used to calculate the minimum distance against all colliders attached to this Rigidbody2D.</param>
        /// <returns>
        /// <para>The minimum distance of collider against all colliders attached to this Rigidbody2D.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern ColliderDistance2D Distance(Collider2D collider);
        /// <summary>
        /// <para>Returns all Collider2D that are attached to this Rigidbody2D.</para>
        /// </summary>
        /// <param name="results">An array of Collider2D used to receive the results.</param>
        /// <returns>
        /// <para>Returns the number of Collider2D placed in the results array.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int GetAttachedColliders(Collider2D[] results);
        /// <summary>
        /// <para>Retrieves all contacts of the Rigidbody.</para>
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
        /// <para>Retrieves all contacts of the Rigidbody.</para>
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
        /// <para>Retrieves all contacts of the Rigidbody.</para>
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
        /// <para>Retrieves all contacts of the Rigidbody.</para>
        /// </summary>
        /// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
        /// <param name="colliders">An array of Collider2D used to receive the results.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Returns the number of contacts placed in the contacts array.</para>
        /// </returns>
        public int GetContacts(ContactFilter2D contactFilter, ContactPoint2D[] contacts) => 
            Physics2D.GetContacts(this, contactFilter, contacts);

        /// <summary>
        /// <para>Get a local space point given the point point in rigidBody global space.</para>
        /// </summary>
        /// <param name="point">The global space point to transform into local space.</param>
        public Vector2 GetPoint(Vector2 point)
        {
            Vector2 vector;
            Internal_GetPoint(this, point, out vector);
            return vector;
        }

        /// <summary>
        /// <para>The velocity of the rigidbody at the point Point in global space.</para>
        /// </summary>
        /// <param name="point">The global space point to calculate velocity for.</param>
        public Vector2 GetPointVelocity(Vector2 point)
        {
            Vector2 vector;
            Internal_GetPointVelocity(this, point, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Get a global space point given the point relativePoint in rigidBody local space.</para>
        /// </summary>
        /// <param name="relativePoint">The local space point to transform into global space.</param>
        public Vector2 GetRelativePoint(Vector2 relativePoint)
        {
            Vector2 vector;
            Internal_GetRelativePoint(this, relativePoint, out vector);
            return vector;
        }

        /// <summary>
        /// <para>The velocity of the rigidbody at the point Point in local space.</para>
        /// </summary>
        /// <param name="relativePoint">The local space point to calculate velocity for.</param>
        public Vector2 GetRelativePointVelocity(Vector2 relativePoint)
        {
            Vector2 vector;
            Internal_GetRelativePointVelocity(this, relativePoint, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Get a global space vector given the vector relativeVector in rigidBody local space.</para>
        /// </summary>
        /// <param name="relativeVector">The local space vector to transform into a global space vector.</param>
        public Vector2 GetRelativeVector(Vector2 relativeVector)
        {
            Vector2 vector;
            Internal_GetRelativeVector(this, relativeVector, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Get a local space vector given the vector vector in rigidBody global space.</para>
        /// </summary>
        /// <param name="vector">The global space vector to transform into a local space vector.</param>
        public Vector2 GetVector(Vector2 vector)
        {
            Vector2 vector2;
            Internal_GetVector(this, vector, out vector2);
            return vector2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddForce(Rigidbody2D self, ref Vector2 force, ForceMode2D mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddForceAtPosition(Rigidbody2D self, ref Vector2 force, ref Vector2 position, ForceMode2D mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddRelativeForce(Rigidbody2D self, ref Vector2 relativeForce, ForceMode2D mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_Cast(Rigidbody2D self, ref Vector2 direction, RaycastHit2D[] results, float distance);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_Internal_Cast(Rigidbody2D self, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, RaycastHit2D[] results);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_GetPoint(Rigidbody2D rigidbody, ref Vector2 point, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_GetPointVelocity(Rigidbody2D rigidbody, ref Vector2 point, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_GetRelativePoint(Rigidbody2D rigidbody, ref Vector2 relativePoint, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_GetRelativePointVelocity(Rigidbody2D rigidbody, ref Vector2 relativePoint, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_GetRelativeVector(Rigidbody2D rigidbody, ref Vector2 relativeVector, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_GetVector(Rigidbody2D rigidbody, ref Vector2 vector, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Internal_IsTouching(Rigidbody2D self, Collider2D collider, ref ContactFilter2D contactFilter);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsTouching(Rigidbody2D self, ref ContactFilter2D contactFilter);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MovePosition(Rigidbody2D self, ref Vector2 position);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MoveRotation(Rigidbody2D self, float angle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_OverlapCollider(Rigidbody2D self, ref ContactFilter2D contactFilter, Collider2D[] results);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_OverlapPoint(Rigidbody2D self, ref Vector2 point);
        private int Internal_Cast(Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results) => 
            INTERNAL_CALL_Internal_Cast(this, ref direction, distance, ref contactFilter, results);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_centerOfMass(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_position(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_velocity(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_worldCenterOfMass(out Vector2 value);
        private static void Internal_GetPoint(Rigidbody2D rigidbody, Vector2 point, out Vector2 value)
        {
            INTERNAL_CALL_Internal_GetPoint(rigidbody, ref point, out value);
        }

        private static void Internal_GetPointVelocity(Rigidbody2D rigidbody, Vector2 point, out Vector2 value)
        {
            INTERNAL_CALL_Internal_GetPointVelocity(rigidbody, ref point, out value);
        }

        private static void Internal_GetRelativePoint(Rigidbody2D rigidbody, Vector2 relativePoint, out Vector2 value)
        {
            INTERNAL_CALL_Internal_GetRelativePoint(rigidbody, ref relativePoint, out value);
        }

        private static void Internal_GetRelativePointVelocity(Rigidbody2D rigidbody, Vector2 relativePoint, out Vector2 value)
        {
            INTERNAL_CALL_Internal_GetRelativePointVelocity(rigidbody, ref relativePoint, out value);
        }

        private static void Internal_GetRelativeVector(Rigidbody2D rigidbody, Vector2 relativeVector, out Vector2 value)
        {
            INTERNAL_CALL_Internal_GetRelativeVector(rigidbody, ref relativeVector, out value);
        }

        private static void Internal_GetVector(Rigidbody2D rigidbody, Vector2 vector, out Vector2 value)
        {
            INTERNAL_CALL_Internal_GetVector(rigidbody, ref vector, out value);
        }

        private bool Internal_IsTouching(Collider2D collider, ContactFilter2D contactFilter) => 
            INTERNAL_CALL_Internal_IsTouching(this, collider, ref contactFilter);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_centerOfMass(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_position(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_velocity(ref Vector2 value);
        /// <summary>
        /// <para>Is the rigidbody "awake"?</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool IsAwake();
        /// <summary>
        /// <para>Is the rigidbody "sleeping"?</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool IsSleeping();
        /// <summary>
        /// <para>Checks whether the rigidbody is touching other colliders.</para>
        /// </summary>
        /// <param name="collider">The collider to check if it is touching any of the collider(s) attached to this rigidbody.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Whether the collider is touching any of the collider(s) attached to this rigidbody or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool IsTouching(Collider2D collider);
        /// <summary>
        /// <para>Checks whether the rigidbody is touching other colliders.</para>
        /// </summary>
        /// <param name="collider">The collider to check if it is touching any of the collider(s) attached to this rigidbody.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Whether the collider is touching any of the collider(s) attached to this rigidbody or not.</para>
        /// </returns>
        public bool IsTouching(ContactFilter2D contactFilter) => 
            INTERNAL_CALL_IsTouching(this, ref contactFilter);

        /// <summary>
        /// <para>Checks whether the rigidbody is touching other colliders.</para>
        /// </summary>
        /// <param name="collider">The collider to check if it is touching any of the collider(s) attached to this rigidbody.</param>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
        /// <returns>
        /// <para>Whether the collider is touching any of the collider(s) attached to this rigidbody or not.</para>
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
        /// <para>Checks whether any of the collider(s) attached to this rigidbody are touching any colliders on the specified layerMask or not.</para>
        /// </summary>
        /// <param name="layerMask">Any colliders on any of these layers count as touching.</param>
        /// <returns>
        /// <para>Whether any of the collider(s) attached to this rigidbody are touching any colliders on the specified layerMask or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask);
        /// <summary>
        /// <para>Moves the rigidbody to position.</para>
        /// </summary>
        /// <param name="position">The new position for the Rigidbody object.</param>
        public void MovePosition(Vector2 position)
        {
            INTERNAL_CALL_MovePosition(this, ref position);
        }

        /// <summary>
        /// <para>Rotates the rigidbody to angle (given in degrees).</para>
        /// </summary>
        /// <param name="angle">The new rotation angle for the Rigidbody object.</param>
        public void MoveRotation(float angle)
        {
            INTERNAL_CALL_MoveRotation(this, angle);
        }

        /// <summary>
        /// <para>Get a list of all colliders that overlap all colliders attached to this Rigidbody2D.</para>
        /// </summary>
        /// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
        /// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
        /// <returns>
        /// <para>Returns the number of results placed in the results array.</para>
        /// </returns>
        public int OverlapCollider(ContactFilter2D contactFilter, Collider2D[] results) => 
            INTERNAL_CALL_OverlapCollider(this, ref contactFilter, results);

        /// <summary>
        /// <para>Check if any of the Rigidbody2D colliders overlap a point in space.</para>
        /// </summary>
        /// <param name="point">A point in world space.</param>
        /// <returns>
        /// <para>Whether the point overlapped any of the Rigidbody2D colliders.</para>
        /// </returns>
        public bool OverlapPoint(Vector2 point) => 
            INTERNAL_CALL_OverlapPoint(this, ref point);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetDragBehaviour(bool dragged);
        /// <summary>
        /// <para>Make the rigidbody "sleep".</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Sleep();
        /// <summary>
        /// <para>Disables the "sleeping" state of a rigidbody.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void WakeUp();

        /// <summary>
        /// <para>Coefficient of angular drag.</para>
        /// </summary>
        public float angularDrag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Angular velocity in degrees per second.</para>
        /// </summary>
        public float angularVelocity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns the number of Collider2D attached to this Rigidbody2D.</para>
        /// </summary>
        public int attachedColliderCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The physical behaviour type of the Rigidbody2D.</para>
        /// </summary>
        public RigidbodyType2D bodyType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The center of mass of the rigidBody in local space.</para>
        /// </summary>
        public Vector2 centerOfMass
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_centerOfMass(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_centerOfMass(ref value);
            }
        }

        /// <summary>
        /// <para>The method used by the physics engine to check if two objects have collided.</para>
        /// </summary>
        public CollisionDetectionMode2D collisionDetectionMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Controls which degrees of freedom are allowed for the simulation of this Rigidbody2D.</para>
        /// </summary>
        public RigidbodyConstraints2D constraints { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Coefficient of drag.</para>
        /// </summary>
        public float drag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should the rigidbody be prevented from rotating?</para>
        /// </summary>
        [Obsolete("The fixedAngle is no longer supported. Use constraints instead.")]
        public bool fixedAngle { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Controls whether physics will change the rotation of the object.</para>
        /// </summary>
        public bool freezeRotation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The degree to which this object is affected by gravity.</para>
        /// </summary>
        public float gravityScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The rigidBody rotational inertia.</para>
        /// </summary>
        public float inertia { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Physics interpolation used between updates.</para>
        /// </summary>
        public RigidbodyInterpolation2D interpolation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should this rigidbody be taken out of physics control?</para>
        /// </summary>
        public bool isKinematic
        {
            get => 
                (this.bodyType == RigidbodyType2D.Kinematic);
            set
            {
                this.bodyType = !value ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
            }
        }

        /// <summary>
        /// <para>Mass of the rigidbody.</para>
        /// </summary>
        public float mass { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The position of the rigidbody.</para>
        /// </summary>
        public Vector2 position
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_position(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_position(ref value);
            }
        }

        /// <summary>
        /// <para>The rotation of the rigdibody.</para>
        /// </summary>
        public float rotation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The PhysicsMaterial2D that is applied to all Collider2D attached to this Rigidbody2D.</para>
        /// </summary>
        public PhysicsMaterial2D sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Indicates whether the rigid body should be simulated or not by the physics system.</para>
        /// </summary>
        public bool simulated { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The sleep state that the rigidbody will initially be in.</para>
        /// </summary>
        public RigidbodySleepMode2D sleepMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should the total rigid-body mass be automatically calculated from the Collider2D.density of attached colliders?</para>
        /// </summary>
        public bool useAutoMass { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should kinematickinematic and kinematicstatic collisions be allowed?</para>
        /// </summary>
        public bool useFullKinematicContacts { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Linear velocity of the rigidbody.</para>
        /// </summary>
        public Vector2 velocity
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_velocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_velocity(ref value);
            }
        }

        /// <summary>
        /// <para>Gets the center of mass of the rigidBody in global space.</para>
        /// </summary>
        public Vector2 worldCenterOfMass
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_worldCenterOfMass(out vector);
                return vector;
            }
        }
    }
}

