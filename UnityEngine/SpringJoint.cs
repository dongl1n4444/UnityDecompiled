namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>The spring joint ties together 2 rigid bodies, spring forces will be automatically applied to keep the object at the given distance.</para>
    /// </summary>
    public sealed class SpringJoint : Joint
    {
        /// <summary>
        /// <para>The damper force used to dampen the spring force.</para>
        /// </summary>
        public float damper { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum distance between the bodies relative to their initial distance.</para>
        /// </summary>
        public float maxDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The minimum distance between the bodies relative to their initial distance.</para>
        /// </summary>
        public float minDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The spring force used to keep the two objects together.</para>
        /// </summary>
        public float spring { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum allowed error between the current spring length and the length defined by minDistance and maxDistance.</para>
        /// </summary>
        public float tolerance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

