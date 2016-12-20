namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Collider for 2D physics representing an circle.</para>
    /// </summary>
    public sealed class CircleCollider2D : Collider2D
    {
        /// <summary>
        /// <para>The center point of the collider in local space.</para>
        /// </summary>
        [Obsolete("CircleCollider2D.center has been deprecated. Use CircleCollider2D.offset instead (UnityUpgradable) -> offset", true)]
        public Vector2 center
        {
            get
            {
                return Vector2.zero;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Radius of the circle.</para>
        /// </summary>
        public float radius { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

