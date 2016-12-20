namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class SocketExtensions
    {
        [Extension]
        public static void DisposePortable(Socket socket)
        {
            socket.Close();
        }
    }
}

