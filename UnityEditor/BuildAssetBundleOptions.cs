namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Asset Bundle building options.</para>
    /// </summary>
    [Flags]
    public enum BuildAssetBundleOptions
    {
        /// <summary>
        /// <para>Append the hash to the assetBundle name.</para>
        /// </summary>
        AppendHashToAssetBundleName = 0x80,
        /// <summary>
        /// <para>Use chunk-based LZ4 compression when creating the AssetBundle.</para>
        /// </summary>
        ChunkBasedCompression = 0x100,
        /// <summary>
        /// <para>Includes all dependencies.</para>
        /// </summary>
        [Obsolete("This has been made obsolete. It is always enabled in the new AssetBundle build system introduced in 5.0.")]
        CollectDependencies = 2,
        /// <summary>
        /// <para>Forces inclusion of the entire asset.</para>
        /// </summary>
        [Obsolete("This has been made obsolete. It is always enabled in the new AssetBundle build system introduced in 5.0.")]
        CompleteAssets = 4,
        /// <summary>
        /// <para>Builds an asset bundle using a hash for the id of the object stored in the asset bundle.</para>
        /// </summary>
        DeterministicAssetBundle = 0x10,
        /// <summary>
        /// <para>Do not include type information within the AssetBundle.</para>
        /// </summary>
        DisableWriteTypeTree = 8,
        /// <summary>
        /// <para>Do a dry run build.</para>
        /// </summary>
        DryRunBuild = 0x400,
        /// <summary>
        /// <para>Force rebuild the assetBundles.</para>
        /// </summary>
        ForceRebuildAssetBundle = 0x20,
        /// <summary>
        /// <para>Ignore the type tree changes when doing the incremental build check.</para>
        /// </summary>
        IgnoreTypeTreeChanges = 0x40,
        /// <summary>
        /// <para>Build assetBundle without any special option.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Do not allow the build to succeed if any errors are reporting during it.</para>
        /// </summary>
        StrictMode = 0x200,
        /// <summary>
        /// <para>Don't compress the data when creating the asset bundle.</para>
        /// </summary>
        UncompressedAssetBundle = 1
    }
}

