namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Asset type that defines the surface properties of a Collider2D.</para>
    /// </summary>
    public sealed class PhysicsMaterial2D : UnityEngine.Object
    {
        public PhysicsMaterial2D()
        {
            Internal_Create(this, null);
        }

        public PhysicsMaterial2D(string name)
        {
            Internal_Create(this, name);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_Create([Writable] PhysicsMaterial2D mat, string name);

        /// <summary>
        /// <para>The degree of elasticity during collisions.</para>
        /// </summary>
        public float bounciness { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Coefficient of friction.</para>
        /// </summary>
        public float friction { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

