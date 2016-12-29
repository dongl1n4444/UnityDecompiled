namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    public static class SocketExtensions
    {
        public static void DisposePortable(this Socket socket)
        {
            socket.Close();
        }
    }
}

