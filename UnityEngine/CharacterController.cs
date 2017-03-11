namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A CharacterController allows you to easily do movement constrained by collisions without having to deal with a rigidbody.</para>
    /// </summary>
    public sealed class CharacterController : Collider
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern CollisionFlags INTERNAL_CALL_Move(CharacterController self, ref Vector3 motion);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_SimpleMove(CharacterController self, ref Vector3 speed);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_center(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_velocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_center(ref Vector3 value);
        /// <summary>
        /// <para>A more complex move function taking absolute movement deltas.</para>
        /// </summary>
        /// <param name="motion"></param>
        public CollisionFlags Move(Vector3 motion) => 
            INTERNAL_CALL_Move(this, ref motion);

        /// <summary>
        /// <para>Moves the character with speed.</para>
        /// </summary>
        /// <param name="speed"></param>
        public bool SimpleMove(Vector3 speed) => 
            INTERNAL_CALL_SimpleMove(this, ref speed);

        /// <summary>
        /// <para>The center of the character's capsule relative to the transform's position.</para>
        /// </summary>
        public Vector3 center
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_center(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_center(ref value);
            }
        }

        /// <summary>
        /// <para>What part of the capsule collided with the environment during the last CharacterController.Move call.</para>
        /// </summary>
        public CollisionFlags collisionFlags { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Determines whether other rigidbodies or character controllers collide with this character controller (by default this is always enabled).</para>
        /// </summary>
        public bool detectCollisions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enables or disables overlap recovery.
        /// Enables or disables overlap recovery. Used to depenetrate character controllers from static objects when an overlap is detected.</para>
        /// </summary>
        public bool enableOverlapRecovery { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The height of the character's capsule.</para>
        /// </summary>
        public float height { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Was the CharacterController touching the ground during the last move?</para>
        /// </summary>
        public bool isGrounded { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Gets or sets the minimum move distance of the character controller.</para>
        /// </summary>
        public float minMoveDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The radius of the character's capsule.</para>
        /// </summary>
        public float radius { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The character's collision skin width.</para>
        /// </summary>
        public float skinWidth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The character controllers slope limit in degrees.</para>
        /// </summary>
        public float slopeLimit { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The character controllers step offset in meters.</para>
        /// </summary>
        public float stepOffset { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The current relative velocity of the Character (see notes).</para>
        /// </summary>
        public Vector3 velocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_velocity(out vector);
                return vector;
            }
        }
    }
}

