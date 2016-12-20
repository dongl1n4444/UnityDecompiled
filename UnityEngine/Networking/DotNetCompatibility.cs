namespace UnityEngine.Networking
{
    using System;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    [Extension]
    internal static class DotNetCompatibility
    {
        [Extension]
        internal static Type GetBaseType(Type type)
        {
            return type.BaseType;
        }

        [Extension]
        internal static string GetErrorCode(SocketException e)
        {
            return e.ErrorCode.ToString();
        }

        [Extension]
        internal static string GetMethodName(Delegate func)
        {
            return func.Method.Name;
        }
    }
}

