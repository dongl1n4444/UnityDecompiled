namespace UnityEditor.Networking.PlayerConnection
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Information of the connected player.</para>
    /// </summary>
    [Serializable]
    public class ConnectedPlayer
    {
        [SerializeField]
        private int m_PlayerId;

        public ConnectedPlayer()
        {
        }

        public ConnectedPlayer(int playerId)
        {
            this.m_PlayerId = playerId;
        }

        /// <summary>
        /// <para>The Id of the player connected.</para>
        /// </summary>
        public int PlayerId =>
            this.m_PlayerId;
    }
}

