namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>ShaderVariantCollection records which shader variants are actually used in each shader.</para>
    /// </summary>
    public sealed class ShaderVariantCollection : UnityEngine.Object
    {
        /// <summary>
        /// <para>Create a new empty shader variant collection.</para>
        /// </summary>
        public ShaderVariantCollection()
        {
            Internal_Create(this);
        }

        public bool Add(ShaderVariant variant) => 
            this.AddInternal(variant.shader, variant.passType, variant.keywords);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddInternal(Shader shader, PassType passType, string[] keywords);
        /// <summary>
        /// <para>Remove all shader variants from the collection.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Clear();
        public bool Contains(ShaderVariant variant) => 
            this.ContainsInternal(variant.shader, variant.passType, variant.keywords);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool ContainsInternal(Shader shader, PassType passType, string[] keywords);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_Create([Writable] ShaderVariantCollection mono);
        public bool Remove(ShaderVariant variant) => 
            this.RemoveInternal(variant.shader, variant.passType, variant.keywords);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool RemoveInternal(Shader shader, PassType passType, string[] keywords);
        /// <summary>
        /// <para>Fully load shaders in ShaderVariantCollection.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void WarmUp();

        /// <summary>
        /// <para>Is this ShaderVariantCollection already warmed up? (Read Only)</para>
        /// </summary>
        public bool isWarmedUp { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Number of shaders in this collection (Read Only).</para>
        /// </summary>
        public int shaderCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Number of total varians in this collection (Read Only).</para>
        /// </summary>
        public int variantCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Identifies a specific variant of a shader.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ShaderVariant
        {
            /// <summary>
            /// <para>Shader to use in this variant.</para>
            /// </summary>
            public Shader shader;
            /// <summary>
            /// <para>Pass type to use in this variant.</para>
            /// </summary>
            public PassType passType;
            /// <summary>
            /// <para>Array of shader keywords to use in this variant.</para>
            /// </summary>
            public string[] keywords;
            /// <summary>
            /// <para>Creates a ShaderVariant structure.</para>
            /// </summary>
            /// <param name="shader"></param>
            /// <param name="passType"></param>
            /// <param name="keywords"></param>
            public ShaderVariant(Shader shader, PassType passType, params string[] keywords)
            {
                this.shader = shader;
                this.passType = passType;
                this.keywords = keywords;
                Internal_CheckVariant(shader, passType, keywords);
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void Internal_CheckVariant(Shader shader, PassType passType, string[] keywords);
        }
    }
}

