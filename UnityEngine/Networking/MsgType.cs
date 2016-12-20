namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>Container class for networking system built-in message types.</para>
    /// </summary>
    public class MsgType
    {
        /// <summary>
        /// <para>Internal networking system message for adding player objects to client instances.</para>
        /// </summary>
        public const short AddPlayer = 0x25;
        /// <summary>
        /// <para>Internal networking system message for sending synchronizing animation state.</para>
        /// </summary>
        public const short Animation = 40;
        /// <summary>
        /// <para>Internal networking system message for sending synchronizing animation parameter state.</para>
        /// </summary>
        public const short AnimationParameters = 0x29;
        /// <summary>
        /// <para>Internal networking system message for sending animation triggers.</para>
        /// </summary>
        public const short AnimationTrigger = 0x2a;
        /// <summary>
        /// <para>Internal networking system message for sending a command from client to server.</para>
        /// </summary>
        public const short Command = 5;
        /// <summary>
        /// <para>Internal networking system message for communicating a connection has occurred.</para>
        /// </summary>
        public const short Connect = 0x20;
        /// <summary>
        /// <para>Internal networking system message for HLAPI CRC checking.</para>
        /// </summary>
        public const short CRC = 14;
        /// <summary>
        /// <para>Internal networking system message for communicating a disconnect has occurred,.</para>
        /// </summary>
        public const short Disconnect = 0x21;
        /// <summary>
        /// <para>Internal networking system message for communicating an error.</para>
        /// </summary>
        public const short Error = 0x22;
        /// <summary>
        /// <para>Internal networking system message for identifying fragmented packets.</para>
        /// </summary>
        public const short Fragment = 0x11;
        /// <summary>
        /// <para>The highest value of built-in networking system message ids. User messages must be above this value.</para>
        /// </summary>
        public const short Highest = 0x2f;
        internal const short HLAPIMsg = 0x1c;
        internal const short HLAPIPending = 0x1f;
        internal const short HLAPIResend = 30;
        /// <summary>
        /// <para>The highest value of internal networking system message ids. User messages must be above this value. User code cannot replace these handlers.</para>
        /// </summary>
        public const short InternalHighest = 0x1f;
        internal const short LLAPIMsg = 0x1d;
        /// <summary>
        /// <para>Internal networking system message for communicating failing to add lobby player.</para>
        /// </summary>
        public const short LobbyAddPlayerFailed = 0x2d;
        /// <summary>
        /// <para>Internal networking system message for communicating a player is ready in the lobby.</para>
        /// </summary>
        public const short LobbyReadyToBegin = 0x2b;
        /// <summary>
        /// <para>Internal networking system messages used to return the game to the lobby scene.</para>
        /// </summary>
        public const short LobbyReturnToLobby = 0x2e;
        /// <summary>
        /// <para>Internal networking system message for communicating a lobby player has loaded the game scene.</para>
        /// </summary>
        public const short LobbySceneLoaded = 0x2c;
        /// <summary>
        /// <para>Internal networking system message for sending tranforms for client object from client to server.</para>
        /// </summary>
        public const short LocalChildTransform = 0x10;
        /// <summary>
        /// <para>Internal networking system message for setting authority to a client for an object.</para>
        /// </summary>
        public const short LocalClientAuthority = 15;
        /// <summary>
        /// <para>Internal networking system message for sending tranforms from client to server.</para>
        /// </summary>
        public const short LocalPlayerTransform = 6;
        internal static string[] msgLabels = new string[] { 
            "none", "ObjectDestroy", "Rpc", "ObjectSpawn", "Owner", "Command", "LocalPlayerTransform", "SyncEvent", "UpdateVars", "SyncList", "ObjectSpawnScene", "NetworkInfo", "SpawnFinished", "ObjectHide", "CRC", "LocalClientAuthority",
            "LocalChildTransform", "Fragment", "PeerClientAuthority", "", "", "", "", "", "", "", "", "", "", "", "", "",
            "Connect", "Disconnect", "Error", "Ready", "NotReady", "AddPlayer", "RemovePlayer", "Scene", "Animation", "AnimationParams", "AnimationTrigger", "LobbyReadyToBegin", "LobbySceneLoaded", "LobbyAddPlayerFailed", "LobbyReturnToLobby", "ReconnectPlayer"
        };
        /// <summary>
        /// <para>Internal networking system message for sending information about network peers to clients.</para>
        /// </summary>
        public const short NetworkInfo = 11;
        /// <summary>
        /// <para>Internal networking system message for server to tell clients they are no longer ready.</para>
        /// </summary>
        public const short NotReady = 0x24;
        /// <summary>
        /// <para>Internal networking system message for destroying objects.</para>
        /// </summary>
        public const short ObjectDestroy = 1;
        /// <summary>
        /// <para>Internal networking system message for hiding objects.</para>
        /// </summary>
        public const short ObjectHide = 13;
        /// <summary>
        /// <para>Internal networking system message for spawning objects.</para>
        /// </summary>
        public const short ObjectSpawn = 3;
        /// <summary>
        /// <para>Internal networking system message for spawning scene objects.</para>
        /// </summary>
        public const short ObjectSpawnScene = 10;
        /// <summary>
        /// <para>Internal networking system message for telling clients they own a player object.</para>
        /// </summary>
        public const short Owner = 4;
        /// <summary>
        /// <para>Internal networking system message for sending information about changes in authority for non-player objects to clients.</para>
        /// </summary>
        public const short PeerClientAuthority = 0x12;
        /// <summary>
        /// <para>Internal networking system message for clients to tell server they are ready.</para>
        /// </summary>
        public const short Ready = 0x23;
        /// <summary>
        /// <para>Internal networking system message used when a client connects to the new host of a game.</para>
        /// </summary>
        public const short ReconnectPlayer = 0x2f;
        /// <summary>
        /// <para>Internal networking system message for removing a player object which was spawned for a client.</para>
        /// </summary>
        public const short RemovePlayer = 0x26;
        /// <summary>
        /// <para>Internal networking system message for sending a ClientRPC from server to client.</para>
        /// </summary>
        public const short Rpc = 2;
        /// <summary>
        /// <para>Internal networking system message that tells clients which scene to load when they connect to a server.</para>
        /// </summary>
        public const short Scene = 0x27;
        /// <summary>
        /// <para>Internal networking system messages used to tell when the initial contents of a scene is being spawned.</para>
        /// </summary>
        public const short SpawnFinished = 12;
        /// <summary>
        /// <para>Internal networking system message for sending a SyncEvent from server to client.</para>
        /// </summary>
        public const short SyncEvent = 7;
        /// <summary>
        /// <para>Internal networking system message for sending a USyncList generic list.</para>
        /// </summary>
        public const short SyncList = 9;
        /// <summary>
        /// <para>Internal networking system message for updating SyncVars on a client from a server.</para>
        /// </summary>
        public const short UpdateVars = 8;
        internal const short UserMessage = 0;

        /// <summary>
        /// <para>Returns the name of internal message types by their id.</para>
        /// </summary>
        /// <param name="value">A internal message id value.</param>
        /// <returns>
        /// <para>The name of the internal message.</para>
        /// </returns>
        public static string MsgTypeToString(short value)
        {
            if ((value < 0) || (value > 0x2f))
            {
                return string.Empty;
            }
            string str2 = msgLabels[value];
            if (string.IsNullOrEmpty(str2))
            {
                str2 = "[" + value + "]";
            }
            return str2;
        }
    }
}

