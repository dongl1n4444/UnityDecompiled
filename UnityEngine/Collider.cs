namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A base class of all colliders.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class Collider : Component
    {
        /// <summary>
        /// <para>Returns a point on the collider that is closest to a given location.</para>
        /// </summary>
        /// <param name="position">Location you want to find the closest point to.</param>
        /// <returns>
        /// <para>The point on the collider that is closest to the specified location.</para>
        /// </returns>
        public Vector3 ClosestPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ClosestPoint(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>The closest point to the bounding box of the attached collider.</para>
        /// </summary>
        /// <param name="position"></param>
        public Vector3 ClosestPointOnBounds(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ClosestPointOnBounds(this, ref position, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ClosestPoint(Collider self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ClosestPointOnBounds(Collider self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Internal_Raycast(Collider col, ref Ray ray, out RaycastHit hitInfo, float maxDistance);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_bounds(out Bounds value);
        private static bool Internal_Raycast(Collider col, Ray ray, out RaycastHit hitInfo, float maxDistance) => 
            INTERNAL_CALL_Internal_Raycast(col, ref ray, out hitInfo, maxDistance);

        public bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance) => 
            Internal_Raycast(this, ray, out hitInfo, maxDistance);

        /// <summary>
        /// <para>The rigidbody the collider is attached to.</para>
        /// </summary>
        public Rigidbody attachedRigidbody { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The world space bounding volume of the collider.</para>
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
        /// <para>Contact offset value of this collider.</para>
        /// </summary>
        public float contactOffset { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enabled Colliders will collide with other colliders, disabled Colliders won't.</para>
        /// </summary>
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is the collider a trigger?</para>
        /// </summary>
        public bool isTrigger { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The material used by the collider.</para>
        /// </summary>
        public PhysicMaterial material { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The shared physic material of this collider.</para>
        /// </summary>
        public PhysicMaterial sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

