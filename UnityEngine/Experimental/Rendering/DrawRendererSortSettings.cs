namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Describes how to sort objects during rendering.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DrawRendererSortSettings
    {
        /// <summary>
        /// <para>Camera view matrix, used to determine distances to objects.</para>
        /// </summary>
        public Matrix4x4 worldToCameraMatrix;
        /// <summary>
        /// <para>Camera position, used to determine distances to objects.</para>
        /// </summary>
        public Vector3 cameraPosition;
        /// <summary>
        /// <para>What kind of sorting to do while rendering.</para>
        /// </summary>
        public SortFlags flags;
        private int _sortOrthographic;
        /// <summary>
        /// <para>Should orthographic sorting be used?</para>
        /// </summary>
        public bool sortOrthographic
        {
            get => 
                (this._sortOrthographic != 0);
            set
            {
                this._sortOrthographic = !value ? 0 : 1;
            }
        }
    }
}

