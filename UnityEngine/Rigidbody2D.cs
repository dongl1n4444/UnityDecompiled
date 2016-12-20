namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Rigidbody physics component for 2D sprites.</para>
    /// </summary>
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
        [MethodImpl(MethodImplOptions.InternalCall)]
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
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
        {
            return INTERNAL_CALL_Cast(this, ref direction, results, distance);
        }

        /// <summary>
        /// <para>Get a local space point given the point point in rigidBody global space.</para>
        /// </summary>
        /// <param name="point">The global space point to transform into local space.</param>
        public Vector2 GetPoint(Vector2 point)
        {
            Vector2 vector;
            Rigidbody2D_CUSTOM_INTERNAL_GetPoint(this, point, out vector);
            return vector;
        }

        /// <summary>
        /// <para>The velocity of the rigidbody at the point Point in global space.</para>
        /// </summary>
        /// <param name="point">The global space point to calculate velocity for.</param>
        public Vector2 GetPointVelocity(Vector2 point)
        {
            Vector2 vector;
            Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(this, point, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Get a global space point given the point relativePoint in rigidBody local space.</para>
        /// </summary>
        /// <param name="relativePoint">The local space point to transform into global space.</param>
        public Vector2 GetRelativePoint(Vector2 relativePoint)
        {
            Vector2 vector;
            Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(this, relativePoint, out vector);
            return vector;
        }

        /// <summary>
        /// <para>The velocity of the rigidbody at the point Point in local space.</para>
        /// </summary>
        /// <param name="relativePoint">The local space point to calculate velocity for.</param>
        public Vector2 GetRelativePointVelocity(Vector2 relativePoint)
        {
            Vector2 vector;
            Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(this, relativePoint, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Get a global space vector given the vector relativeVector in rigidBody local space.</para>
        /// </summary>
        /// <param name="relativeVector">The local space vector to transform into a global space vector.</param>
        public Vector2 GetRelativeVector(Vector2 relativeVector)
        {
            Vector2 vector;
            Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(this, relativeVector, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Get a local space vector given the vector vector in rigidBody global space.</para>
        /// </summary>
        /// <param name="vector">The global space vector to transform into a local space vector.</param>
        public Vector2 GetVector(Vector2 vector)
        {
            Vector2 vector2;
            Rigidbody2D_CUSTOM_INTERNAL_GetVector(this, vector, out vector2);
            return vector2;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_AddForce(Rigidbody2D self, ref Vector2 force, ForceMode2D mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_AddForceAtPosition(Rigidbody2D self, ref Vector2 force, ref Vector2 position, ForceMode2D mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_AddRelativeForce(Rigidbody2D self, ref Vector2 relativeForce, ForceMode2D mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_Cast(Rigidbody2D self, ref Vector2 direction, RaycastHit2D[] results, float distance);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_MovePosition(Rigidbody2D self, ref Vector2 position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_MoveRotation(Rigidbody2D self, float angle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_OverlapPoint(Rigidbody2D self, ref Vector2 point);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPoint(Rigidbody2D rigidbody, ref Vector2 point, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(Rigidbody2D rigidbody, ref Vector2 point, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(Rigidbody2D rigidbody, ref Vector2 relativePoint, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(Rigidbody2D rigidbody, ref Vector2 relativePoint, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(Rigidbody2D rigidbody, ref Vector2 relativeVector, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetVector(Rigidbody2D rigidbody, ref Vector2 vector, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_centerOfMass(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_position(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_velocity(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_worldCenterOfMass(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_centerOfMass(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_position(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_velocity(ref Vector2 value);
        /// <summary>
        /// <para>Is the rigidbody "awake"?</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsAwake();
        /// <summary>
        /// <para>Is the rigidbody "sleeping"?</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsSleeping();
        /// <summary>
        /// <para>Check whether any of the collider(s) attached to this rigidbody are touching the collider or not.</para>
        /// </summary>
        /// <param name="collider">The collider to check if it is touching any of the collider(s) attached to this rigidbody.</param>
        /// <returns>
        /// <para>Whether the collider is touching any of the collider(s) attached to this rigidbody or not.</para>
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
        /// <para>Checks whether any of the collider(s) attached to this rigidbody are touching any colliders on the specified layerMask or not.</para>
        /// </summary>
        /// <param name="layerMask">Any colliders on any of these layers count as touching.</param>
        /// <returns>
        /// <para>Whether any of the collider(s) attached to this rigidbody are touching any colliders on the specified layerMask or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
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
        /// <para>Check if any of the Rigidbody2D colliders overlap a point in space.</para>
        /// </summary>
        /// <param name="point">A point in world space.</param>
        /// <returns>
        /// <para>Whether the point overlapped any of the Rigidbody2D colliders.</para>
        /// </returns>
        public bool OverlapPoint(Vector2 point)
        {
            return INTERNAL_CALL_OverlapPoint(this, ref point);
        }

        private static void Rigidbody2D_CUSTOM_INTERNAL_GetPoint(Rigidbody2D rigidbody, Vector2 point, out Vector2 value)
        {
            INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPoint(rigidbody, ref point, out value);
        }

        private static void Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(Rigidbody2D rigidbody, Vector2 point, out Vector2 value)
        {
            INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(rigidbody, ref point, out value);
        }

        private static void Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(Rigidbody2D rigidbody, Vector2 relativePoint, out Vector2 value)
        {
            INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(rigidbody, ref relativePoint, out value);
        }

        private static void Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(Rigidbody2D rigidbody, Vector2 relativePoint, out Vector2 value)
        {
            INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(rigidbody, ref relativePoint, out value);
        }

        private static void Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(Rigidbody2D rigidbody, Vector2 relativeVector, out Vector2 value)
        {
            INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(rigidbody, ref relativeVector, out value);
        }

        private static void Rigidbody2D_CUSTOM_INTERNAL_GetVector(Rigidbody2D rigidbody, Vector2 vector, out Vector2 value)
        {
            INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetVector(rigidbody, ref vector, out value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetDragBehaviour(bool dragged);
        /// <summary>
        /// <para>Make the rigidbody "sleep".</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Sleep();
        /// <summary>
        /// <para>Disables the "sleeping" state of a rigidbody.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void WakeUp();

        /// <summary>
        /// <para>Coefficient of angular drag.</para>
        /// </summary>
        public float angularDrag { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Angular velocity in degrees per second.</para>
        /// </summary>
        public float angularVelocity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The physical behaviour type of the Rigidbody2D.</para>
        /// </summary>
        public RigidbodyType2D bodyType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

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
        public CollisionDetectionMode2D collisionDetectionMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Controls which degrees of freedom are allowed for the simulation of this Rigidbody2D.</para>
        /// </summary>
        public RigidbodyConstraints2D constraints { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Coefficient of drag.</para>
        /// </summary>
        public float drag { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the rigidbody be prevented from rotating?</para>
        /// </summary>
        [Obsolete("The fixedAngle is no longer supported. Use constraints instead.")]
        public bool fixedAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Controls whether physics will change the rotation of the object.</para>
        /// </summary>
        public bool freezeRotation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The degree to which this object is affected by gravity.</para>
        /// </summary>
        public float gravityScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The rigidBody rotational inertia.</para>
        /// </summary>
        public float inertia { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Physics interpolation used between updates.</para>
        /// </summary>
        public RigidbodyInterpolation2D interpolation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should this rigidbody be taken out of physics control?</para>
        /// </summary>
        public bool isKinematic
        {
            get
            {
                return (this.bodyType == RigidbodyType2D.Kinematic);
            }
            set
            {
                this.bodyType = !value ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
            }
        }

        /// <summary>
        /// <para>Mass of the rigidbody.</para>
        /// </summary>
        public float mass { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

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
        /// <para>The rotation of the rigidbody.</para>
        /// </summary>
        public float rotation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The PhysicsMaterial2D that is applied to all Collider2D attached to this Rigidbody2D.</para>
        /// </summary>
        public PhysicsMaterial2D sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Indicates whether the rigid body should be simulated or not by the physics system.</para>
        /// </summary>
        public bool simulated { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The sleep state that the rigidbody will initially be in.</para>
        /// </summary>
        public RigidbodySleepMode2D sleepMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the total rigid-body mass be automatically calculated from the Collider2D.density of attached colliders?</para>
        /// </summary>
        public bool useAutoMass { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should kinematickinematic and kinematicstatic collisions be allowed?</para>
        /// </summary>
        public bool useFullKinematicContacts { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

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

