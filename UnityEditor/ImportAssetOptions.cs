namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Asset importing options.</para>
    /// </summary>
    [Flags]
    public enum ImportAssetOptions
    {
        /// <summary>
        /// <para>Default import options.</para>
        /// </summary>
        Default = 0,
        /// <summary>
        /// <para>Force a full reimport but don't download the assets from the cache server.</para>
        /// </summary>
        DontDownloadFromCacheServer = 0x2000,
        /// <summary>
        /// <para>Import all assets synchronously.</para>
        /// </summary>
        ForceSynchronousImport = 8,
        /// <summary>
        /// <para>Forces asset import as uncompressed for edition facilities.</para>
        /// </summary>
        ForceUncompressedImport = 0x4000,
        /// <summary>
        /// <para>User initiated asset import.</para>
        /// </summary>
        ForceUpdate = 1,
        /// <summary>
        /// <para>When a folder is imported, import all its contents as well.</para>
        /// </summary>
        ImportRecursive = 0x100
    }
}

