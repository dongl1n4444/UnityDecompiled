namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>LODGroup culling parameters.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LODParameters
    {
        /// <summary>
        /// <para>Indicates whether camera is orthographic.</para>
        /// </summary>
        public bool isOrthographic;
        /// <summary>
        /// <para>Camera position.</para>
        /// </summary>
        public Vector3 cameraPosition;
        /// <summary>
        /// <para>Camera's field of view.</para>
        /// </summary>
        public float fieldOfView;
        /// <summary>
        /// <para>Orhographic camera size.</para>
        /// </summary>
        public float orthoSize;
        /// <summary>
        /// <para>Rendering view height in pixels.</para>
        /// </summary>
        public int cameraPixelHeight;
    }
}

