namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Behaviours are Components that can be enabled or disabled.</para>
    /// </summary>
    public class Behaviour : Component
    {
        /// <summary>
        /// <para>Enabled Behaviours are Updated, disabled Behaviours are not.</para>
        /// </summary>
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Has the Behaviour had enabled called.</para>
        /// </summary>
        public bool isActiveAndEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

