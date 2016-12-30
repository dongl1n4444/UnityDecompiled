﻿namespace UnityEngine.Networking.Match
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    /// <summary>
    /// <para>Details about a UNET MatchMaker match.</para>
    /// </summary>
    public class MatchInfo
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private NetworkAccessToken <accessToken>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <address>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private int <domain>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NetworkID <networkId>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private NodeID <nodeId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <port>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <usingRelay>k__BackingField;

        public MatchInfo()
        {
        }

        internal MatchInfo(CreateMatchResponse matchResponse)
        {
            this.address = matchResponse.address;
            this.port = matchResponse.port;
            this.domain = matchResponse.domain;
            this.networkId = matchResponse.networkId;
            this.accessToken = new NetworkAccessToken(matchResponse.accessTokenString);
            this.nodeId = matchResponse.nodeId;
            this.usingRelay = matchResponse.usingRelay;
        }

        internal MatchInfo(JoinMatchResponse matchResponse)
        {
            this.address = matchResponse.address;
            this.port = matchResponse.port;
            this.domain = matchResponse.domain;
            this.networkId = matchResponse.networkId;
            this.accessToken = new NetworkAccessToken(matchResponse.accessTokenString);
            this.nodeId = matchResponse.nodeId;
            this.usingRelay = matchResponse.usingRelay;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.networkId, this.address, this.port, this.nodeId, this.usingRelay };
            return UnityString.Format("{0} @ {1}:{2} [{3},{4}]", args);
        }

        /// <summary>
        /// <para>The binary access token this client uses to authenticate its session for future commands.</para>
        /// </summary>
        public NetworkAccessToken accessToken { get; private set; }

        /// <summary>
        /// <para>IP address of the host of the match,.</para>
        /// </summary>
        public string address { get; private set; }

        /// <summary>
        /// <para>The numeric domain for the match.</para>
        /// </summary>
        public int domain { get; private set; }

        /// <summary>
        /// <para>The unique ID of this match.</para>
        /// </summary>
        public NetworkID networkId { get; private set; }

        /// <summary>
        /// <para>NodeID for this member client in the match.</para>
        /// </summary>
        public NodeID nodeId { get; private set; }

        /// <summary>
        /// <para>Port of the host of the match.</para>
        /// </summary>
        public int port { get; private set; }

        /// <summary>
        /// <para>This flag indicates whether or not the match is using a Relay server.</para>
        /// </summary>
        public bool usingRelay { get; private set; }
    }
}

