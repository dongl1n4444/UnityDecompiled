namespace UnityEditor.RestService
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class Request
    {
        private IntPtr m_nativeRequestPtr;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetParam(string paramName);

        public int Depth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool Info { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int MessageType { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public string Payload { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public string Url { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

