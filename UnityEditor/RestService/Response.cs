namespace UnityEditor.RestService
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    internal sealed class Response
    {
        private IntPtr m_nativeRequestPtr;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SimpleResponse(HttpStatusCode status, string payload);
    }
}

