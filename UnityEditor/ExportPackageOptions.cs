namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Export package option. Multiple options can be combined together using the | operator.</para>
    /// </summary>
    [Flags]
    public enum ExportPackageOptions
    {
        /// <summary>
        /// <para>Default mode. Will not include dependencies or subdirectories nor include Library assets unless specifically included in the asset list.</para>
        /// </summary>
        Default = 0,
        /// <summary>
        /// <para>In addition to the assets paths listed, all dependent assets will be included as well.</para>
        /// </summary>
        IncludeDependencies = 4,
        /// <summary>
        /// <para>The exported package will include all library assets, ie. the project settings located in the Library folder of the project.</para>
        /// </summary>
        IncludeLibraryAssets = 8,
        /// <summary>
        /// <para>The export operation will be run asynchronously and reveal the exported package file in a file browser window after the export is finished.</para>
        /// </summary>
        Interactive = 1,
        /// <summary>
        /// <para>Will recurse through any subdirectories listed and include all assets inside them.</para>
        /// </summary>
        Recurse = 2
    }
}

