namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Describes the culling information for a given shadow split (e.g. directional cascade).</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct ShadowSplitData
    {
        /// <summary>
        /// <para>The number of culling planes.</para>
        /// </summary>
        public int cullingPlaneCount;
        private <_cullingPlanes>__FixedBuffer0 _cullingPlanes;
        /// <summary>
        /// <para>The culling sphere.  The first three components of the vector describe the sphere center, and the last component specifies the radius.</para>
        /// </summary>
        public Vector4 cullingSphere;
        /// <summary>
        /// <para>Gets a culling plane.</para>
        /// </summary>
        /// <param name="index">The culling plane index.</param>
        /// <returns>
        /// <para>The culling plane.</para>
        /// </returns>
        public unsafe Plane GetCullingPlane(int index)
        {
            if (((index < 0) || (index >= this.cullingPlaneCount)) || (index >= 10))
            {
                throw new IndexOutOfRangeException("Invalid plane index");
            }
            fixed (float* numRef = &this._cullingPlanes.FixedElementField)
            {
                return new Plane(new Vector3(numRef[(index * 4) * 4], numRef[((index * 4) + 1) * 4], numRef[((index * 4) + 2) * 4]), numRef[((index * 4) + 3) * 4]);
            }
        }

        /// <summary>
        /// <para>Sets a culling plane.</para>
        /// </summary>
        /// <param name="index">The index of the culling plane to set.</param>
        /// <param name="plane">The culling plane.</param>
        public unsafe void SetCullingPlane(int index, Plane plane)
        {
            if (((index < 0) || (index >= this.cullingPlaneCount)) || (index >= 10))
            {
                throw new IndexOutOfRangeException("Invalid plane index");
            }
            fixed (float* numRef = &this._cullingPlanes.FixedElementField)
            {
                numRef[(index * 4) * 4] = plane.normal.x;
                numRef[((index * 4) + 1) * 4] = plane.normal.y;
                numRef[((index * 4) + 2) * 4] = plane.normal.z;
                numRef[((index * 4) + 3) * 4] = plane.distance;
            }
        }
        [StructLayout(LayoutKind.Sequential, Size=160), CompilerGenerated, UnsafeValueType]
        public struct <_cullingPlanes>__FixedBuffer0
        {
            public float FixedElementField;
        }
    }
}

