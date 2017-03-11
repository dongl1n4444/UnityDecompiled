namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Control of an object's position through physics simulation.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class Rigidbody : Component
    {
        /// <summary>
        /// <para>Applies a force to a rigidbody that simulates explosion effects.</para>
        /// </summary>
        /// <param name="explosionForce">The force of the explosion (which may be modified by distance).</param>
        /// <param name="explosionPosition">The centre of the sphere within which the explosion has its effect.</param>
        /// <param name="explosionRadius">The radius of the sphere within which the explosion has its effect.</param>
        /// <param name="upwardsModifier">Adjustment to the apparent position of the explosion to make it seem to lift objects.</param>
        /// <param name="mode">The method used to apply the force to its targets.</param>
        [ExcludeFromDocs]
        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
        {
            ForceMode force = ForceMode.Force;
            float upwardsModifier = 0f;
            INTERNAL_CALL_AddExplosionForce(this, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, force);
        }

        /// <summary>
        /// <para>Applies a force to a rigidbody that simulates explosion effects.</para>
        /// </summary>
        /// <param name="explosionForce">The force of the explosion (which may be modified by distance).</param>
        /// <param name="explosionPosition">The centre of the sphere within which the explosion has its effect.</param>
        /// <param name="explosionRadius">The radius of the sphere within which the explosion has its effect.</param>
        /// <param name="upwardsModifier">Adjustment to the apparent position of the explosion to make it seem to lift objects.</param>
        /// <param name="mode">The method used to apply the force to its targets.</param>
        [ExcludeFromDocs]
        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier)
        {
            ForceMode force = ForceMode.Force;
            INTERNAL_CALL_AddExplosionForce(this, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, force);
        }

        /// <summary>
        /// <para>Applies a force to a rigidbody that simulates explosion effects.</para>
        /// </summary>
        /// <param name="explosionForce">The force of the explosion (which may be modified by distance).</param>
        /// <param name="explosionPosition">The centre of the sphere within which the explosion has its effect.</param>
        /// <param name="explosionRadius">The radius of the sphere within which the explosion has its effect.</param>
        /// <param name="upwardsModifier">Adjustment to the apparent position of the explosion to make it seem to lift objects.</param>
        /// <param name="mode">The method used to apply the force to its targets.</param>
        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, [DefaultValue("0.0F")] float upwardsModifier, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddExplosionForce(this, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, mode);
        }

        /// <summary>
        /// <para>Adds a force to the rigidbody.</para>
        /// </summary>
        /// <param name="force">Force vector in world coordinates.</param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void AddForce(Vector3 force)
        {
            ForceMode mode = ForceMode.Force;
            INTERNAL_CALL_AddForce(this, ref force, mode);
        }

        /// <summary>
        /// <para>Adds a force to the rigidbody.</para>
        /// </summary>
        /// <param name="force">Force vector in world coordinates.</param>
        /// <param name="mode"></param>
        public void AddForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddForce(this, ref force, mode);
        }

        /// <summary>
        /// <para>Adds a force to the rigidbody.</para>
        /// </summary>
        /// <param name="x">Size of force along the world x-axis.</param>
        /// <param name="y">Size of force along the world y-axis.</param>
        /// <param name="z">Size of force along the world z-axis.</param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void AddForce(float x, float y, float z)
        {
            ForceMode force = ForceMode.Force;
            this.AddForce(x, y, z, force);
        }

        /// <summary>
        /// <para>Adds a force to the rigidbody.</para>
        /// </summary>
        /// <param name="x">Size of force along the world x-axis.</param>
        /// <param name="y">Size of force along the world y-axis.</param>
        /// <param name="z">Size of force along the world z-axis.</param>
        /// <param name="mode"></param>
        public void AddForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            this.AddForce(new Vector3(x, y, z), mode);
        }

        /// <summary>
        /// <para>Applies force at position. As a result this will apply a torque and force on the object.</para>
        /// </summary>
        /// <param name="force">Force vector in world coordinates.</param>
        /// <param name="position">Position in world coordinates.</param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void AddForceAtPosition(Vector3 force, Vector3 position)
        {
            ForceMode mode = ForceMode.Force;
            INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
        }

        /// <summary>
        /// <para>Applies force at position. As a result this will apply a torque and force on the object.</para>
        /// </summary>
        /// <param name="force">Force vector in world coordinates.</param>
        /// <param name="position">Position in world coordinates.</param>
        /// <param name="mode"></param>
        public void AddForceAtPosition(Vector3 force, Vector3 position, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
        }

        /// <summary>
        /// <para>Adds a force to the rigidbody relative to its coordinate system.</para>
        /// </summary>
        /// <param name="force">Force vector in local coordinates.</param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void AddRelativeForce(Vector3 force)
        {
            ForceMode mode = ForceMode.Force;
            INTERNAL_CALL_AddRelativeForce(this, ref force, mode);
        }

        /// <summary>
        /// <para>Adds a force to the rigidbody relative to its coordinate system.</para>
        /// </summary>
        /// <param name="force">Force vector in local coordinates.</param>
        /// <param name="mode"></param>
        public void AddRelativeForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddRelativeForce(this, ref force, mode);
        }

        /// <summary>
        /// <para>Adds a force to the rigidbody relative to its coordinate system.</para>
        /// </summary>
        /// <param name="x">Size of force along the local x-axis.</param>
        /// <param name="y">Size of force along the local y-axis.</param>
        /// <param name="z">Size of force along the local z-axis.</param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void AddRelativeForce(float x, float y, float z)
        {
            ForceMode force = ForceMode.Force;
            this.AddRelativeForce(x, y, z, force);
        }

        /// <summary>
        /// <para>Adds a force to the rigidbody relative to its coordinate system.</para>
        /// </summary>
        /// <param name="x">Size of force along the local x-axis.</param>
        /// <param name="y">Size of force along the local y-axis.</param>
        /// <param name="z">Size of force along the local z-axis.</param>
        /// <param name="mode"></param>
        public void AddRelativeForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            this.AddRelativeForce(new Vector3(x, y, z), mode);
        }

        /// <summary>
        /// <para>Adds a torque to the rigidbody relative to its coordinate system.</para>
        /// </summary>
        /// <param name="torque">Torque vector in local coordinates.</param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void AddRelativeTorque(Vector3 torque)
        {
            ForceMode force = ForceMode.Force;
            INTERNAL_CALL_AddRelativeTorque(this, ref torque, force);
        }

        /// <summary>
        /// <para>Adds a torque to the rigidbody relative to its coordinate system.</para>
        /// </summary>
        /// <param name="torque">Torque vector in local coordinates.</param>
        /// <param name="mode"></param>
        public void AddRelativeTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddRelativeTorque(this, ref torque, mode);
        }

        /// <summary>
        /// <para>Adds a torque to the rigidbody relative to its coordinate system.</para>
        /// </summary>
        /// <param name="x">Size of torque along the local x-axis.</param>
        /// <param name="y">Size of torque along the local y-axis.</param>
        /// <param name="z">Size of torque along the local z-axis.</param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void AddRelativeTorque(float x, float y, float z)
        {
            ForceMode force = ForceMode.Force;
            this.AddRelativeTorque(x, y, z, force);
        }

        /// <summary>
        /// <para>Adds a torque to the rigidbody relative to its coordinate system.</para>
        /// </summary>
        /// <param name="x">Size of torque along the local x-axis.</param>
        /// <param name="y">Size of torque along the local y-axis.</param>
        /// <param name="z">Size of torque along the local z-axis.</param>
        /// <param name="mode"></param>
        public void AddRelativeTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            this.AddRelativeTorque(new Vector3(x, y, z), mode);
        }

        /// <summary>
        /// <para>Adds a torque to the rigidbody.</para>
        /// </summary>
        /// <param name="torque">Torque vector in world coordinates.</param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void AddTorque(Vector3 torque)
        {
            ForceMode force = ForceMode.Force;
            INTERNAL_CALL_AddTorque(this, ref torque, force);
        }

        /// <summary>
        /// <para>Adds a torque to the rigidbody.</para>
        /// </summary>
        /// <param name="torque">Torque vector in world coordinates.</param>
        /// <param name="mode"></param>
        public void AddTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddTorque(this, ref torque, mode);
        }

        /// <summary>
        /// <para>Adds a torque to the rigidbody.</para>
        /// </summary>
        /// <param name="x">Size of torque along the world x-axis.</param>
        /// <param name="y">Size of torque along the world y-axis.</param>
        /// <param name="z">Size of torque along the world z-axis.</param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void AddTorque(float x, float y, float z)
        {
            ForceMode force = ForceMode.Force;
            this.AddTorque(x, y, z, force);
        }

        /// <summary>
        /// <para>Adds a torque to the rigidbody.</para>
        /// </summary>
        /// <param name="x">Size of torque along the world x-axis.</param>
        /// <param name="y">Size of torque along the world y-axis.</param>
        /// <param name="z">Size of torque along the world z-axis.</param>
        /// <param name="mode"></param>
        public void AddTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            this.AddTorque(new Vector3(x, y, z), mode);
        }

        /// <summary>
        /// <para>The closest point to the bounding box of the attached colliders.</para>
        /// </summary>
        /// <param name="position"></param>
        public Vector3 ClosestPointOnBounds(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ClosestPointOnBounds(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>The velocity of the rigidbody at the point worldPoint in global space.</para>
        /// </summary>
        /// <param name="worldPoint"></param>
        public Vector3 GetPointVelocity(Vector3 worldPoint)
        {
            Vector3 vector;
            INTERNAL_CALL_GetPointVelocity(this, ref worldPoint, out vector);
            return vector;
        }

        /// <summary>
        /// <para>The velocity relative to the rigidbody at the point relativePoint.</para>
        /// </summary>
        /// <param name="relativePoint"></param>
        public Vector3 GetRelativePointVelocity(Vector3 relativePoint)
        {
            Vector3 vector;
            INTERNAL_CALL_GetRelativePointVelocity(this, ref relativePoint, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddExplosionForce(Rigidbody self, float explosionForce, ref Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddForce(Rigidbody self, ref Vector3 force, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddForceAtPosition(Rigidbody self, ref Vector3 force, ref Vector3 position, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddRelativeForce(Rigidbody self, ref Vector3 force, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddRelativeTorque(Rigidbody self, ref Vector3 torque, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_AddTorque(Rigidbody self, ref Vector3 torque, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ClosestPointOnBounds(Rigidbody self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetPointVelocity(Rigidbody self, ref Vector3 worldPoint, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetRelativePointVelocity(Rigidbody self, ref Vector3 relativePoint, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsSleeping(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MovePosition(Rigidbody self, ref Vector3 position);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MoveRotation(Rigidbody self, ref Quaternion rot);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ResetCenterOfMass(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ResetInertiaTensor(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetDensity(Rigidbody self, float density);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Sleep(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_SweepTest(Rigidbody self, ref Vector3 direction, out RaycastHit hitInfo, float maxDistance, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern RaycastHit[] INTERNAL_CALL_SweepTestAll(Rigidbody self, ref Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_WakeUp(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_angularVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_centerOfMass(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_inertiaTensor(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_inertiaTensorRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_position(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_rotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_velocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_worldCenterOfMass(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_angularVelocity(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_centerOfMass(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_inertiaTensor(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_inertiaTensorRotation(ref Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_position(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_rotation(ref Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_velocity(ref Vector3 value);
        /// <summary>
        /// <para>Is the rigidbody sleeping?</para>
        /// </summary>
        public bool IsSleeping() => 
            INTERNAL_CALL_IsSleeping(this);

        /// <summary>
        /// <para>Moves the rigidbody to position.</para>
        /// </summary>
        /// <param name="position">The new position for the Rigidbody object.</param>
        public void MovePosition(Vector3 position)
        {
            INTERNAL_CALL_MovePosition(this, ref position);
        }

        /// <summary>
        /// <para>Rotates the rigidbody to rotation.</para>
        /// </summary>
        /// <param name="rot">The new rotation for the Rigidbody.</param>
        public void MoveRotation(Quaternion rot)
        {
            INTERNAL_CALL_MoveRotation(this, ref rot);
        }

        /// <summary>
        /// <para>Reset the center of mass of the rigidbody.</para>
        /// </summary>
        public void ResetCenterOfMass()
        {
            INTERNAL_CALL_ResetCenterOfMass(this);
        }

        /// <summary>
        /// <para>Reset the inertia tensor value and rotation.</para>
        /// </summary>
        public void ResetInertiaTensor()
        {
            INTERNAL_CALL_ResetInertiaTensor(this);
        }

        /// <summary>
        /// <para>Sets the mass based on the attached colliders assuming a constant density.</para>
        /// </summary>
        /// <param name="density"></param>
        public void SetDensity(float density)
        {
            INTERNAL_CALL_SetDensity(this, density);
        }

        [Obsolete("use Rigidbody.maxAngularVelocity instead.")]
        public void SetMaxAngularVelocity(float a)
        {
            this.maxAngularVelocity = a;
        }

        /// <summary>
        /// <para>Forces a rigidbody to sleep at least one frame.</para>
        /// </summary>
        public void Sleep()
        {
            INTERNAL_CALL_Sleep(this);
        }

        [ExcludeFromDocs]
        public bool SweepTest(Vector3 direction, out RaycastHit hitInfo)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_SweepTest(this, ref direction, out hitInfo, positiveInfinity, useGlobal);
        }

        [ExcludeFromDocs]
        public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_SweepTest(this, ref direction, out hitInfo, maxDistance, useGlobal);
        }

        public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction) => 
            INTERNAL_CALL_SweepTest(this, ref direction, out hitInfo, maxDistance, queryTriggerInteraction);

        [ExcludeFromDocs]
        public RaycastHit[] SweepTestAll(Vector3 direction)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_SweepTestAll(this, ref direction, positiveInfinity, useGlobal);
        }

        [ExcludeFromDocs]
        public RaycastHit[] SweepTestAll(Vector3 direction, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_SweepTestAll(this, ref direction, maxDistance, useGlobal);
        }

        /// <summary>
        /// <para>Like Rigidbody.SweepTest, but returns all hits.</para>
        /// </summary>
        /// <param name="direction">The direction into which to sweep the rigidbody.</param>
        /// <param name="maxDistance">The length of the sweep.</param>
        /// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
        /// <returns>
        /// <para>An array of all colliders hit in the sweep.</para>
        /// </returns>
        public RaycastHit[] SweepTestAll(Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction) => 
            INTERNAL_CALL_SweepTestAll(this, ref direction, maxDistance, queryTriggerInteraction);

        /// <summary>
        /// <para>Forces a rigidbody to wake up.</para>
        /// </summary>
        public void WakeUp()
        {
            INTERNAL_CALL_WakeUp(this);
        }

        /// <summary>
        /// <para>The angular drag of the object.</para>
        /// </summary>
        public float angularDrag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The angular velocity vector of the rigidbody.</para>
        /// </summary>
        public Vector3 angularVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_angularVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_angularVelocity(ref value);
            }
        }

        /// <summary>
        /// <para>The center of mass relative to the transform's origin.</para>
        /// </summary>
        public Vector3 centerOfMass
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_centerOfMass(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_centerOfMass(ref value);
            }
        }

        /// <summary>
        /// <para>The Rigidbody's collision detection mode.</para>
        /// </summary>
        public CollisionDetectionMode collisionDetectionMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Controls which degrees of freedom are allowed for the simulation of this Rigidbody.</para>
        /// </summary>
        public RigidbodyConstraints constraints { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should collision detection be enabled? (By default always enabled).</para>
        /// </summary>
        public bool detectCollisions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The drag of the object.</para>
        /// </summary>
        public float drag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Controls whether physics will change the rotation of the object.</para>
        /// </summary>
        public bool freezeRotation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The diagonal inertia tensor of mass relative to the center of mass.</para>
        /// </summary>
        public Vector3 inertiaTensor
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_inertiaTensor(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_inertiaTensor(ref value);
            }
        }

        /// <summary>
        /// <para>The rotation of the inertia tensor.</para>
        /// </summary>
        public Quaternion inertiaTensorRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_inertiaTensorRotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_inertiaTensorRotation(ref value);
            }
        }

        /// <summary>
        /// <para>Interpolation allows you to smooth out the effect of running physics at a fixed frame rate.</para>
        /// </summary>
        public RigidbodyInterpolation interpolation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Controls whether physics affects the rigidbody.</para>
        /// </summary>
        public bool isKinematic { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The mass of the rigidbody.</para>
        /// </summary>
        public float mass { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The maximimum angular velocity of the rigidbody. (Default 7) range { 0, infinity }.</para>
        /// </summary>
        public float maxAngularVelocity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Maximum velocity of a rigidbody when moving out of penetrating state.</para>
        /// </summary>
        public float maxDepenetrationVelocity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The position of the rigidbody.</para>
        /// </summary>
        public Vector3 position
        {
            get
            {
                Vector3 vector;
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
        public Quaternion rotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_rotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_rotation(ref value);
            }
        }

        /// <summary>
        /// <para>The angular velocity below which objects start going to sleep.  (Default 0.14) range { 0, infinity }.</para>
        /// </summary>
        [Obsolete("The sleepAngularVelocity is no longer supported. Set Use sleepThreshold to specify energy.")]
        public float sleepAngularVelocity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The mass-normalized energy threshold, below which objects start going to sleep.</para>
        /// </summary>
        public float sleepThreshold { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The linear velocity below which objects start going to sleep. (Default 0.14) range { 0, infinity }.</para>
        /// </summary>
        [Obsolete("The sleepVelocity is no longer supported. Use sleepThreshold. Note that sleepThreshold is energy but not velocity.")]
        public float sleepVelocity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Please use Rigidbody.solverIterations instead. (UnityUpgradable) -> solverIterations")]
        public int solverIterationCount
        {
            get => 
                this.solverIterations;
            set
            {
                this.solverIterations = value;
            }
        }

        /// <summary>
        /// <para>The solverIterations determines how accurately Rigidbody joints and collision contacts are resolved. Overrides Physics.defaultSolverIterations. Must be positive.</para>
        /// </summary>
        public int solverIterations { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Please use Rigidbody.solverVelocityIterations instead. (UnityUpgradable) -> solverVelocityIterations")]
        public int solverVelocityIterationCount
        {
            get => 
                this.solverVelocityIterations;
            set
            {
                this.solverVelocityIterations = value;
            }
        }

        /// <summary>
        /// <para>The solverVelocityIterations affects how how accurately Rigidbody joints and collision contacts are resolved. Overrides Physics.defaultSolverVelocityIterations. Must be positive.</para>
        /// </summary>
        public int solverVelocityIterations { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Force cone friction to be used for this rigidbody.</para>
        /// </summary>
        [Obsolete("Cone friction is no longer supported.")]
        public bool useConeFriction { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Controls whether gravity affects this rigidbody.</para>
        /// </summary>
        public bool useGravity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The velocity vector of the rigidbody.</para>
        /// </summary>
        public Vector3 velocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_velocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_velocity(ref value);
            }
        }

        /// <summary>
        /// <para>The center of mass of the rigidbody in world space (Read Only).</para>
        /// </summary>
        public Vector3 worldCenterOfMass
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_worldCenterOfMass(out vector);
                return vector;
            }
        }
    }
}

