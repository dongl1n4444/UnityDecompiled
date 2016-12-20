namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>CollisionFlags is a bitmask returned by CharacterController.Move.</para>
    /// </summary>
    public enum CollisionFlags
    {
        /// <summary>
        /// <para>CollisionFlags is a bitmask returned by CharacterController.Move.</para>
        /// </summary>
        Above = 2,
        /// <summary>
        /// <para>CollisionFlags is a bitmask returned by CharacterController.Move.</para>
        /// </summary>
        Below = 4,
        CollidedAbove = 2,
        CollidedBelow = 4,
        CollidedSides = 1,
        /// <summary>
        /// <para>CollisionFlags is a bitmask returned by CharacterController.Move.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>CollisionFlags is a bitmask returned by CharacterController.Move.</para>
        /// </summary>
        Sides = 1
    }
}

