namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Structure to hold camera data extracted from a SketchUp file.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SketchUpImportCamera
    {
        /// <summary>
        /// <para>The position of the camera.</para>
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// <para>The position the camera is looking at.</para>
        /// </summary>
        public Vector3 lookAt;
        /// <summary>
        /// <para>Up vector of the camera.</para>
        /// </summary>
        public Vector3 up;
        /// <summary>
        /// <para>Field of view of the camera.</para>
        /// </summary>
        public float fieldOfView;
        /// <summary>
        /// <para>Aspect ratio of the camera.</para>
        /// </summary>
        public float aspectRatio;
        /// <summary>
        /// <para>The orthogonal projection size of the camera. This value only make sense if SketchUpImportCamera.isPerspective is false.</para>
        /// </summary>
        public float orthoSize;
        /// <summary>
        /// <para>Indicate if the camera is using a perspective or orthogonal projection.</para>
        /// </summary>
        public bool isPerspective;
    }
}

