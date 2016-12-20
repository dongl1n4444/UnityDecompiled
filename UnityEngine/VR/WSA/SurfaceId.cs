namespace UnityEngine.VR.WSA
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>SurfaceId is a structure wrapping the unique ID used to denote Surfaces.  SurfaceIds are provided through the onSurfaceChanged callback in Update and returned after a RequestMeshAsync call has completed.  SurfaceIds are guaranteed to be unique though Surfaces are sometimes replaced with a new Surface in the same location with a different ID.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SurfaceId
    {
        /// <summary>
        /// <para>The actual integer ID referring to a single surface.</para>
        /// </summary>
        public int handle;
    }
}

