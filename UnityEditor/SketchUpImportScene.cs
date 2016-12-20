namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Structure to hold scene data extracted from a SketchUp file.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SketchUpImportScene
    {
        /// <summary>
        /// <para>The camera data of the SketchUp scene.</para>
        /// </summary>
        public SketchUpImportCamera camera;
        /// <summary>
        /// <para>The name of the SketchUp scene.</para>
        /// </summary>
        public string name;
    }
}

