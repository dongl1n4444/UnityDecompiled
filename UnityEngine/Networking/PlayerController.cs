namespace UnityEngine.Networking
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>This represents a networked player.</para>
    /// </summary>
    public class PlayerController
    {
        /// <summary>
        /// <para>The game object for this player.</para>
        /// </summary>
        public GameObject gameObject;
        internal const short kMaxLocalPlayers = 8;
        /// <summary>
        /// <para>The maximum number of local players that a client connection can have.</para>
        /// </summary>
        public const int MaxPlayersPerClient = 0x20;
        /// <summary>
        /// <para>The local player ID number of this player.</para>
        /// </summary>
        public short playerControllerId;
        /// <summary>
        /// <para>The NetworkIdentity component of the player.</para>
        /// </summary>
        public NetworkIdentity unetView;

        public PlayerController()
        {
            this.playerControllerId = -1;
        }

        internal PlayerController(GameObject go, short playerControllerId)
        {
            this.playerControllerId = -1;
            this.gameObject = go;
            this.unetView = go.GetComponent<NetworkIdentity>();
            this.playerControllerId = playerControllerId;
        }

        /// <summary>
        /// <para>String representation of the player objects state.</para>
        /// </summary>
        /// <returns>
        /// <para>String with the object state.</para>
        /// </returns>
        public override string ToString() => 
            $"ID={this.playerControllerId} NetworkIdentity NetID={((this.unetView == null) ? "null" : this.unetView.netId.ToString())} Player={((this.gameObject == null) ? "null" : this.gameObject.name)}";

        /// <summary>
        /// <para>Checks if this PlayerController has an actual player attached to it.</para>
        /// </summary>
        public bool IsValid =>
            (this.playerControllerId != -1);
    }
}

