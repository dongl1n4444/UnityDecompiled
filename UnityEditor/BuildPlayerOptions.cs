namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Provide various options to control the behavior of BuildPipeline.BuildPlayer.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BuildPlayerOptions
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string[] <scenes>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <locationPathName>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <assetBundleManifestPath>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BuildTargetGroup <targetGroup>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private BuildTarget <target>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BuildOptions <options>k__BackingField;
        /// <summary>
        /// <para>The scenes to be included in the build. If empty, the currently open scene will be built. Paths are relative to the project folder (AssetsMyLevelsMyScene.unity).</para>
        /// </summary>
        public string[] scenes { get; set; }
        /// <summary>
        /// <para>The path where the application will be built.</para>
        /// </summary>
        public string locationPathName { get; set; }
        /// <summary>
        /// <para>The path to an manifest file describing all of the asset bundles used in the build (optional).</para>
        /// </summary>
        public string assetBundleManifestPath { get; set; }
        /// <summary>
        /// <para>The BuildTargetGroup to build.</para>
        /// </summary>
        public BuildTargetGroup targetGroup { get; set; }
        /// <summary>
        /// <para>The BuildTarget to build.</para>
        /// </summary>
        public BuildTarget target { get; set; }
        /// <summary>
        /// <para>Additional BuildOptions, like whether to run the built player.</para>
        /// </summary>
        public BuildOptions options { get; set; }
    }
}

