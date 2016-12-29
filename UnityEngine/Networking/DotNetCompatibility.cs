namespace UnityEngine.Networking
{
    using System;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    internal static class DotNetCompatibility
    {
        internal static Type GetBaseType(this Type type) => 
            type.BaseType;

        internal static string GetErrorCode(this SocketException e) => 
            e.ErrorCode.ToString();

        internal static string GetMethodName(this Delegate func) => 
            func.Method.Name;
    }
}

