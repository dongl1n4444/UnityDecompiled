namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Derives from AssetImporter to handle importing of SketchUp files.</para>
    /// </summary>
    public sealed class SketchUpImporter : ModelImporter
    {
        /// <summary>
        /// <para>The default camera or the camera of the active scene which the SketchUp file was saved with.</para>
        /// </summary>
        /// <returns>
        /// <para>The default camera.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern SketchUpImportCamera GetDefaultCamera();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern SketchUpNodeInfo[] GetNodes();
        /// <summary>
        /// <para>The method returns an array of SketchUpImportScene which represents SketchUp scenes.</para>
        /// </summary>
        /// <returns>
        /// <para>Array of scenes extracted from a SketchUp file.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern SketchUpImportScene[] GetScenes();

        /// <summary>
        /// <para>Retrieves the latitude Geo Coordinate imported from the SketchUp file.</para>
        /// </summary>
        public double latitude { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Retrieves the longitude Geo Coordinate imported from the SketchUp file.</para>
        /// </summary>
        public double longitude { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Retrieves the north correction value imported from the SketchUp file.</para>
        /// </summary>
        public double northCorrection { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

