using System;
using System.Net.Sockets;

namespace Unity.IL2CPP.Portability
{
	public static class SocketExtensions
	{
		public static void DisposePortable(this Socket socket)
		{
			socket.Close();
		}
	}
}
