namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Shader pass name identifier.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ShaderPassName
    {
        private int nameIndex;
        /// <summary>
        /// <para>Create shader pass name identifier.</para>
        /// </summary>
        /// <param name="name">Pass name.</param>
        public ShaderPassName(string name)
        {
            this.nameIndex = Init(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int Init(string name);
    }
}

