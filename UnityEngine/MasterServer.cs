﻿namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>The Master Server is used to make matchmaking between servers and clients easy.</para>
    /// </summary>
    public sealed class MasterServer
    {
        /// <summary>
        /// <para>Clear the host list which was received by MasterServer.PollHostList.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ClearHostList();
        /// <summary>
        /// <para>Check for the latest host list received by using MasterServer.RequestHostList.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern HostData[] PollHostList();
        /// <summary>
        /// <para>Register this server on the master server.</para>
        /// </summary>
        /// <param name="gameTypeName"></param>
        /// <param name="gameName"></param>
        /// <param name="comment"></param>
        [ExcludeFromDocs]
        public static void RegisterHost(string gameTypeName, string gameName)
        {
            string comment = "";
            RegisterHost(gameTypeName, gameName, comment);
        }

        /// <summary>
        /// <para>Register this server on the master server.</para>
        /// </summary>
        /// <param name="gameTypeName"></param>
        /// <param name="gameName"></param>
        /// <param name="comment"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RegisterHost(string gameTypeName, string gameName, [DefaultValue("\"\"")] string comment);
        /// <summary>
        /// <para>Request a host list from the master server.</para>
        /// </summary>
        /// <param name="gameTypeName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RequestHostList(string gameTypeName);
        /// <summary>
        /// <para>Unregister this server from the master server.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UnregisterHost();

        /// <summary>
        /// <para>Report this machine as a dedicated server.</para>
        /// </summary>
        public static bool dedicatedServer { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The IP address of the master server.</para>
        /// </summary>
        public static string ipAddress { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The connection port of the master server.</para>
        /// </summary>
        public static int port { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set the minimum update rate for master server host information update.</para>
        /// </summary>
        public static int updateRate { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

