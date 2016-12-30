namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Building options. Multiple options can be combined together.</para>
    /// </summary>
    [Flags]
    public enum BuildOptions
    {
        /// <summary>
        /// <para>Used when building Xcode (iOS) or Eclipse (Android) projects.</para>
        /// </summary>
        AcceptExternalModificationsToPlayer = 0x20,
        /// <summary>
        /// <para>Allow script debuggers to attach to the player remotely.</para>
        /// </summary>
        AllowDebugging = 0x200,
        /// <summary>
        /// <para>Run the built player.</para>
        /// </summary>
        AutoRunPlayer = 4,
        /// <summary>
        /// <para>Build a compressed asset bundle that contains streamed scenes loadable with the WWW class.</para>
        /// </summary>
        BuildAdditionalStreamedScenes = 0x10,
        /// <summary>
        /// <para>Build only the scripts of a project.</para>
        /// </summary>
        BuildScriptsOnly = 0x8000,
        [Obsolete("Texture Compression is now always enabled")]
        CompressTextures = 0,
        ComputeCRC = 0x100000,
        /// <summary>
        /// <para>Sets the Player to connect to the Editor.</para>
        /// </summary>
        ConnectToHost = 0x1000,
        /// <summary>
        /// <para>Start the player with a connection to the profiler in the editor.</para>
        /// </summary>
        ConnectWithProfiler = 0x100,
        /// <summary>
        /// <para>Build a development version of the player.</para>
        /// </summary>
        Development = 1,
        /// <summary>
        /// <para>Build headless Linux standalone.</para>
        /// </summary>
        EnableHeadlessMode = 0x4000,
        /// <summary>
        /// <para>Include assertions in the build. By default, the assertions are only included in development builds.</para>
        /// </summary>
        ForceEnableAssertions = 0x20000,
        /// <summary>
        /// <para>Force full optimizations for script complilation in Development builds.</para>
        /// </summary>
        ForceOptimizeScriptCompilation = 0x80000,
        Il2CPP = 0x10000,
        InstallInBuildFolder = 0x40,
        /// <summary>
        /// <para>Perform the specified build without any special settings or extra tasks.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Show the built player.</para>
        /// </summary>
        ShowBuiltPlayer = 8,
        /// <summary>
        /// <para>Do not allow the build to succeed if any errors are reporting during it.</para>
        /// </summary>
        StrictMode = 0x200000,
        [Obsolete("Use BuildOptions.Development instead")]
        StripDebugSymbols = 0,
        /// <summary>
        /// <para>Symlink runtime libraries when generating iOS Xcode project. (Faster iteration time).</para>
        /// </summary>
        SymlinkLibraries = 0x400,
        /// <summary>
        /// <para>Don't compress the data when creating the asset bundle.</para>
        /// </summary>
        UncompressedAssetBundle = 0x800,
        /// <summary>
        /// <para>Copy UnityObject.js alongside Web Player so it wouldn't have to be downloaded from internet.</para>
        /// </summary>
        [Obsolete("WebPlayer has been removed in 5.4")]
        WebPlayerOfflineDeployment = 0x80
    }
}

