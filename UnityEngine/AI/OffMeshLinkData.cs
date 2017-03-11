﻿namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// <para>State of OffMeshLink.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), MovedFrom("UnityEngine")]
    public struct OffMeshLinkData
    {
        private int m_Valid;
        private int m_Activated;
        private int m_InstanceID;
        private OffMeshLinkType m_LinkType;
        private Vector3 m_StartPos;
        private Vector3 m_EndPos;
        /// <summary>
        /// <para>Is link valid (Read Only).</para>
        /// </summary>
        public bool valid =>
            (this.m_Valid != 0);
        /// <summary>
        /// <para>Is link active (Read Only).</para>
        /// </summary>
        public bool activated =>
            (this.m_Activated != 0);
        /// <summary>
        /// <para>Link type specifier (Read Only).</para>
        /// </summary>
        public OffMeshLinkType linkType =>
            this.m_LinkType;
        /// <summary>
        /// <para>Link start world position (Read Only).</para>
        /// </summary>
        public Vector3 startPos =>
            this.m_StartPos;
        /// <summary>
        /// <para>Link end world position (Read Only).</para>
        /// </summary>
        public Vector3 endPos =>
            this.m_EndPos;
        /// <summary>
        /// <para>The OffMeshLink if the link type is a manually placed Offmeshlink (Read Only).</para>
        /// </summary>
        public OffMeshLink offMeshLink =>
            this.GetOffMeshLinkInternal(this.m_InstanceID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern OffMeshLink GetOffMeshLinkInternal(int instanceID);
    }
}

