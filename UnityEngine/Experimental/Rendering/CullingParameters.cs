namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Parameters controlling culling process in CullResults.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct CullingParameters
    {
        /// <summary>
        /// <para>True if camera is orthographic (this affects LOD culling).</para>
        /// </summary>
        public int isOrthographic;
        /// <summary>
        /// <para>LODGroup culling parameters.</para>
        /// </summary>
        public LODParameters lodParameters;
        private <_cullingPlanes>__FixedBuffer0 _cullingPlanes;
        /// <summary>
        /// <para>Number of culling planes to use.</para>
        /// </summary>
        public int cullingPlaneCount;
        /// <summary>
        /// <para>Layer mask used for culling.</para>
        /// </summary>
        public int cullingMask;
        private <_layerFarCullDistances>__FixedBuffer1 _layerFarCullDistances;
        private int layerCull;
        /// <summary>
        /// <para>World to clip space matrix.</para>
        /// </summary>
        public Matrix4x4 cullingMatrix;
        /// <summary>
        /// <para>Camera position.</para>
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// <para>Realtime shadows distance.</para>
        /// </summary>
        public float shadowDistance;
        private int _cullingFlags;
        private int _cameraInstanceID;
        /// <summary>
        /// <para>Visible reflection probes sorting options.</para>
        /// </summary>
        public ReflectionProbeSortOptions reflectionProbeSortOptions;
        /// <summary>
        /// <para>Get per-layer culling distance.</para>
        /// </summary>
        /// <param name="layerIndex">Game object layer index (0 to 31).</param>
        /// <returns>
        /// <para>Distance beyond which objects in this layer are culled.</para>
        /// </returns>
        public unsafe float GetLayerCullDistance(int layerIndex)
        {
            if ((layerIndex < 0) || (layerIndex >= 0x20))
            {
                throw new IndexOutOfRangeException("Invalid layer index");
            }
            fixed (float* numRef = &this._layerFarCullDistances.FixedElementField)
            {
                return numRef[layerIndex * 4];
            }
        }

        /// <summary>
        /// <para>Set per-layer culling distance.</para>
        /// </summary>
        /// <param name="layerIndex">Game object layer index (0 to 31).</param>
        /// <param name="distance">Distance beyond which objects in this layer are culled.</param>
        public unsafe void SetLayerCullDistance(int layerIndex, float distance)
        {
            if ((layerIndex < 0) || (layerIndex >= 0x20))
            {
                throw new IndexOutOfRangeException("Invalid layer index");
            }
            fixed (float* numRef = &this._layerFarCullDistances.FixedElementField)
            {
                numRef[layerIndex * 4] = distance;
            }
        }

        /// <summary>
        /// <para>Get a culling plane.</para>
        /// </summary>
        /// <param name="index">Plane index (up to cullingPlaneCount).</param>
        /// <returns>
        /// <para>Culling plane.</para>
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
        /// <para>Set a culling plane.</para>
        /// </summary>
        /// <param name="index">Plane index (up to cullingPlaneCount).</param>
        /// <param name="plane">Culling plane.</param>
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
        [StructLayout(LayoutKind.Sequential, Size=160), UnsafeValueType, CompilerGenerated]
        public struct <_cullingPlanes>__FixedBuffer0
        {
            public float FixedElementField;
        }

        [StructLayout(LayoutKind.Sequential, Size=0x80), UnsafeValueType, CompilerGenerated]
        public struct <_layerFarCullDistances>__FixedBuffer1
        {
            public float FixedElementField;
        }
    }
}

