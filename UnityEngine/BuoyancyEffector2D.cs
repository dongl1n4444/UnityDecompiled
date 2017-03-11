namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Applies forces to simulate buoyancy, fluid-flow and fluid drag.</para>
    /// </summary>
    public sealed class BuoyancyEffector2D : Effector2D
    {
        /// <summary>
        /// <para>A force applied to slow angular movement of any Collider2D in contact with the effector.</para>
        /// </summary>
        public float angularDrag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The density of the fluid used to calculate the buoyancy forces.</para>
        /// </summary>
        public float density { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The angle of the force used to similate fluid flow.</para>
        /// </summary>
        public float flowAngle { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The magnitude of the force used to similate fluid flow.</para>
        /// </summary>
        public float flowMagnitude { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The random variation of the force used to similate fluid flow.</para>
        /// </summary>
        public float flowVariation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>A force applied to slow linear movement of any Collider2D in contact with the effector.</para>
        /// </summary>
        public float linearDrag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Defines an arbitrary horizontal line that represents the fluid surface level.</para>
        /// </summary>
        public float surfaceLevel { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

