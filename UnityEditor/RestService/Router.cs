namespace UnityEditor.RestService
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class Router
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool RegisterHandler(string route, Handler handler);
    }
}

