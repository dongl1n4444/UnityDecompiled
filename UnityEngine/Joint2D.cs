namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Parent class for joints to connect Rigidbody2D objects.</para>
    /// </summary>
    public class Joint2D : Behaviour
    {
        /// <summary>
        /// <para>Gets the reaction force of the joint given the specified timeStep.</para>
        /// </summary>
        /// <param name="timeStep">The time to calculate the reaction force for.</param>
        /// <returns>
        /// <para>The reaction force of the joint in the specified timeStep.</para>
        /// </returns>
        public Vector2 GetReactionForce(float timeStep)
        {
            Vector2 vector;
            Joint2D_CUSTOM_INTERNAL_GetReactionForce(this, timeStep, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Gets the reaction torque of the joint given the specified timeStep.</para>
        /// </summary>
        /// <param name="timeStep">The time to calculate the reaction torque for.</param>
        /// <returns>
        /// <para>The reaction torque of the joint in the specified timeStep.</para>
        /// </returns>
        public float GetReactionTorque(float timeStep) => 
            INTERNAL_CALL_GetReactionTorque(this, timeStep);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float INTERNAL_CALL_GetReactionTorque(Joint2D self, float timeStep);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Joint2D_CUSTOM_INTERNAL_GetReactionForce(Joint2D joint, float timeStep, out Vector2 value);

        /// <summary>
        /// <para>The force that needs to be applied for this joint to break.</para>
        /// </summary>
        public float breakForce { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The torque that needs to be applied for this joint to break.</para>
        /// </summary>
        public float breakTorque { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Can the joint collide with the other Rigidbody2D object to which it is attached?</para>
        /// </summary>
        [Obsolete("Joint2D.collideConnected has been deprecated. Use Joint2D.enableCollision instead (UnityUpgradable) -> enableCollision", true)]
        public bool collideConnected
        {
            get => 
                this.enableCollision;
            set
            {
                this.enableCollision = value;
            }
        }

        /// <summary>
        /// <para>The Rigidbody2D object to which the other end of the joint is attached (ie, the object without the joint component).</para>
        /// </summary>
        public Rigidbody2D connectedBody { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the two rigid bodies connected with this joint collide with each other?</para>
        /// </summary>
        public bool enableCollision { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Gets the reaction force of the joint.</para>
        /// </summary>
        public Vector2 reactionForce =>
            this.GetReactionForce(Time.fixedDeltaTime);

        /// <summary>
        /// <para>Gets the reaction torque of the joint.</para>
        /// </summary>
        public float reactionTorque =>
            this.GetReactionTorque(Time.fixedDeltaTime);
    }
}

