namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Asynchronous load request from the Resources bundle.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class ResourceRequest : AsyncOperation
    {
        internal string m_Path;
        internal System.Type m_Type;
        /// <summary>
        /// <para>Asset object being loaded (Read Only).</para>
        /// </summary>
        public UnityEngine.Object asset =>
            Resources.Load(this.m_Path, this.m_Type);
    }
}

