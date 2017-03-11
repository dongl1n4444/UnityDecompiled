namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Physics material describes how to handle colliding objects (friction, bounciness).</para>
    /// </summary>
    public sealed class PhysicMaterial : UnityEngine.Object
    {
        /// <summary>
        /// <para>Creates a new material.</para>
        /// </summary>
        public PhysicMaterial()
        {
            Internal_CreateDynamicsMaterial(this, null);
        }

        /// <summary>
        /// <para>Creates a new material named name.</para>
        /// </summary>
        /// <param name="name"></param>
        public PhysicMaterial(string name)
        {
            Internal_CreateDynamicsMaterial(this, name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_CreateDynamicsMaterial([Writable] PhysicMaterial mat, string name);

        /// <summary>
        /// <para>Determines how the bounciness is combined.</para>
        /// </summary>
        public PhysicMaterialCombine bounceCombine { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How bouncy is the surface? A value of 0 will not bounce. A value of 1 will bounce without any loss of energy.</para>
        /// </summary>
        public float bounciness { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Use PhysicMaterial.bounciness instead", true)]
        public float bouncyness
        {
            get => 
                this.bounciness;
            set
            {
                this.bounciness = value;
            }
        }

        /// <summary>
        /// <para>The friction used when already moving.  This value has to be between 0 and 1.</para>
        /// </summary>
        public float dynamicFriction { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If anisotropic friction is enabled, dynamicFriction2 will be applied along frictionDirection2.</para>
        /// </summary>
        [Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
        public float dynamicFriction2 { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Determines how the friction is combined.</para>
        /// </summary>
        public PhysicMaterialCombine frictionCombine { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
        public Vector3 frictionDirection
        {
            get => 
                Vector3.zero;
            set
            {
            }
        }

        /// <summary>
        /// <para>The direction of anisotropy. Anisotropic friction is enabled if the vector is not zero.</para>
        /// </summary>
        [Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
        public Vector3 frictionDirection2
        {
            get => 
                Vector3.zero;
            set
            {
            }
        }

        /// <summary>
        /// <para>The friction coefficient used when an object is lying on a surface.</para>
        /// </summary>
        public float staticFriction { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If anisotropic friction is enabled, staticFriction2 will be applied along frictionDirection2.</para>
        /// </summary>
        [Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
        public float staticFriction2 { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

