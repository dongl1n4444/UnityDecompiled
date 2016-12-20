namespace UnityEditor.RestService
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class Response
    {
        private IntPtr m_nativeRequestPtr;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SimpleResponse(HttpStatusCode status, string payload);
    }
}

