namespace UnityEngine.Diagnostics
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Class for handling the connection between Editor and Player.
    /// This connection can be established by connecting the profiler to the player.</para>
    /// </summary>
    public static class PlayerConnection
    {
        /// <summary>
        /// <para>Send a file from the player to the editor and save it on disk.
        /// You can specify either the absolute path or the relative path. When the path you specify is not absolute, it is relative to the project path.</para>
        /// </summary>
        /// <param name="remoteFilePath">File Path.</param>
        /// <param name="data">File contents.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SendFile(string remoteFilePath, byte[] data);

        /// <summary>
        /// <para>Returns true when Editor is connected to the player. When called in Editor, this function will always returns false.</para>
        /// </summary>
        public static bool connected { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

