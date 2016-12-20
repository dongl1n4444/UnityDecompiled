namespace UnityEngine.VR.WSA
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>SurfaceData is a container struct used for requesting baked spatial mapping data and receiving that data once baked.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SurfaceData
    {
        /// <summary>
        /// <para>This is the ID for the surface to be baked or the surface that was baked and being returned to the user.</para>
        /// </summary>
        public SurfaceId id;
        /// <summary>
        /// <para>This MeshFilter will receive the baked mesh prepared by the system when requesting baked surface data.  The MeshFilter is returned in the SurfaceDataReadyDelegate for those users requiring advanced workflows.</para>
        /// </summary>
        public MeshFilter outputMesh;
        /// <summary>
        /// <para>This WorldAnchor is used to lock the surface into place relative to real world objects.  It will be filled in when calling RequestMeshAsync to generate data for a surface and returned with the SurfaceDataReadyDelegate.</para>
        /// </summary>
        public WorldAnchor outputAnchor;
        /// <summary>
        /// <para>This MeshCollider will receive the baked physics mesh prepared by the system when requesting baked surface data through RequestMeshAsync.  The MeshCollider is returned in the SurfaceDataReadyDelegate for those users requiring advanced workflows.</para>
        /// </summary>
        public MeshCollider outputCollider;
        /// <summary>
        /// <para>This value controls the basic resolution of baked mesh data and is returned with the SurfaceDataReadyDelegate.  The device will deliver up to this number of triangles per cubic meter.</para>
        /// </summary>
        public float trianglesPerCubicMeter;
        /// <summary>
        /// <para>Set this field to true when requesting data to bake collider data.  This field will be set to true when receiving baked data if it was requested.  Setting this field to true requires that a valid outputCollider is also specified.</para>
        /// </summary>
        public bool bakeCollider;
        /// <summary>
        /// <para>Constructor for conveniently filling out a SurfaceData struct.</para>
        /// </summary>
        /// <param name="_id">ID for the surface in question.</param>
        /// <param name="_outputMesh">MeshFilter to write Mesh data to.</param>
        /// <param name="_outputAnchor">WorldAnchor receiving the anchor point for the surface.</param>
        /// <param name="_outputCollider">MeshCollider to write baked physics data to (optional).</param>
        /// <param name="_trianglesPerCubicMeter">Requested resolution for the computed Mesh.  Actual resolution may be less than this value.</param>
        /// <param name="_bakeCollider">Set to true if collider baking is/has been requested.</param>
        public SurfaceData(SurfaceId _id, MeshFilter _outputMesh, WorldAnchor _outputAnchor, MeshCollider _outputCollider, float _trianglesPerCubicMeter, bool _bakeCollider)
        {
            this.id = _id;
            this.outputMesh = _outputMesh;
            this.outputAnchor = _outputAnchor;
            this.outputCollider = _outputCollider;
            this.trianglesPerCubicMeter = _trianglesPerCubicMeter;
            this.bakeCollider = _bakeCollider;
        }
    }
}

